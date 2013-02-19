#region Copyright Notice

//    Copyright 2011-2013 Eleftherios Aslanoglou
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

#endregion

#region Using Directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

#endregion

namespace NonByteAlignedBinaryRW
{
    public class NonByteAlignedBinaryWriter : BinaryWriter
    {
        private int _inBytePosition;

        public NonByteAlignedBinaryWriter(Stream stream) : base(stream)
        {
        }

        public NonByteAlignedBinaryWriter(Stream stream, Encoding encoding) : base(stream, encoding)
        {
        }

        public int InBytePosition
        {
            get { return _inBytePosition; }
            set
            {
                if (value >= 0 && value <= 7)
                {
                    _inBytePosition = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value", "The InBytePosition value can't be less than 0 or larger than 7.");
                }
            }
        }

        private readonly List<long> _positionsToWatch = new List<long> {58920, 58921, 58922};
        private const bool WatchPositions = false;

        public void WriteNonByteAlignedByte(byte b, byte[] originalBytes = null)
        {
#if DEBUG
            if (WatchPositions && _positionsToWatch.Contains(BaseStream.Position))
            {
                System.Diagnostics.Debugger.Break();
            }
#endif   
            if (_inBytePosition == 0)
            {
                Write(b);
            }
            else
            {
                var ba = new BitArray(originalBytes);
                var temp = new BitArray(new[] {b});
                var r = new BitArray(8);
                for (int i = 0; i < 8; i++)
                {
                    r[7 - i] = temp[i];
                }
                for (int i = _inBytePosition; i < _inBytePosition + 8; i++)
                {
                    int pos = 7 + i - 2*(i%8);
                    ba[pos] = r[i - _inBytePosition];
                }
                var bytes = new byte[2];
                ba.CopyTo(bytes, 0);
                Write(bytes);
                BaseStream.Position -= 1;
            }
        }

        public void WriteNonByteAlignedBits(string s, IEnumerable<byte> originalBytes)
        {
#if DEBUG
            if (WatchPositions && _positionsToWatch.Contains(BaseStream.Position))
            {
                System.Diagnostics.Debugger.Break();
            }
#endif   
            string r = originalBytes.Aggregate("", (current, b) => current + Convert.ToString(b, 2).PadLeft(8, '0'));
            char[] rca = r.ToCharArray();
            char[] oca = s.ToCharArray();
            int endPosition = _inBytePosition + oca.Length;
            for (int i = _inBytePosition; i < endPosition; i++)
            {
                rca[i] = oca[i - _inBytePosition];
            }
            byte[] ba = BitStringToByteArray(new string(rca));
            Write(ba);
            //ba.ToList().ForEach(b => Console.WriteLine(b));
            //Console.WriteLine();

            //BaseStream.Position -= ((rca.Length + _inBytePosition) / 8) + 1;
            BaseStream.Position -= (rca.Length/8) - ((s.Length + _inBytePosition)/8);
            _inBytePosition = (s.Length + _inBytePosition)%8;
        }

        public static byte[] BitStringToByteArray(string s)
        {
            int count = s.Length/8;
            var ba = new byte[count];
            for (int j = 0; j < count; j++)
            {
                byte b = 0;
                char[] ca = s.Skip(j*8).Take(8).Reverse().ToArray();
                for (int i = 0; i < ca.Length; i++)
                {
                    if (ca[i] == '1')
                    {
                        b += Convert.ToByte(Math.Pow(2, i));
                    }
                }
                ba[j] = b;
            }
            return ba;
        }

        public static byte BitStringToByte(string s)
        {
            return BitStringToByteArray(s)[0];
        }

        public void MoveStreamPosition(int bytes, int bits)
        {
            if (_inBytePosition + bits >= 8)
            {
                BaseStream.Position += (_inBytePosition + bits)/8;
            }
            _inBytePosition = (_inBytePosition + bits)%8;
            BaseStream.Position += bytes;
        }
    }
}