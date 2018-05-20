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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PhilUtil;
using ZLibNet;
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
    /// T7 Fast File Processor
    /// </summary>
    class T7FastFile
    {
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
                /// Loads 16 Byte Fast File Header
                /// </summary>
                /// <param name="inputStream"></param>
                /// <returns></returns>
                public static FastFileHeader Load(BinaryReader inputStream)
                {
                    FastFileHeader header = new FastFileHeader();

                    header.Magic = inputStream.ReadFixedString(8);
                    header.Version = inputStream.ReadInt32();
                    header.Compression = (CompressionType)(inputStream.ReadInt32() >> 8);

                    return header;
                }
            }

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


            public void Decode(string fileName)
            {
                File = fileName;

                DecodedFile = File + ".decoded.dat";

                using (BinaryReader stream = new BinaryReader(new FileStream(fileName, FileMode.Open)))
                {
                    FastFileHeader header = FastFileHeader.Load(stream);


                }
            }
        }

        /// <summary>
        /// Default Asset Byte Sequence to Search for
        /// </summary>
        public static byte[] DefaultNeedle = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };

        /// <summary>
        /// Decodes a ZLIB Black Ops III Fast File
        /// </summary>
        /// <param name="input">Input File Path</param>
        /// <param name="output">Output File Path</param>
        /// <returns>True/False if successful.</returns>
        public static FastFile Decode(string input, string output)
        {
            using (BinaryReader reader = new BinaryReader(new FileStream(input, FileMode.Open)))
            {

                // Fast File's Header
                string magic = Encoding.ASCII.GetString(reader.ReadBytes(8));
                int version = reader.ReadInt32();
                CompressionType compressionType = (CompressionType)(reader.ReadInt32() >> 8);
                reader.Seek(28, SeekOrigin.Current);
                string buildMachine = Regex.Replace(Encoding.ASCII.GetString(reader.ReadBytes(32)).Trim(), "[^a-zA-Z0-9 -_]", "");
                reader.Seek(68, SeekOrigin.Current);
                int decompressedSize = reader.ReadInt32();
                reader.Seek(100, SeekOrigin.Current);
                string fastfileName = Regex.Replace(Encoding.ASCII.GetString(reader.ReadBytes(64)).Trim(), "[^a-zA-Z0-9 -_]", "");
                reader.Seek(272, SeekOrigin.Current);
                // Tell the boyo some info
                Print.Info(string.Format("FF Name           : {0}", fastfileName));
                Print.Info(string.Format("FF Header         : {0}", magic));
                Print.Info(string.Format("FF Version        : 0x{0:X}", version));
                Print.Info(string.Format("FF BuildMachine   : {0}", buildMachine));
                Print.Info(string.Format("FF Compression    : {0}", compressionType));
                Print.Info(string.Format("FF Size           : 0x{0:X}", reader.BaseStream.Length));
                Print.Info(string.Format("FF Decoded Size   : 0x{0:X}", decompressedSize));

                // Check Magic
                if (magic != "TAff0000")
                {
                    Print.Error(string.Format("File Magic \"{0}\" does not match T7's \"TAff0000\"", magic));
                    return null;
                }

                // Check Version
                if(version != 0x251)
                {
                    Print.Error(string.Format("Fast File Version \"0x{0:x}\" does not match T7's \"0x251\"", version));
                    return null;
                }
                
                // Check Compression Type
                // TODO: Support None and LZ4 and figure out the other compression type
                if(!Enum.IsDefined(typeof(CompressionType), compressionType) || compressionType != CompressionType.ZLIB)
                {
                    Print.Error(string.Format("Invalid Compression Type, Expecting ZLIB Fast File."));
                    return null;
                }
                    
                // Execute Fast File Decompression
                using (var progressBar = new ProgressBar())
                {
                    using (var outputStream = new FileStream(output, FileMode.Create))
                    {
                        Print.Info();
                        Print.Info("Decoding: ", false);
                        // Decode until 0 block size
                        for (
                            int blockSize = -1;
                            blockSize != 0;
                            )
                        {
                            progressBar.Report(
                                (double)reader.BaseStream.Position
                                / reader.BaseStream.Length);
                            blockSize = reader.ReadInt32();
                            int decodedBlockSize = reader.ReadInt32();
                            // Sometimes not exactly same as blockSize
                            // but is always the size of the block?
                            int blockSize2 = reader.ReadInt32();
                            // "blockSize" pos should = this
                            int blockPosition = reader.ReadInt32();
                            int blockHeader = reader.ReadInt16();

                            if (blockHeader != 0xD48)
                            {
                                if (!ResolveNextZlibBlock(reader))
                                    break;

                                continue;
                            }
                            // Retreat on any Psh's
                            try
                            {
                                new DeflateStream(
                                    new MemoryStream(reader.ReadBytes(blockSize2 - 2)),
                                    CompressionMode.Decompress).CopyTo(outputStream);
                            }
                            catch(Exception e)
                            {
                                Print.Error("I ran into an issue and I'm too dumb to figure it out.");
                                Print.Error("Take this to my creator:");
                                Print.Error(e);
                                return null;
                            }
                        }
                    }
                }
                Console.WriteLine();
                Print.Info();
                return null;
            }
        }

        public static List<string> Strings = new List<string>();

        /// <summary>
        /// Attempts to find next ZLIB Block if we hit a non-ZLIB Block
        /// </summary>
        /// <param name="input">Input Reader</param>
        /// <returns>True/False if found.</returns>
        public static bool ResolveNextZlibBlock(BinaryReader input)
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
                if(input.ReadInt32() == input.BaseStream.Position - 16)
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
        /// Handles Processing Decoded Fast File
        /// </summary>
        /// <param name="input">Decoded File Name</param>
        /// <returns>Bool</returns>
        public static bool Process(string input)
        {
            using (var input_stream = new BinaryReader(new FileStream(input, FileMode.Open)))
            {
                ProcessFastFileStrings(input_stream);

                Print.Info("Scanning for assets, please wait...");
                Print.Info();

                List<long> offsets = new List<long>();
                offsets.AddRange(input_stream.FindBytes(DefaultNeedle));
                offsets.Sort();

                foreach (long offset in offsets)
                {
                    input_stream.Seek(offset, SeekOrigin.Begin);
                    ProcessPotentialAsset(input_stream, offset);
                }
            }
            return true;
        }

        /// <summary>
        /// Handles loading the Fast Files String Table
        /// </summary>
        /// <param name="reader">Input Reader</param>
        static void ProcessFastFileStrings(BinaryReader input)
        {
            // Number of Strings
            int numStrings = input.ReadInt32();
            input.Seek(28, SeekOrigin.Current);
            int numAssets = input.ReadInt32();
            // Seek to the strings
            input.Seek(56 + (numStrings - 1) * 8, SeekOrigin.Begin);
            using (StreamWriter output = new StreamWriter("StringTable.txt"))
            {
                for (int i = 0; i < numStrings; i++)
                {
                    string str = input.ReadCString();
                    // Hash and add to our Hash Table.
                    GlobalStringTable.Strings[Hash.DJB(Encoding.ASCII.GetBytes(str))] = str;
                    // Add to the fast files list.
                    // Strings.Add(str);
                    output.WriteLine("{0} - {1}", i, str);
                }
            }
            // Dump Info
            Print.Info(string.Format("String Count      : {0}", numStrings));
            Print.Info(string.Format("Asset Count       : {0}", numAssets));
            Print.Info(string.Format("Stream Length     : 0x{0:X}", input.BaseStream.Length));
            Print.Info();

        }
        /// <summary>
        /// Handles Processing Potential xAsset
        /// </summary>
        /// <param name="input">Input Reader</param>
        /// <param name="offset">Location of Potential Asset</param>
        public static void ProcessPotentialAsset(BinaryReader input, long offset)
        {
            // Don't continue if we're at EOF
            if(input.BaseStream.Position == input.BaseStream.Length)
                return;
            // Check next 8 bytes is 
            if (input.ReadUInt64() != ulong.MaxValue)
            {
                input.Seek(-8, SeekOrigin.Current);
            }

            int assetSize = input.ReadInt32();
            int nullBytes = input.ReadInt32();

            if(nullBytes != 0)
            {
                input.Seek(-8, SeekOrigin.Current);
            }
            else
            {
                offset = input.BaseStream.Position;
            }
            // Asset Name
            string name;
            // Read Name, Cap at 192 to avoid it going haywire.
            try
            {
                name = input.ReadCString(192);
            }
            catch
            {
                return;
            }



            if (!Regex.Match(name, @"^[\\/a-zA-Z0-9_.]+$").Success)
                return;

            // Get Extension, use split to avoid invalid path issues
            var extension = name.Split('.').Last();

            /*
            if (!Settings.ActiveSettings.Export.ContainsKey(extension))
                return;

            if (!Settings.ActiveSettings.Export[extension])
                return;
            */
            if (extension != "ai_bt")
                return;
            // Switch known extensions
            switch (extension)
            {
                // String Tables
                case "csv":
                    Print.Export(name);
                    input.Seek(offset - 16, SeekOrigin.Begin);
                    try
                    {
                        StringTable.Decompile(input);
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
                    Print.Export(name);
                    input.Seek(offset - 16, SeekOrigin.Begin);
                    RawFile.Export(input, extension);
                    break;
                // Anim Trees
                case "atr":
                    Print.Export(name);
                    input.Seek(offset - 16, SeekOrigin.Begin);
                    AnimTree.Decompress(input);
                    break;
                // D3DBSP Ent Maps
                case "d3dbsp":
                    Print.Export(name);
                    input.Seek(offset - 56, SeekOrigin.Begin);
                    D3DBSP.Export(input);
                    break;
                // AI BTs
                case "ai_asm":
                    Print.Export(name);
                    input.Seek(offset - 40, SeekOrigin.Begin);
                    AnimStateMachine.Decompile(input);
                    break;
                case "ai_ast":
                    Print.Export(name);
                    break;
                case "ai_am":
                    Print.Export(name);
                    input.Seek(offset - 8, SeekOrigin.Begin);
                    AnimMappingTable.Decompile(input);
                    break;
                case "ai_bt":
                    Print.Export(name);
                    input.Seek(offset - 24, SeekOrigin.Begin);
                    BehaviorTree.Decompile(input);
                    break;
                default:
                    break;
            }
        }
    }
}
