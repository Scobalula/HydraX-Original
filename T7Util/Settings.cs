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
    /// Exportable Assets
    /// </summary>
    public Dictionary<string, bool> Export = new Dictionary<string, bool>();

    /// <summary>
    /// Export .Script
    /// </summary>
    public bool ExportScript = true;

    /// <summary>
    /// Export .LUA
    /// </summary>
    public bool ExportLUA = true;

    /// <summary>
    /// Export .GSC
    /// </summary>
    public bool ExportGSC = true;

    /// <summary>
    /// Export .CSC
    /// </summary>
    public bool ExportCSC = true;

    /// <summary>
    /// Export .GSH
    /// </summary>
    public bool ExportGSH = true;

    /// <summary>
    /// Export .Vision
    /// </summary>
    public bool ExportVision = true;

    /// <summary>
    /// Export .CFG
    /// </summary>
    public bool ExportCFG = true;

    /// <summary>
    /// Export .Graph
    /// </summary>
    public bool ExportGraph = true;

    /// <summary>
    /// Export .TXT
    /// </summary>
    public bool ExportTXT = true;

    /// <summary>
    /// Export .CSV
    /// </summary>
    public bool ExportStringTables = true;

    /// <summary>
    /// Export .ATR
    /// </summary>
    public bool ExportAnimTrees = true;

    /// <summary>
    /// Export .D3DBSP Ents
    /// </summary>
    public bool ExportEntMap = true;

    /// <summary>
    /// Export AI_AM files
    /// </summary>
    public bool ExportAIAnimMappingTableFiles = true;

    /// <summary>
    /// Export AI_ASM files
    /// </summary>
    public bool ExportAIAnimStateMachineFiles = true;

    /// <summary>
    /// Export ONLY .CSV
    /// </summary>
    public bool ExportStringTablesOnly = false;

    /// <summary>
    /// Delete .decoded.dat file.
    /// </summary>
    public bool DeleteDecodedFastFile = true;


    /// <summary>
    /// Active Settings for this run
    /// </summary>
    public static Settings ActiveSettings { get; set; }

    /// <summary>
    /// Loads JSON Settings File
    /// </summary>
    /// <param name="file">File Path</param>
    public static void Load(string file)
    {
        ActiveSettings = JsonConvert.DeserializeObject<Settings>(File.ReadAllText(file));
        // Build Exportable Dictionary
        ActiveSettings.Export["csv"]    = ActiveSettings.ExportStringTablesOnly ? true  : ActiveSettings.ExportStringTables;
        ActiveSettings.Export["script"] = ActiveSettings.ExportStringTablesOnly ? false : ActiveSettings.ExportScript;
        ActiveSettings.Export["lua"]    = ActiveSettings.ExportStringTablesOnly ? false : ActiveSettings.ExportLUA;
        ActiveSettings.Export["gsc"]    = ActiveSettings.ExportStringTablesOnly ? false : ActiveSettings.ExportGSC;
        ActiveSettings.Export["csc"]    = ActiveSettings.ExportStringTablesOnly ? false : ActiveSettings.ExportCSC;
        ActiveSettings.Export["gsh"]    = ActiveSettings.ExportStringTablesOnly ? false : ActiveSettings.ExportGSH;
        ActiveSettings.Export["vision"] = ActiveSettings.ExportStringTablesOnly ? false : ActiveSettings.ExportVision;
        ActiveSettings.Export["cfg"]    = ActiveSettings.ExportStringTablesOnly ? false : ActiveSettings.ExportCFG;
        ActiveSettings.Export["graph"]  = ActiveSettings.ExportStringTablesOnly ? false : ActiveSettings.ExportGraph;
        ActiveSettings.Export["txt"]    = ActiveSettings.ExportStringTablesOnly ? false : ActiveSettings.ExportTXT;
        ActiveSettings.Export["atr"]    = ActiveSettings.ExportStringTablesOnly ? false : ActiveSettings.ExportAnimTrees;
        ActiveSettings.Export["d3dbsp"] = ActiveSettings.ExportStringTablesOnly ? false : ActiveSettings.ExportEntMap;
        ActiveSettings.Export["ai_am"]  = ActiveSettings.ExportStringTablesOnly ? false : ActiveSettings.ExportAIAnimMappingTableFiles;
        ActiveSettings.Export["ai_ast"] = false;
        ActiveSettings.Export["ai_asm"] = ActiveSettings.ExportStringTablesOnly ? false : ActiveSettings.ExportAIAnimStateMachineFiles;
    }

    /// <summary>
    /// Writes Settings to JSON File
    /// </summary>
    /// <param name="file">File Path</param>
    public static void Write(string file)
    {
        if(FileUtil.CanAccessFile(file))
            File.WriteAllText(file, JsonConvert.SerializeObject(ActiveSettings, Formatting.Indented));
    }
}