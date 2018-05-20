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
using CompressionUtil;

namespace Assets
{
    /// <summary>
    /// Anim Tree Processor
    /// </summary>
    class AnimTree
    {
        /// <summary>
        /// Handles Exporting Anim Tree
        /// </summary>
        /// <param name="input"></param>
        public static void Decompress(BinaryReader input)
        {
            int assetSize = input.ReadInt32();

            input.Seek(12, SeekOrigin.Current);

            string assetName = input.ReadCString();

            PathUtil.CreateFilePath("exported_files\\" + assetName);

            input.Seek(4, SeekOrigin.Current);

            byte[] decodedBytes = DeflateUtil.Decode(input.ReadBytes(assetSize - 3)).ToArray();

            File.WriteAllBytes("exported_files\\" + assetName, decodedBytes);

            Print.Info(string.Format("Exported Anim Tree - {0:0.00} KB", decodedBytes.Length / 1024.0));
        }
    }
}
