/*
    Copyright (c) 2018 Philip/Scobalula - Utility Lib

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using System;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace PhilUtil
{
    class Print
    {
        public static bool EnableDebug = true;
        /// <summary>
        /// Print general info
        /// </summary>
        public static void Info(object value = null, bool newLine = true)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Write(" INFO        |");
            Console.ResetColor();
            if(newLine)
                Console.WriteLine(" {0}", value);
            else
                Console.Write(" {0}", value);
        }

        /// <summary>
        /// Print general info
        /// </summary>
        public static void Debug(object value = null, bool newLine = true)
        {
            if (EnableDebug)
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.Write(" DEBUG       |");
                Console.ResetColor();
                if (newLine)
                    Console.WriteLine(" {0}", value);
                else
                    Console.Write(" {0}", value);
            }
        }

        /// <summary>
        /// Prints an error with Red background.
        /// </summary>
        public static void Error(object value = null)
        {
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write(" ERROR       |");
            Console.WriteLine(" {0}", value);
            Console.ResetColor();
        }

        /// <summary>
        /// Print general info
        /// </summary>
        public static void Warning(object value = null, bool newLine = true)
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(" WARNING     |");
            Console.ResetColor();
            if (newLine)
                Console.WriteLine(" {0}", value);
            else
                Console.Write(" {0}", value);
        }

        /// <summary>
        /// Print export info
        /// </summary>
        public static void Export(string assetName, long position)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Write(" EXPORT      |");
            Console.ResetColor();
            Console.WriteLine(" Exporting : {0} - Position 0x{1:X}", Path.GetFileName(assetName), position);
        }

        public static void Exception(Exception value)
        {
            StackTrace trace = new StackTrace(value, true);
            StackFrame frame = trace.GetFrame(0);

            Error("Unhandled Exception Occured");
            using (StringReader reader = new StringReader(value.ToString()))
            {
                string line;

                while((line = reader.ReadLine()) != null)
                {
                    Error(line);
                }
            }
        }
        /// <summary>
        /// Print general info
        /// </summary>
        public static void Custom(object value = null, object prefix = null, ConsoleColor bgColor = ConsoleColor.DarkBlue)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            
            Console.Write(" INFO        |");
            Console.ResetColor();
            Console.WriteLine(" {0}", value);
        }
    }
}
