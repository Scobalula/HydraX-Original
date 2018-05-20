using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhilUtil
{
    /// <summary>
    /// Common Hash Functions
    /// </summary>
    class Hash
    {
        /// <summary>
        /// Calculates DJB Hash for a sequence of bytes.
        /// </summary>
        /// <param name="bytes">Sequence of Bytes</param>
        /// <returns>Unsigned DJB Int</returns>
        public static uint DJB(byte[] bytes)
        {
            uint hash = 0x1505;

            for (int i = 0; i < bytes.Length; i++)
            {
                hash = ((hash << 5) + hash) + bytes[i];
            }

            return hash;
        }
    }
}
