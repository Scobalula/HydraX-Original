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
using System.IO;
using System.Collections.Generic;
using System.Text;
using CompressionUtil;
using PhilUtil;

namespace T7FastFileUtil
{
    /// <summary>
    /// Handles String Cache/Global String Table
    /// </summary>
    class GlobalStringTable
    {
        /// <summary>
        /// Hashes/Strings
        /// </summary>
        public static Dictionary<uint, string> Strings = new Dictionary<uint, string>();

        /// <summary>
        /// Loads StringCache
        /// </summary>
        public static void LoadStringCache()
        {
            // Check if we can access it
            if (FileUtil.CanAccessFile("StringCache.dat"))
            {
                // Decode it (Compressed)
                using (MemoryStream input = DeflateUtil.Decode(File.ReadAllBytes("StringCache.dat")))
                {
                    using (var stringCache = new BinaryReader(input))
                    {
                        if (stringCache.ReadUInt64() != 0x4548434143525453)
                        {
                            //
                        }
                        // String Count
                        int num_strings = stringCache.ReadInt32();
                        // Parse Hashes/Strings
                        for (int i = 0; i < num_strings; i++)
                        {
                            var hash = stringCache.ReadUInt32();
                            var str = stringCache.ReadCString();
                            Strings[hash] = str;
                        }
                    }
                }
                // Info
                Print.Info(string.Format("Loaded {0} Strings from String Cache successfully.", Strings.Count));
            }
            else
            {
                // fk
                Print.Error("Cannot access StringCache.dat. File doesn't exist or Permissions Denied.");
            }

            Print.Info();
        }

        /// <summary>
        /// Writes String Hashes/Strings to file
        /// </summary>
        public static void WriteStringCache()
        {
            using (MemoryStream output = new MemoryStream())
            {
                using (var stringCache = new BinaryWriter(output))
                {
                    // Write Header
                    stringCache.Write(Encoding.ASCII.GetBytes("STRCACHE"));
                    // Write String Count
                    stringCache.Write(Strings.Count);
                    // Write Hashes/Strings
                    foreach (KeyValuePair<uint, string> kvp in Strings)
                    {
                        stringCache.Write(kvp.Key);
                        stringCache.Write(Encoding.ASCII.GetBytes(kvp.Value));
                        stringCache.Write((byte)0);

                    }
                }
                // Encode/Dump to File
                File.WriteAllBytes( "StringCache.dat", DeflateUtil.Encode(output.ToArray()).ToArray());
                // Infos
                Print.Info(string.Format("Wrote {0} Strings to String Cache successfully.", Strings.Count));
                Print.Info();
            }
        }
    }
}
