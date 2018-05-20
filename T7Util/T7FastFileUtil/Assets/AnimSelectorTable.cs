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
using System.Text;
using PhilUtil;
using T7FastFileUtil;

namespace Assets
{
    /// <summary>
    /// Animation Selector Table Class
    /// </summary>
    class AnimSelectorTable
    {
        /// <summary>
        /// Animation Selectors
        /// </summary>
        public AnimationSelector[] Selectors { get; set; }

        /// <summary>
        /// Animation Selector Class
        /// </summary>
        public class AnimationSelector
        {
            /// <summary>
            /// Animation Selector Row
            /// </summary>
            public class Row
            {
                public string[] Columns { get; set; }
            }

            /// <summary>
            /// Animation Selector Name
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Animation Selector Headers (Each selector has its own headers)
            /// </summary>
            public string[] Headers { get; set; }
            
            /// <summary>
            /// Animation Selector Row
            /// </summary>
            public Row[] Rows { get; set; }
        };

        /// <summary>
        /// Reads a float from a compiled Animation Selector Table
        /// </summary>
        /// <param name="fastFile"></param>
        /// <returns></returns>
        public static float ReadFloat(FastFile fastFile)
        {
            fastFile.DecodedStream.Seek(4, SeekOrigin.Current);

            return fastFile.DecodedStream.ReadSingle();
        }

        /// <summary>
        /// Reads an integer from a compiled Animation Selector Table
        /// </summary>
        /// <param name="fastFile"></param>
        /// <returns></returns>
        public static int ReadInt(FastFile fastFile)
        {
            fastFile.DecodedStream.Seek(4, SeekOrigin.Current);

            return fastFile.DecodedStream.ReadInt32();
        }

        /// <summary>
        /// Reads a string from compiled Animation Selector Table
        /// </summary>
        /// <param name="fastFile"></param>
        /// <returns></returns>
        public static string ReadString(FastFile fastFile)
        {
            int stringIndex = fastFile.DecodedStream.ReadInt32();

            fastFile.DecodedStream.Seek(4, SeekOrigin.Current);

            return fastFile.GetString(stringIndex - 1);
        }

        /// <summary>
        /// Decompiles a binary Animation Selector Table
        /// </summary>
        /// <param name="fastFile"></param>
        public static void Decompile(FastFile fastFile)
        {
            AnimSelectorTable selectorTable = new AnimSelectorTable();

            selectorTable.Selectors = new AnimationSelector[fastFile.DecodedStream.ReadInt32()];
            fastFile.DecodedStream.Seek(4, SeekOrigin.Current);


            string assetName = "exported_files\\animtables\\" + fastFile.DecodedStream.ReadCString();
            PathUtil.CreateFilePath(assetName);

            for(int i = 0; i < selectorTable.Selectors.Length; i++)
            {
                selectorTable.Selectors[i] = new AnimationSelector();

                selectorTable.Selectors[i].Name = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);

                fastFile.DecodedStream.Seek(12, SeekOrigin.Current);

                selectorTable.Selectors[i].Headers = new string[fastFile.DecodedStream.ReadInt32()];

                fastFile.DecodedStream.Seek(12, SeekOrigin.Current);

                selectorTable.Selectors[i].Rows = new AnimationSelector.Row[fastFile.DecodedStream.ReadInt32()];

                for (int j = 0; j < selectorTable.Selectors[i].Rows.Length; j++)
                    selectorTable.Selectors[i].Rows[j] = new AnimationSelector.Row();

                fastFile.DecodedStream.Seek(52, SeekOrigin.Current);

            }

            for (int i = 0; i < selectorTable.Selectors.Length; i++)
            {
                for(int j = 0; j < selectorTable.Selectors[i].Headers.Length; j++)
                {
                    selectorTable.Selectors[i].Headers[j] = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);

                    fastFile.DecodedStream.Seek(20, SeekOrigin.Current);

                }

                fastFile.DecodedStream.Seek(16 * selectorTable.Selectors[i].Rows.Length, SeekOrigin.Current);

                int numHeaders = selectorTable.Selectors[i].Headers.Length;
                int numRows    = selectorTable.Selectors[i].Rows.Length;

                for (int j = 0; j < numRows; j++)
                {
                    selectorTable.Selectors[i].Rows[j].Columns = new string[numHeaders];

                    for (int k = 0; k < numHeaders; k++)
                    {
                        // TODO: Figure other headers, this shouldn't result
                        // in an exception as each entry seems to be 8 bytes long anyway
                        switch(selectorTable.Selectors[i].Headers[k])
                        {
                            // Floating Point Numbers
                            case "_locomotion_exit_yaw_min":
                            case "_locomotion_exit_yaw_max":
                            case "_locomotion_turn_yaw_min":
                            case "_locomotion_turn_yaw_max":
                            case "_speed_min":
                            case "_speed_max":
                            case "_blend_in_time":
                            case "_blend_out_time":
                            case "_board_attack_spot":
                                selectorTable.Selectors[i].Rows[j].Columns[k] = ReadFloat(fastFile).ToString();
                                break;
                            case "_which_board_pull":
                                selectorTable.Selectors[i].Rows[j].Columns[k] = ReadInt(fastFile).ToString();
                                break;
                            // Everything else
                            default:
                                selectorTable.Selectors[i].Rows[j].Columns[k] = ReadString(fastFile);
                                break;
                        }
                    }
                }
            }

            // Build new AST String
            StringBuilder output = new StringBuilder();

            for (int i = 0; i < selectorTable.Selectors.Length; i++)
            {
                output.AppendLine(selectorTable.Selectors[i].Name + ",");

                for (int k = 0; k < selectorTable.Selectors[i].Headers.Length; k++)
                {
                    output.Append(selectorTable.Selectors[i].Headers[k] + ",");
                }

                output.AppendLine();

                for (int k = 0; k < selectorTable.Selectors[i].Rows.Length; k++)
                {
                    for(int j = 0; j < selectorTable.Selectors[i].Rows[k].Columns.Length; j++)
                    {
                        output.Append(selectorTable.Selectors[i].Rows[k].Columns[j] + ",");
                    }

                    output.AppendLine();
                }

                output.AppendLine(",");
            }

            // Dump
            File.WriteAllText(assetName, output.ToString());

            Print.Info(string.Format("Decompiled Successfully - Selectors {0}", selectorTable.Selectors.Length));

        }
    }
}
