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
using System.ComponentModel;
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
        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int SubStateCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonIgnore]
        public int TransitionCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("animation_selector")]
        public string AnimationSelector { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("transition_decorator")]
        public string TransitionDecorator { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("aim_selector")]
        public string AimSelector { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("shoot_selector")]
        public string ShootSelector { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("delta_layer_function")]
        public string DeltaLayerFunction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("transdec_layer_function")]
        public string TransDecLayerFunction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("asm_client_notify")]
        public string ASMClientNotify { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("loopsync")]
        [DefaultValue(false)]
        public bool LoopSync { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("cleanloop")]
        [DefaultValue(false)]
        public bool CleanLoop { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("multipledelta")]
        [DefaultValue(false)]
        public bool MultipleDelta { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("terminal")]
        [DefaultValue(false)]
        public bool Terminal { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("parametric2d")]
        [DefaultValue(false)]
        public bool Parametric2D { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("animdrivenlocomotion")]
        [DefaultValue(false)]
        public bool AnimDrivenLocmotion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("coderate")]
        [DefaultValue(false)]
        public bool Coderate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("speedblend")]
        [DefaultValue(false)]
        public bool SpeedBlend { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("allow_transdec_aim")]
        [DefaultValue(false)]
        public bool AllowTransDecAim { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("force_fire")]
        [DefaultValue(false)]
        public bool ForceFire { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("requires_ragdoll_notetrack")]
        [DefaultValue(false)]
        public bool RequiresRagdollNote { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(Order = 1)]
        public Dictionary<string, AnimState> substates;

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty(Order = 2)]
        public Dictionary<string, Transition> transitions;
    }

    class AnimStateMachine
    {
        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("states")]
        public Dictionary<string, AnimState> States = new Dictionary<string, AnimState>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool RequiresRagdollNote(int values)
        {
            return (values & 0x2) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsTerminal(int values)
        {
            return (values & 0x1) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsLoopSync(int values)
        {
            return (values & 0x2) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsCleanLoop(int values)
        {
            return (values & 0x80) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsMultipleDelta(int values)
        {
            return (values & 0x4) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsParametric2D(int values)
        {
            return (values & 0x8) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsAnimDrivenLocomotion(int values)
        {
            return (values & 0x100) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsCoderate(int values)
        {
            return (values & 0x10) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsSpeedBlend(int values)
        {
            return (values & 0x200) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool AllowTransdecAim(int values)
        {
            return (values & 0x20) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool ForceFire(int values)
        {
            return (values & 0x40) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static void Decompile(FastFile fastFile)
        {
            // Total Transitions from all states - Max 127?
            int totalTransitions = 0;

            var aiAsm = new AnimStateMachine();

            // Main State count - Max 24?
            int numMainStates = fastFile.DecodedStream.ReadInt32();
            fastFile.DecodedStream.ReadInt32();
            fastFile.DecodedStream.Seek(8, SeekOrigin.Current);

            // Total States - Max 127?
            int numStates = fastFile.DecodedStream.ReadInt32();
            fastFile.DecodedStream.ReadInt32();
            fastFile.DecodedStream.Seek(16, SeekOrigin.Current);


            string[] mainStates = new string[numMainStates];

            string assetName = "exported_files\\animstatemachines\\" + fastFile.DecodedStream.ReadCString();
            PathUtil.CreateFilePath(assetName);


            for(int i = 0; i < numMainStates; i++)
            {
                // Name from Fast File String Table
                string stateName = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);

                // ???
                fastFile.DecodedStream.ReadInt32();
                fastFile.DecodedStream.ReadInt64();
                
                // Sub states this state has
                int numSubStates    = fastFile.DecodedStream.ReadInt32();

                // ???
                fastFile.DecodedStream.ReadInt32();

                aiAsm.States[stateName] = new AnimState();
                aiAsm.States[stateName].SubStateCount = numSubStates;
                mainStates[i] = stateName;

            }

            // Indexes for in-game possibly?
            fastFile.DecodedStream.Seek(4 * numStates, SeekOrigin.Current);

            for (int i = 0; i < numMainStates; i++)
            {
                AnimState state = aiAsm.States[mainStates[i]];

                state.substates = new Dictionary<string, AnimState>();

                for(int j = 0; j < state.SubStateCount; j++)
                {

                    string stateName = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);

                    state.substates[stateName] = new AnimState();

                    fastFile.DecodedStream.ReadInt32();
                    int boolValues1 = fastFile.DecodedStream.ReadInt32();
                    int boolValues2 = fastFile.DecodedStream.ReadInt32();

                    fastFile.DecodedStream.ReadInt32();

                    state.substates[stateName].AnimationSelector        = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
                    state.substates[stateName].AimSelector              = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
                    state.substates[stateName].ShootSelector            = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
                    state.substates[stateName].TransitionDecorator      = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
                    state.substates[stateName].DeltaLayerFunction       = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
                    state.substates[stateName].TransitionDecorator      = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);
                    state.substates[stateName].ASMClientNotify          = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);

                    // Transitions are stored at end of ASM, assuming in order of each state
                    bool hasTransitions      = fastFile.DecodedStream.ReadUInt64() == ulong.MaxValue;
                    int transitionCount      = fastFile.DecodedStream.ReadInt32();

                    fastFile.DecodedStream.ReadInt32();

                    totalTransitions += transitionCount;

                    if (hasTransitions)
                        state.substates[stateName].transitions              = new Dictionary<string, Transition>();

                    state.substates[stateName].RequiresRagdollNote          = RequiresRagdollNote(boolValues1);
                    state.substates[stateName].Terminal                     = IsTerminal(boolValues2);
                    state.substates[stateName].LoopSync                     = IsLoopSync(boolValues2);
                    state.substates[stateName].CleanLoop                    = IsCleanLoop(boolValues2);
                    state.substates[stateName].MultipleDelta                = IsMultipleDelta(boolValues2);
                    state.substates[stateName].Parametric2D                 = IsParametric2D(boolValues2);
                    state.substates[stateName].AnimDrivenLocmotion          = IsAnimDrivenLocomotion(boolValues2);
                    state.substates[stateName].Coderate                     = IsCoderate(boolValues2);
                    state.substates[stateName].SpeedBlend                   = IsSpeedBlend(boolValues2);
                    state.substates[stateName].AllowTransDecAim             = AllowTransdecAim(boolValues2);
                    state.substates[stateName].ForceFire                    = ForceFire(boolValues2);

                    state.substates[stateName].TransitionCount              = transitionCount;
                }
            }
            // Like with states, these might be indexes?
            fastFile.DecodedStream.Seek(totalTransitions * 4, SeekOrigin.Current);
            // Process Transitions
            foreach (KeyValuePair<string, AnimState> states in aiAsm.States)
            {
                foreach (KeyValuePair<string, AnimState> subStates in states.Value.substates)
                {
                    for (int i = 0; i < subStates.Value.TransitionCount; i++)
                    {
                        string transitionName = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);

                        subStates.Value.transitions[transitionName] = new Transition();

                        /*
                         * Rest of these values may just point
                         * parent, etc.
                         */
                        fastFile.DecodedStream.Seek(24, SeekOrigin.Current);

                        subStates.Value.transitions[transitionName].animation_selector = fastFile.GetString(fastFile.DecodedStream.ReadInt32() - 1);

                        // Seek to next Transition
                        fastFile.DecodedStream.Seek(16, SeekOrigin.Current);
                    }
                }
            }
            // Save AI_ASM
            aiAsm.Save(assetName);

            Print.Info(string.Format("Decompiled Successfully - States {0} - Transitions {1}", numStates, totalTransitions));


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            if (FileUtil.CanAccessFile(path))
            {
                using (JsonTextWriter output = new JsonTextWriter(new StreamWriter(path)))
                {
                    output.Formatting = Formatting.Indented;
                    output.Indentation = 4;
                    output.IndentChar = ' ';

                    JsonSerializer serializer = new JsonSerializer();

                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    serializer.DefaultValueHandling = DefaultValueHandling.Ignore;

                    serializer.Serialize(output, this);

                }
            }
        }
    }
}
