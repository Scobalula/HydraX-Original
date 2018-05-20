/*
    HydraX - Black Ops III Fast File Exporter
    Copyright (C) 2018 Philip/Scobalula

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PhilUtil;
using CompressionUtil;
using Assets;


namespace T7FastFileUtil
{
    /// <summary>
    /// Fast File Compression Types
    /// </summary>
    public enum CompressionType
    {
        NONE,
        ZLIB,
        UNKOWN,
        LZ4,

    }

    /// <summary>
    /// T7 Fast File 
    /// </summary>
    public class FastFile
    {
        /// <summary>
        /// Fast File Header
        /// </summary>
        public class FastFileHeader
        {
            /// <summary>
            /// Fast File 8 Byte Magic
            /// </summary>
            public string Magic { get; set; }

            /// <summary>
            /// Fast File Version
            /// </summary>
            public int Version { get; set; }

            /// <summary>
            /// Fast File Compression Type
            /// </summary>
            public CompressionType Compression { get; set; }

            /// <summary>
            /// Fast File Name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Fast File Build Machine
            /// </summary>
            public string BuildMachine { get; set; }

            /// <summary>
            /// Fast File Decoded Size
            /// </summary>
            public int DecodedSize { get; set; }

            /// <summary>
            /// Loads 16 Byte Fast File Header
            /// </summary>
            /// <param name="inputStream"></param>
            /// <returns></returns>
            public static FastFileHeader Load(BinaryReader inputStream)
            {
                FastFileHeader header = new FastFileHeader();

                header.Magic            = inputStream.ReadFixedString(8);
                header.Version          = inputStream.ReadInt32();
                header.Compression      = (CompressionType)(inputStream.ReadInt32() >> 8);
                inputStream.Seek(28, SeekOrigin.Current);
                header.BuildMachine     = inputStream.ReadFixedString(32);
                inputStream.Seek(68, SeekOrigin.Current);
                header.DecodedSize      = inputStream.ReadInt32();
                inputStream.Seek(100, SeekOrigin.Current);
                header.Name             = inputStream.ReadFixedString(32);
                inputStream.Seek(304, SeekOrigin.Current);

                return header;
            }
        }


        /// <summary>
        /// Default Asset Byte Sequence to Search for
        /// </summary>
        public static byte[] DefaultNeedle = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

        /// <summary>
        /// Fast File Header
        /// </summary>
        public FastFileHeader Header { get; set; }

        /// <summary>
        /// Fast File Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Fast File Build Machine
        /// </summary>
        public string BuildMachine { get; set; }

        /// <summary>
        /// Fast File Path
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Fast File Decoded Path
        /// </summary>
        public string DecodedFile { get; set; }

        /// <summary>
        /// Fast File Size
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Fast File Decoded Size
        /// </summary>
        public int DecodedSize { get; set; }

        /// <summary>
        /// Fast File Asset Count
        /// </summary>
        public int Assets { get; set; }

        /// <summary>
        /// Fast File Strings
        /// </summary>
        private string[] Strings { get; set; }

        /// <summary>
        /// Fast File Decoded Binary Reader
        /// </summary>
        public BinaryReader DecodedStream { get; set; }

        /// <summary>
        /// Decodes Fast File to a new file
        /// </summary>
        /// <param name="fileName">Fast File Path</param>
        public void Decode(string fileName)
        {
            File = fileName;

            string output = File + ".decoded.dat";

            using (BinaryReader reader = new BinaryReader(new FileStream(fileName, FileMode.Open)))
            {
                // Load 584 Bytes Header containing Magic, Version, etc.
                Header = FastFileHeader.Load(reader);

                Print.Info(String.Format("FF Name           : {0}",     Header.Name));
                Print.Info(String.Format("FF Header         : {0}",     Header.Magic));
                Print.Info(String.Format("FF Version        : 0x{0:X}", Header.Version));
                Print.Info(String.Format("FF BuildMachine   : {0}",     Header.BuildMachine));
                Print.Info(String.Format("FF Compression    : {0}",     Header.Compression));
                Print.Info(String.Format("FF Size           : 0x{0:X}", reader.BaseStream.Length));
                Print.Info(String.Format("FF Decoded Size   : 0x{0:X}", Header.DecodedSize));

                // Verify Header's data, ensuring this FF came from T7 and is ZLIB FF
                if(Header.Magic != "TAff0000")
                {
                    Print.Error(String.Format("File Magic \"{0}\" does not match T7's \"TAff0000\"", Header.Magic));
                    return;
                }

                if (Header.Version != 0x251)
                {
                    Print.Error(string.Format("Fast File Version \"0x{0:x}\" does not match T7's \"0x251\"", Header.Version));
                    return;
                }

                if (Header.Compression != CompressionType.ZLIB)
                {
                    Print.Error(string.Format("Invalid Compression Type, Expecting ZLIB Fast File."));
                    return;
                }

                // Decode FF to Disk
                using (var progressBar = new ProgressBar())
                {
                    using (var outputStream = new FileStream(output, FileMode.Create))
                    {
                        Print.Info();
                        Print.Info("Decoding: ", false);

                        for(int blockSize = -1; blockSize != 0;)
                        {
                            // Report to Progress based off our current position in file
                            progressBar.Report((double)reader.BaseStream.Position / reader.BaseStream.Length);

                            blockSize = reader.ReadInt32();
                            int decodedBlockSize = reader.ReadInt32();
                            // Sometimes not exactly same as blockSize
                            // but is always the size of the block?
                            int blockSize2 = reader.ReadInt32();
                            // "blockSize" pos should = this
                            int blockPosition = reader.ReadInt32();
                            int blockHeader = reader.ReadInt16();

                            // There's blocks of data I don't quite understand
                            // in the FF, this is a hacky way through skip it.
                            if (blockHeader != 0xD48)
                            {
                                if (!ResolveNextZlibBlock(reader))
                                    break;

                                continue;
                            }

                            try
                            {
                                DeflateUtil.Decode(reader.ReadBytes(blockSize2 - 2)).CopyTo(outputStream);
                            }
                            catch(Exception e)
                            {
                                progressBar.PrintFinal = false;
                                Console.WriteLine();
                                Print.Error(String.Format("Decompression Failed @ Block 0x{0:X}", blockPosition));
                                Print.Exception(e);
                                return;
                            }
                        }
                    }
                }
                Console.WriteLine();
                Print.Info();

                DecodedFile = output;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Load()
        {
            if (!FileUtil.CanAccessFile(DecodedFile))
            {
                Print.Error("Failed to load Decoded File. File doesn't exist or Permissions Denied.");
                return;
            }

            using (DecodedStream = new BinaryReader(new FileStream(DecodedFile, FileMode.Open)))
            {
                LoadStrings();

                Print.Info("Searching for assets, please wait...");
                Print.Info();

                long[] offsets = DecodedStream.FindBytes(DefaultNeedle);

                for(int i = 0; i < offsets.Length; i++)
                {
                    ProcessPotentialAsset(offsets[i]);
                }
            }
        }

        /// <summary>
        /// Processes a potential asset 
        /// </summary>
        /// <param name="offset"></param>
        public void ProcessPotentialAsset(long offset)
        {

            DecodedStream.Seek(offset, SeekOrigin.Begin);


            // Read as a buffer to reduce IO calls
            byte[] buffer = DecodedStream.ReadBytes(384);

            int stringOffset = 0;

            // If these are -1 or 0 etc. then extend our
            // "stringOffset" as these bytes won't be a str
            if (BitConverter.ToInt64(buffer, 0) == -1)
            {
                if (BitConverter.ToInt32(buffer, 11) == 0)
                    stringOffset = 16;
                else
                    stringOffset = 8;
            }
            else if(BitConverter.ToInt32(buffer, 3) == 0)
            {
                stringOffset = 8;
            }

            // Keep at max of 255 (we shouldn't be dealing with longer)


            string name = ByteUtil.ReadCString(buffer, stringOffset, 255);

            // Take only A-Z, Slashes, _ and .
            if (!Regex.Match(name, @"^[\\/a-zA-Z0-9_.]+$").Success)
                return;

            // Avoid issues with Path class
            string extension = name.Split('.').Last();




            // Switch known extensions
            switch (extension)
            {
                // String Tables
                case "csv":
                    Print.Export(name, offset);
                    DecodedStream.Seek(offset - 16, SeekOrigin.Begin);
                    try
                    {
                        StringTable.Decompile(DecodedStream);
                    }
                    catch
                    {
                        return;
                    }
                    break;
                // "RawFiles" (including LUA and GSC/CSC)
                case "script":
                case "lua":
                case "gsc":
                case "csc":
                case "gsh":
                case "vision":
                case "cfg":
                case "graph":
                case "txt":
                    Print.Export(name, offset);
                    DecodedStream.Seek(offset - 16, SeekOrigin.Begin);
                    RawFile.Export(DecodedStream, extension);
                    break;
                // Anim Trees
                case "atr":
                    Print.Export(name, offset);
                    DecodedStream.Seek(offset - 16, SeekOrigin.Begin);
                    AnimTree.Decompress(DecodedStream);
                    break;
                // D3DBSP Ent Maps
                case "d3dbsp":
                    Print.Export(name, offset);
                    DecodedStream.Seek(offset - 56, SeekOrigin.Begin);
                    D3DBSP.Export(DecodedStream);
                    break;
                // AI BTs
                case "ai_asm":
                    Print.Export(name, offset);
                    DecodedStream.Seek(offset - 32, SeekOrigin.Begin);
                    AnimStateMachine.Decompile(this);
                    break;
                case "ai_ast":
                    Print.Export(name, offset);
                    DecodedStream.Seek(offset + 8, SeekOrigin.Begin);
                    AnimSelectorTable.Decompile(this);
                    break;
                case "ai_am":
                    Print.Export(name, offset);
                    DecodedStream.Seek(offset + 8, SeekOrigin.Begin);
                    AnimMappingTable.Decompile(this);
                    break;
                case "ai_bt":
                    Print.Export(name, offset);
                    DecodedStream.Seek(offset - 16, SeekOrigin.Begin);
                    BehaviorTree.Decompile(this);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Attempts to find next ZLIB Block if we hit a non-ZLIB Block
        /// </summary>
        /// <param name="input">Input Reader</param>
        /// <returns>True/False if found.</returns>
        public bool ResolveNextZlibBlock(BinaryReader input)
        {
            // T7 FF ZLIB Header
            byte[] needle = { 0x48, 0x0D };
            // Loop until we find valid block
            while (true)
            {
                // Search for next ZLIB Header
                long[] result = input.FindBytes(needle, true, input.BaseStream.Position);
                // EOF or no headers
                if (result.Length == 0)
                    return false;
                // Seek back to block Position
                input.Seek(result[0] - 6, SeekOrigin.Begin);
                // Check Block Position
                if (input.ReadInt32() == input.BaseStream.Position - 16)
                {
                    input.Seek(result[0] - 18, SeekOrigin.Begin);
                    return true;
                }
                else
                {
                    input.Seek(result[0] + 2, SeekOrigin.Begin);
                    continue;
                }
            }
        }

        /// <summary>
        /// Loads Fast File String Table
        /// </summary>
        private void LoadStrings()
        {
            // These strings are used by anims, models, AI files, etc.
            // and need to be loaded.
            if(DecodedStream != null)
            {
                DecodedStream.Seek(0, SeekOrigin.Begin);

                Strings = new string[DecodedStream.ReadInt32()];

                DecodedStream.Seek(56 + (Strings.Length - 1) * 8, SeekOrigin.Begin);

                using (StreamWriter output = new StreamWriter(File + ".strings.txt"))
                {
                    for(int i = 0; i < Strings.Length; i++)
                    {
                        Strings[i] = DecodedStream.ReadCString();

                        GlobalStringTable.Strings[Hash.DJB(Encoding.ASCII.GetBytes(Strings[i]))] = Strings[i];

                        output.WriteLine("{0} - {1}", i, Strings[i]);
                    }
                }

            }
        }

        /// <summary>
        /// Gets a string from the Fast Files String Table. 
        /// </summary>
        /// <param name="index">Index of the string.</param>
        /// <returns>String if exists, otherwise null.</returns>
        public string GetString(int index)
        {
            return Strings?.ElementAtOrDefault(index);
        }
    }
}
