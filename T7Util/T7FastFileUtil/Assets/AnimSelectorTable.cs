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
using System.Collections.Generic;
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
        /// AST Definitions, from AST Definitions JSON File included in the Mod Tools
        /// </summary>
        public static Dictionary<string, Func<FastFile, string>> ASTDefinitions = new Dictionary<string, Func<FastFile, string>>()
        {
            { "_context",                                     ReadString },
            { "_context2",                                    ReadString },
            { "_cover_concealed",                             ReadEnum },
            { "_cover_direction",                             ReadEnum },
            { "_previous_cover_direction",                    ReadEnum },
            { "_desired_cover_direction",                     ReadEnum },
            { "_cover_mode",                                  ReadEnum },
            { "_previous_cover_mode",                         ReadEnum },
            { "_cover_type",                                  ReadEnum },
            { "_current_location_cover_type",                 ReadEnum },
            { "_previous_cover_type",                         ReadEnum },
            { "_exposed_type",                                ReadEnum },
            { "_stance",                                      ReadEnum },
            { "_desired_stance",                              ReadEnum },
            { "_arrival_stance",                              ReadEnum },
            { "_locomotion_should_turn",                      ReadEnum },
            { "_arrival_type",                                ReadEnum },
            { "_locomotion_speed",                            ReadEnum },
            { "_grapple_direction",                           ReadEnum },
            { "_run_n_gun_variation",                         ReadEnum },
            { "_has_legs",                                    ReadEnum },
            { "_idgun_damage_direction",                      ReadEnum },
            { "_which_board_pull",                            ReadInt },
            { "_variant_type",                                ReadInt },
            { "_low_gravity_variant",                         ReadInt },
            { "_board_attack_spot",                           ReadFloat },
            { "_weapon_class",                                ReadEnum },
            { "_weapon_type",                                 ReadEnum },
            { "_damage_weapon",                               ReadEnum },
            { "_damage_direction",                            ReadEnum },
            { "_damage_location",                             ReadEnum },
            { "_fatal_damage_location",                       ReadEnum },
            { "_damage_taken",                                ReadEnum },
            { "_damage_weapon_class",                         ReadEnum },
            { "_tracking_turn_yaw_min",                       ReadFloat },
            { "_tracking_turn_yaw_max",                       ReadFloat },
            { "_melee_distance_min",                          ReadFloat },
            { "_melee_distance_max",                          ReadFloat },
            { "_throw_distance_min",                          ReadFloat },
            { "_throw_distance_max",                          ReadFloat },
            { "_locomotion_exit_yaw_min",                     ReadFloat },
            { "_locomotion_exit_yaw_max",                     ReadFloat },
            { "_locomotion_motion_angle_min",                 ReadFloat },
            { "_locomotion_motion_angle_max",                 ReadFloat },
            { "_lookahead_angle_min",                         ReadFloat },
            { "_lookahead_angle_max",                         ReadFloat },
            { "_locomotion_turn_yaw_min",                     ReadFloat },
            { "_locomotion_turn_yaw_max",                     ReadFloat },
            { "_locomotion_arrival_yaw_min",                  ReadFloat },
            { "_locomotion_arrival_yaw_max",                  ReadFloat },
            { "_tactical_arrival_facing_yaw_min",             ReadFloat },
            { "_tactical_arrival_facing_yaw_max",             ReadFloat },
            { "_locomotion_arrival_distance_min",             ReadFloat },
            { "_locomotion_arrival_distance_max",             ReadFloat },
            { "_enemy_yaw_min",                               ReadFloat },
            { "_enemy_yaw_max",                               ReadFloat },
            { "_perfect_enemy_yaw_min",                       ReadFloat },
            { "_perfect_enemy_yaw_max",                       ReadFloat },
            { "_react_yaw_min",                               ReadFloat },
            { "_react_yaw_max",                               ReadFloat },
            { "_speed_min",                                   ReadFloat },
            { "_speed_max",                                   ReadFloat },
            { "_locomotion_face_enemy_quadrant",              ReadEnum },
            { "_locomotion_face_enemy_quadrant_previous",     ReadEnum },
            { "_traversal_type",                              ReadString },
            { "_locomotion_pain_type",                        ReadString },
            { "_human_locomotion_movement_type",              ReadEnum },
            { "_animation_alias",                             ReadString },
            { "_aim_up_alias",                                ReadString },
            { "_aim_down_alias",                              ReadString },
            { "_aim_left_alias",                              ReadString },
            { "_aim_right_alias",                             ReadString },
            { "_animation_alias_semi",                        ReadString },
            { "_animation_alias_singleshot",                  ReadString },
            { "_animation_alias_burst3",                      ReadString },
            { "_animation_alias_burst4",                      ReadString },
            { "_animation_alias_burst5",                      ReadString },
            { "_animation_alias_burst6",                      ReadString },
            { "_animation_alias_param_f",                     ReadString },
            { "_animation_alias_param_b",                     ReadString },
            { "_animation_alias_param_l",                     ReadString },
            { "_animation_alias_param_r",                     ReadString },
            { "_animation_alias_param_balance",               ReadString },
            { "_animation_alias_turn_r",                      ReadString },
            { "_animation_alias_turn_l",                      ReadString },
            { "_param_idle_blend_dropoff",                    ReadFloat },
            { "_param_turn_blend_min_ratio",                  ReadFloat },
            { "_param_turn_blend_scale",                      ReadFloat },
            { "_blend_in_time",                               ReadFloat },
            { "_blend_out_time",                              ReadFloat },
            { "_animation_mocomp",                            ReadEnum },
            { "_aim_table",                                   ReadEnum },
            { "_gib_location",                                ReadEnum },
            { "_yaw_to_cover_min",                            ReadFloat },
            { "_yaw_to_cover_max",                            ReadFloat },
            { "_should_run",                                  ReadEnum },
            { "_should_howl",                                 ReadEnum },
            { "_arms_position",                               ReadEnum },
            { "_mind_control",                                ReadEnum },
            { "_move_mode",                                   ReadEnum },
            { "_fire_mode",                                   ReadEnum },
            { "_special_death",                               ReadEnum },
            { "_juke_direction",                              ReadEnum },
            { "_juke_distance",                               ReadEnum },
            { "_panic",                                       ReadEnum },
            { "_gibbed_limbs",                                ReadEnum },
            { "_human_cover_flankability",                    ReadEnum },
            { "_robot_step_in",                               ReadEnum },
            { "_awareness",                                   ReadEnum },
            { "_awareness_prev",                              ReadEnum },
            { "_robot_jump_direction",                        ReadEnum },
            { "_robot_wallrun_direction",                     ReadEnum },
            { "_robot_locomotion_type",                       ReadEnum },
            { "_robot_traversal_type",                        ReadEnum },
            { "_staircase_exit_type",                         ReadEnum },
            { "_staircase_type",                              ReadEnum },
            { "_staircase_state",                             ReadEnum },
            { "_staircase_direction",                         ReadEnum },
            { "_staircase_skip_num",                          ReadEnum },
            { "_melee_enemy_type",                            ReadEnum },
            { "_zombie_damageweapon_type",                    ReadEnum },
            { "_parasite_firing_rate",                        ReadEnum },
            { "_margwa_head",                                 ReadEnum },
            { "_margwa_teleport",                             ReadEnum },
            { "_enemy",                                       ReadEnum },
            { "_patrol",                                      ReadEnum },
            { "_knockdown_direction",                         ReadEnum },
            { "_getup_direction",                             ReadEnum },
            { "_push_direction",                              ReadEnum },
            { "_human_locomotion_variation",                  ReadEnum },
            { "_robot_mode",                                  ReadEnum },
            { "_low_gravity",                                 ReadEnum },
            { "_knockdown_type",                              ReadEnum },
            { "_mechz_part",                                  ReadEnum },
            { "_apothicon_bamf_distance_min",                 ReadFloat },
            { "_apothicon_bamf_distance_max",                 ReadFloat },
            { "_keeper_protector_attack",                     ReadEnum },
            { "_keeper_protector_attack_type",                ReadEnum },
            { "_whirlwind_speed",                             ReadEnum },
            { "_quad_wall_crawl",                             ReadEnum },
            { "_quad_phase_direction",                        ReadEnum },
            { "_quad_phase_distance",                         ReadEnum },
            { "_zombie_blackholebomb_pull_state",             ReadEnum },
        };

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
        public static string ReadFloat(FastFile fastFile)
        {
            fastFile.DecodedStream.Seek(4, SeekOrigin.Current);

            return fastFile.DecodedStream.ReadSingle().ToString();
        }

        /// <summary>
        /// Reads an integer from a compiled Animation Selector Table
        /// </summary>
        /// <param name="fastFile"></param>
        /// <returns></returns>
        public static string ReadInt(FastFile fastFile)
        {
            fastFile.DecodedStream.Seek(4, SeekOrigin.Current);

            return fastFile.DecodedStream.ReadInt32().ToString();
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
        /// Reads an Enumerator from compiled Animation Selector Table
        /// </summary>
        /// <param name="fastFile"></param>
        /// <returns></returns>
        public static string ReadEnum(FastFile fastFile)
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
                        selectorTable.Selectors[i].Rows[j].Columns[k] = "";

                        if (ASTDefinitions.ContainsKey(selectorTable.Selectors[i].Headers[k]))
                        {
                            selectorTable.Selectors[i].Rows[j].Columns[k] = ASTDefinitions[selectorTable.Selectors[i].Headers[k]](fastFile);
                        }
                        // Unknown Entry
                        else
                        {
                            fastFile.DecodedStream.Seek(8, SeekOrigin.Current);
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
