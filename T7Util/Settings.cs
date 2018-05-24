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
using Newtonsoft.Json;

/// <summary>
/// Class to Hold Hydra's Settings
/// </summary>
class Settings
{
    /// <summary>
    /// Export Options
    /// </summary>
    public Dictionary<string, bool> ExportOptions = new Dictionary<string, bool>()
    {
        { "csv",                        true },
        { "script",                     true },
        { "lua",                        true },
        { "gsc",                        true },
        { "csc",                        true },
        { "gsh",                        true },
        { "vision",                     true },
        { "cfg",                        true },
        { "graph",                      true },
        { "txt",                        true },
        { "atr",                        true },
        { "d3dbsp",                     true },
        { "ai_asm",                     true },
        { "ai_ast",                     true },
        { "ai_am",                      true },
        { "ai_bt",                      true },
    };

    /// <summary>
    /// Fast File Options
    /// </summary>
    public Dictionary<string, bool> FastFileOptions = new Dictionary<string, bool>()
    {
        { "DeleteDecodedFile",          true },
    };

    /// <summary>
    /// Current Active Settings
    /// </summary>
    public static Settings ActiveSettings;

    /// <summary>
    /// Loads JSON Settings File
    /// </summary>
    /// <param name="file">File Path</param>
    public static void Load(string file)
    {
        ActiveSettings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(file));
    }

    /// <summary>
    /// Writes Settings to JSON File
    /// </summary>
    /// <param name="file">File Path</param>
    public static void Write(string file)
    {
        ActiveSettings = new Settings();
        if(FileUtil.CanAccessFile(file))
            File.WriteAllText(file, JsonConvert.SerializeObject(ActiveSettings, Formatting.Indented));
    }
}