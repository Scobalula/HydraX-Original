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
using PhilUtil;
using T7FastFileUtil;

namespace Assets
{
    /// <summary>
    /// String Table Processor
    /// </summary>
    public class StringTable
    {
        /// <summary>
        /// Hold String Table Row with Columns
        /// </summary>
        public class StringTableRow
        {
            /// <summary>
            /// Columns of ints pointing to strings
            /// </summary>
            public uint[] Columns { get; set; }

            /// <summary>
            /// Creates a new String Table Row with specified column count.
            /// </summary>
            /// <param name="columns"></param>
            public StringTableRow(int columns)
            {
                Columns = new uint[columns];
            }
        }

        /// <summary>
        /// Decompiles a binary string table from a Black Ops III Fast File
        /// </summary>
        /// <param name="input">Input Reader</param>
        public static void Decompile(BinaryReader input)
        {
            // Read Rows/Colums, Data length will be (rows * cols) * 16
            int columns = input.ReadInt32();
            int rows    = input.ReadInt32();
            // Skip to CSV name
            input.Seek(16, SeekOrigin.Current);
            // Asset Name - Null Terminated
            string name = input.ReadCString();
            // Create File Path
            PathUtil.CreateFilePath("exported_files\\" + name);
            // Create rows
            StringTableRow[] string_rows = new StringTableRow[rows];
            for (int i = 0; i < rows; i++)
                string_rows[i] = new StringTableRow(columns);
            // New String Hash List
            List<uint> hashes = new List<uint>();
            // Add empty string hash if not present
            if(!GlobalStringTable.Strings.ContainsKey(0x1505))
                GlobalStringTable.Strings.Add(0x1505, "");
            // Process Rows/Columns
            for(int i = 0; i < rows; i++)
            {
                for(int j = 0; j < columns; j++)
                {
                    // If a new string hash, this = 0xFFFFFFFFFFFFFFFF
                    bool newString = input.ReadUInt64() == ulong.MaxValue;
                    // Read DJB String Hash
                    uint stringHash = input.ReadUInt32();
                    // ???
                    uint unknown = input.ReadUInt32();
                    // Add to Row/Col
                    string_rows[i].Columns[j] = stringHash;
                    // If new string, add hash to list
                    if (newString)
                        hashes.Add(stringHash);
                }
            }
            // Process New Strings
            for (int i = 0; i < hashes.Count; i++)
                GlobalStringTable.Strings[hashes[i]] = input.ReadCString();
            // Dump String Table
            using (StreamWriter output = new StreamWriter("exported_files\\" + name))
            {
                foreach (var row in string_rows)
                {
                    foreach (var column in row.Columns)
                    {
                        output.Write("{0},", GlobalStringTable.Strings.ContainsKey(column) ? GlobalStringTable.Strings[column] : "" );
                    }
                    output.WriteLine();
                }
            }
            // Info
            Print.Info(string.Format("Decompiled Successfully - Rows {0} - Columns {1}", rows, columns));
        }
    }
}
