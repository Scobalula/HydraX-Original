using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PhilUtil;
using T7FastFileUtil;
using Newtonsoft.Json;

namespace Assets
{
    class Transition
    {
        public string animation_selector;
    }

    class AnimState
    {
        [JsonIgnore]
        public int SubStateCount { get; set; }

        [JsonIgnore]
        public int TransitionCount { get; set; }

        public string animation_selector { get; set; }
        public string transition_decorator { get; set; }
        public string aim_selector { get; set; }
        public string shoot_selector { get; set; }
        public string delta_layer_function { get; set; }
        public string transdec_layer_function { get; set; }
        public string asm_client_notify { get; set; }

        public bool? loopsync { get; set; }
        public bool? cleanloop { get; set; }
        public bool? multipledelta { get; set; }
        public bool? terminal { get; set; }
        public bool? parametric2d { get; set; }
        public bool? animdrivenlocomotion { get; set; }
        public bool? coderate { get; set; }
        public bool? speedblend { get; set; }
        public bool? allow_transdec_aim { get; set; }
        public bool? force_fire { get; set; }
        public bool? requires_ragdoll_notetrack { get; set; }

        [JsonProperty(Order = 1)]
        public Dictionary<string, AnimState> substates;

        [JsonProperty(Order = 2)]
        public Dictionary<string, Transition> transitions;
    }

    class AnimStateMachine
    {
        public Dictionary<string, AnimState> states = new Dictionary<string, AnimState>();

        public static void Decompile(FastFile fastFile)
        {
            // Total num of transitions from all Sub States
            int totalTransitions = 0;
            // New AnimStateMachine
            var aiAsm = new AnimStateMachine();
            // Number of Main States
            int numMainStates = fastFile.DecodedStream.ReadInt32();
            // ??? Always 0
            fastFile.DecodedStream.ReadInt32();
            // Skip
            fastFile.DecodedStream.Seek(8, SeekOrigin.Current);
            // Total Number of States
            int numStates = fastFile.DecodedStream.ReadInt32();
            // ??? Always 0
            fastFile.DecodedStream.ReadInt32();
            // Skip
            fastFile.DecodedStream.Seek(16, SeekOrigin.Current);
            // Main State Names
            string[] mainStates = new string[numMainStates];
            // Read name
            string assetName = "exported_files\\animstatemachines\\" + fastFile.DecodedStream.ReadCString();
            // Create Directories
            PathUtil.CreateFilePath(assetName);
            // Process "main" states
            for(int i = 0; i < numMainStates; i++)
            {
                // String Index 
                int stringIndex = fastFile.DecodedStream.ReadInt32();
                // ??? Always 0
                fastFile.DecodedStream.ReadInt32();
                // Separator
                fastFile.DecodedStream.ReadUInt64();
                // Number of Sub States
                int numSubStates = fastFile.DecodedStream.ReadInt32();
                // ??? Always 0
                fastFile.DecodedStream.ReadInt32();
                // State Name
                string stateName = fastFile.GetString(stringIndex - 1);
                // Create states
                aiAsm.states[stateName] = new AnimState();
                // Set Count
                aiAsm.states[stateName].SubStateCount = numSubStates;
                // Add to main states
                mainStates[i] = stateName;

            }
            // Indexes for in-game possibly?
            input.Seek(4 * numStates, SeekOrigin.Current);
            // Process Sub States
            for (int i = 0; i < numMainStates; i++)
            {
                AnimState state = aiAsm.states[mainStates[i]];

                state.substates = new Dictionary<string, AnimState>();

                for(int j = 0; j < state.SubStateCount; j++)
                {

                    string stateName = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
                    // ???
                    fastFile.DecodedStream.ReadInt32();
                    bool reqRagDogg = (fastFile.DecodedStream.ReadInt32() & 2) != 0;
                    int boolValues = fastFile.DecodedStream.ReadInt32();
                    // ???
                    fastFile.DecodedStream.ReadInt32();
                    // String Indexes
                    state.substates[stateName].animation_selector      = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
                    state.substates[stateName].aim_selector            = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
                    state.substates[stateName].shoot_selector          = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
                    state.substates[stateName].transition_decorator    = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
                    state.substates[stateName].delta_layer_function    = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
                    state.substates[stateName].transdec_layer_function = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
                    state.substates[stateName].asm_client_notify       = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);

                    bool hasTransitions      = fastFile.DecodedStream.ReadUInt64() == ulong.MaxValue;
                    int transitionCount      = fastFile.DecodedStream.ReadInt32();
                    // ???
                    fastFile.DecodedStream.ReadInt32();

                    totalTransitions += transitionCount;

                    state.substates[stateName] = new AnimState();

                    if (hasTransitions)
                        state.substates[stateName].transitions              = new Dictionary<string, Transition>();

                    if (reqRagDogg)
                        state.substates[stateName].requires_ragdoll_notetrack   = true;
                    if ((boolValues & 0x1) != 0)
                        state.substates[stateName].terminal                     = true;
                    if ((boolValues & 0x2)      != 0)
                        state.substates[stateName].loopsync                     = true;
                    if ((boolValues & 0x80)     != 0)
                        state.substates[stateName].cleanloop                    = true;
                    if ((boolValues & 0x4)      != 0)
                        state.substates[stateName].multipledelta                = true;
                    if ((boolValues & 0x8)      != 0)
                        state.substates[stateName].parametric2d                 = true;
                    if ((boolValues & 0x100)    != 0)
                        state.substates[stateName].animdrivenlocomotion         = true;
                    if ((boolValues & 0x10)     != 0)
                        state.substates[stateName].coderate                     = true;
                    if ((boolValues & 0x200)    != 0)
                        state.substates[stateName].speedblend                   = true;
                    if ((boolValues & 0x20)     != 0)
                        state.substates[stateName].allow_transdec_aim           = true;
                    if ((boolValues & 0x40)     != 0)
                        state.substates[stateName].force_fire                   = true;

                    state.substates[stateName].TransitionCount = transitionCount;
                }
            }
            // Like with states, these might be indexes?
            input.Seek(totalTransitions * 4, SeekOrigin.Current);
            // Process Transitions
            foreach (KeyValuePair<string, AnimState> states in aiAsm.states)
            {
                foreach (KeyValuePair<string, AnimState> subStates in states.Value.substates)
                {
                    for (int i = 0; i < subStates.Value.TransitionCount; i++)
                    {
                        // Index of this Transitions Name
                        int nameIndex = input.ReadInt32();
                        /*
                         * Rest of these values may just point
                         * parent, etc.
                         */
                        input.Seek(24, SeekOrigin.Current);
                        // Anim Selector (seems to be its only property?)
                        int animSelectorIndex = input.ReadInt32();
                        // Name of Transition
                        string transitionName = T7FastFile.Strings[nameIndex - 1];
                        // Name of selector
                        string animSelectorName = T7FastFile.Strings[animSelectorIndex - 1];
                        // Create Data
                        subStates.Value.transitions[transitionName] = new Transition();
                        subStates.Value.transitions[transitionName].animation_selector = animSelectorName;
                        // Seek to next Transition
                        input.Seek(16, SeekOrigin.Current);
                    }
                }
            }
            // Save AI_ASM
            aiAsm.Save(assetName);

            Print.Info(string.Format("Decompiled Successfully - States {0} - Transitions {1}", numStates, totalTransitions));


        }

        public void Save(string path)
        {
            if (FileUtil.CanAccessFile(path))
                File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,

                }));
        }
    }
}
