using System.IO;
using PhilUtil;

namespace Assets
{
    /// <summary>
    /// D3DBSP Processor
    /// </summary>
    class D3DBSP
    {
        /// <summary>
        /// Handles Exporting D3DBSP Entity Maps
        /// </summary>
        /// <param name="input">Reader Input</param>
        public static void Export(BinaryReader input)
        {
            // Ent Map Size
            int entMapSize = input.ReadInt32();
            // Skip to D3DBSP Name
            input.Seek(52, SeekOrigin.Current);
            // 
            string assetName = input.ReadCString();
            // Create File Path
            PathUtil.CreateFilePath("exported_files\\" + assetName);
            // Dump Bytes
            File.WriteAllBytes("exported_files\\" + Path.ChangeExtension(assetName, null) + ".map", input.ReadBytes(entMapSize - 1));
            // Info
            Print.Info(string.Format("Exported Ent Map - {0:0.00} KB", entMapSize / 1024.0));

        }
    }
}
