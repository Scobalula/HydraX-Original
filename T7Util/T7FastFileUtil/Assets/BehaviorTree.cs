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
using System.Collections.Generic;
using PhilUtil;
using System.IO;
using T7FastFileUtil;
using Newtonsoft.Json;

namespace Assets
{
    class BehaviorTree
    {
        public class Behavior
        {
            private static Dictionary<int, string> Types = new Dictionary<int, string>()
            {
                { 0 , "action" },
                { 2 , "condition_script" },
                { 3 , "condition_script_negate" },
                { 4 , "condition_service_script" },
                { 8 , "parallel" },
                { 9 , "sequence" },
                { 10 , "selector" },
                { 13 , "link_node" },
            };

            public string type { get; set; }
            public string id { get; set; }

            [JsonIgnore]
            public int Index { get; set; }

            [JsonIgnore]
            public int ParentIndex { get; set; }

            ///Specific to Action Behaviors
            public string ActionName { get; set; }
            public string ASMStateName { get; set; }
            public string actionNotify { get; set; }
            public string StartFunction { get; set; }
            public string TerminateFunction { get; set; }
            public string UpdateFunction { get; set; }
            public int? loopingAction { get; set; }
            public int? actionTimeMax { get; set; }

            public string scriptFunction { get; set; }
            public string interruptName { get; set; }

            public int? cooldownMin { get; set; }
            public int? cooldownMax { get; set; }

            [JsonProperty(Order = 1)]
            public List<Behavior> children;

            public void Save(string path)
            {
                using (JsonTextWriter output = new JsonTextWriter(new StreamWriter(path)))
                {
                    output.Formatting = Formatting.Indented;
                    output.Indentation = 4;
                    output.IndentChar = ' ';

                    JsonSerializer serializer = new JsonSerializer();

                    serializer.NullValueHandling = NullValueHandling.Ignore;

                    serializer.Serialize(output, this);

                }
            }

            public static string GetBehaviorType(int typeIndex)
            {
                if (Types.ContainsKey(typeIndex))
                    return Types[typeIndex];

                return String.Format("BT_UNKNOWN_TYPE_INDEX: {0}", typeIndex);
            }
        }

        /// <summary>
        /// Process a Behavior and all child Behaviors
        /// </summary>
        /// <param name="fastFile"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static Behavior ProcessBehavior(FastFile fastFile, Behavior parent = null)
        {
            // New Behavior
            Behavior behavior           = new Behavior();

            parent?.children.Add(behavior);
            behavior.id                 = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
            behavior.type               = Behavior.GetBehaviorType(fastFile.DecodedStream.ReadInt32());

            behavior.Index              = fastFile.DecodedStream.ReadInt32();
            behavior.ParentIndex        = fastFile.DecodedStream.ReadInt32();

            bool hasChildren            = fastFile.DecodedStream.ReadInt64() == -1;
            int numChildBehaviors       = fastFile.DecodedStream.ReadInt32();
            // Grab all of these as they can be used differently depending on type
            // Byte 28
            string behaviorString0      = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
            string behaviorString1      = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
            string behaviorString2      = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
            string behaviorString3      = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
            string behaviorString4      = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
            string behaviorString5      = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
            string behaviorString6      = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
            string behaviorString7      = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
            string behaviorString8      = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);


            int integer1                = fastFile.DecodedStream.ReadInt32();
            int integer2                = fastFile.DecodedStream.ReadInt32();

            // Print.Debug(String.Format("Behavior {0} - Position 0x{1:X}", behavior.id, fastFile.DecodedStream.BaseStream.Position));

            // Switch Type, as what our strings/ints do depends on type
            switch (behavior.type)
            {
                // Action Behaviors
                case "action":
                    behavior.ASMStateName         = behaviorString3;
                    behavior.ActionName           = behaviorString4;
                    behavior.actionNotify         = behaviorString5;
                    behavior.StartFunction        = behaviorString6;
                    behavior.UpdateFunction       = behaviorString7;
                    behavior.TerminateFunction    = behaviorString8;
                    behavior.loopingAction        = integer1;
                    behavior.actionTimeMax        = integer2;
                    break;
                // Condition Script Behaviors
                case "condition_script":
                case "condition_script_negate":
                case "condition_service_script":
                    behavior.scriptFunction     = behaviorString6;
                    behavior.interruptName      = behaviorString7;
                    behavior.cooldownMin        = integer1;
                    behavior.cooldownMax        = integer2;
                    break;
                default:
                    break;
            }
            // If we have children, create the list
            if(hasChildren)
                behavior.children = new List<Behavior>();
            // Process Child Nodes
            for (int i = 0; i < numChildBehaviors; i++)
                ProcessBehavior(fastFile, behavior);
            // Return to root
            return behavior;
        }


        public static void Decompile(FastFile fastFile)
        {
            // Number of Behaviors
            int numBehaviors = fastFile.DecodedStream.ReadInt32();
            // ??? sometimes has a value
            fastFile.DecodedStream.Seek(20, SeekOrigin.Current);
            // Get BT Name
            string assetName = "exported_files\\behavior\\" + fastFile.DecodedStream.ReadCString();
            // Create File Paths
            PathUtil.CreateFilePath(assetName);
            // Process root and nested behaviors
            Behavior root = ProcessBehavior(fastFile);
            // Save
            root.Save(assetName);

            Print.Info(String.Format("Decompiled Successfully - Total Behaviors {0}", numBehaviors));
        }

    }
}
