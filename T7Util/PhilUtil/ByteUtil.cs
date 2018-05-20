using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhilUtil
{
    class ByteUtil
    {
        public static string ReadCString(byte[] input, int startIndex, int maxSize = 0)
        {
            List<byte> result = new List<byte>();

            for(int i = startIndex; i < input.Length && input[i] != 0 && i < maxSize; i++)
            {
                result.Add(input[i]);
            }

            return Encoding.ASCII.GetString(result.ToArray());
        }
    }
}
