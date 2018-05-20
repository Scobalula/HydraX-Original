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
using PhilUtil;
using T7FastFileUtil;

namespace Assets
{
    /// <summary>
    /// Animation Mapping Table
    /// </summary>
    class AnimMappingTable
    {
        /// <summary>
        /// Animation Mapping Table Rot
        /// </summary>
        public class AnimationMap
        {
            /// <summary>
            /// Name of Map
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Entries within this Map
            /// </summary>
            public string[] Entries { get; set; }

            /// <summary>
            /// Creates a new Animation Mapping Entry
            /// </summary>
            /// <param name="name">Entry Name</param>
            /// <param name="entryCount">Entry Count</param>
            public AnimationMap(string name, int entryCount)
            {
                Name = name;
                Entries = new string[entryCount];
            }
        }

        /// <summary>
        /// Decompiles an Anim Mapping Table from a Fast File
        /// </summary>
        /// <param name="fastFile">Fast File</param>
        public static void Decompile(FastFile fastFile)
        {   
            // Number of rows, including void,void footer
            int numRows = fastFile.DecodedStream.ReadInt32();
            fastFile.DecodedStream.ReadInt32();

            AnimationMap[] rows = new AnimationMap[numRows];

            string assetName = "exported_files\\animtables\\" + fastFile.DecodedStream.ReadCString();

            // Process Rows
            for (int i = 0; i < numRows; i++)
            {
                int stringIndex = fastFile.DecodedStream.ReadInt32();
                fastFile.DecodedStream.ReadInt32();

                long separator = fastFile.DecodedStream.ReadInt64();
                int numEntries = fastFile.DecodedStream.ReadInt32();
                fastFile.DecodedStream.ReadInt32();

                rows[i] = new AnimationMap(fastFile.GetString(stringIndex - 1), numEntries);
            }

            // Process Entries
            for (int i = 0; i < numRows; i++)
                for (int j = 0; j < rows[i].Entries.Length; j++)
                    rows[i].Entries[j] = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);

            PathUtil.CreateFilePath(assetName);

            using (StreamWriter output = new StreamWriter(assetName))
            {
                output.WriteLine("#");
                for (int i = 0; i < numRows; i++)
                {
                    output.Write(rows[i].Name);

                    for (int j = 0; j < rows[i].Entries.Length; j++)
                    {
                        output.Write(",{0}", rows[i].Entries[j]);
                    }

                    output.WriteLine();
                }
            }

            Print.Info(string.Format("Decompiled Successfully - Entries {0}", numRows));
        }
    }
}
