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
using System.Reflection;
using System.Linq;
using T7FastFileUtil;
using PhilUtil;

namespace T7Util
{
    class Program
    {
        /// <summary>
        /// Application Version
        /// </summary>
        public static string AppVersion = "0.1.5 Alpha";

        /// <summary>
        /// Main App Function
        /// </summary>
        /// <param name="args">Command Line Args/Fast Files</param>
        static void Main(string[] args)
        {
            // Force working directory back to exe
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            Console.SetWindowSize((int)(Console.WindowWidth * 1.2), (int)(Console.WindowHeight * 1.5));
            // Load Settings
            try
            {
                Settings.Load("Settings.json");
            }
            catch
            {
                Settings.Write("Settings.json");
            }
            // General App Info
            Print.Info(@" _   ___   _____________  ___  __   __");
            Print.Info(@"| | | \ \ / /  _  \ ___ \/ _ \ \ \ / /");
            Print.Info(@"| |_| |\ V /| | | | |_/ / /_\ \ \ V / ");
            Print.Info(@"|  _  | \ / | | | |    /|  _  | /   \ ");
            Print.Info(@"| | | | | | | |/ /| |\ \| | | |/ /^\ \");
            Print.Info(@"\_| |_/ \_/ |___/ \_| \_\_| |_/\/   \/");
            Print.Info();
            Print.Info(string.Format("by Scobalula - Version {0}", AppVersion));
            Print.Info();
            Print.Info("Usage - Drag and Drop a Fast File/Fast Files");
            Print.Info();
            // Load String Cache
            GlobalStringTable.LoadStringCache();

            string[] files = args.Where(x => Path.GetExtension(x) == ".ff" && File.Exists(x)).ToArray();

            if (files.Length < 1)
            {
                Print.Error("No valid Fast Files given.");
            }

            foreach (var file in files)
            {
                if(!FileUtil.CanAccessFile(file))
                {
                    Print.Error(string.Format("File {0} is in-use or permissions were denied", Path.GetFileName(file)));
                    continue;
                }

                FastFile fastFile = new FastFile();
                try
                {
                    fastFile.Decode(file);
                    fastFile.Load();

                }
                catch(Exception e)
                {
                    Print.Exception(e);
                }
            }

            Print.Info();            
            Print.Info(string.Format("{0} Files Processed.", files.Length));
            Print.Info();

            GlobalStringTable.WriteStringCache();

            Print.Info("Press enter to exit");

            Console.ReadLine();
        }
    }
}
