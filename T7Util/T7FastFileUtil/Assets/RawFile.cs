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
using PhilUtil;

namespace Assets
{
    /// <summary>
    /// RAW File Processor
    /// </summary>
    class RawFile
    {
        /// <summary>
        /// Handles Exporting RAW Assets
        /// </summary>
        /// <param name="input">Binary Reader</param>
        /// <param name="extension"></param>
        public static void Export(BinaryReader input, string extension)
        {
            // Asset Size
            int assetSize = input.ReadInt32();
            // Seek to name of xAsset
            input.Seek(12, SeekOrigin.Current);
            // Replace with "c" for including with L3AKMod
            string assetName = input.ReadCString().
                Replace(".gsc", ".gscc").
                Replace(".csc", ".cscc").
                Replace(".gsh", ".gshc").
                Replace(".lua", ".luac");
            // Create File Path
            PathUtil.CreateFilePath("exported_files\\" + assetName);
            // Dump Bytes to File
            File.WriteAllBytes("exported_files\\" + assetName, input.ReadBytes(assetSize));
            // Info
            Print.Info(String.Format("Exported {0} File Successfully - {1:0.00} KB", extension.ToUpper(), assetSize / 1024.0));
        }
    }
}
