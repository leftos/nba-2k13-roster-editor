using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace NBA_2K13_Roster_Editor
{
    public class NonByteAlignedBinaryWriter : BinaryWriter
    {
        private int _inBytePosition;

        public NonByteAlignedBinaryWriter(Stream stream) : base(stream)
        {
        }

        public NonByteAlignedBinaryWriter(Stream stream, Encoding encoding)
            : base(stream, encoding)
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

        public void WriteNonByteAlignedByte(byte b, byte[] originalBytes = null)
        {
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

        public void WriteNonByteAlignedBits(string s, byte[] originalBytes)
        {
            string r = "";
            foreach (byte b in originalBytes)
            {
                r += Convert.ToString(b, 2).PadLeft(8, '0');
            }
            char[] rca = r.ToCharArray();
            char[] oca = s.ToCharArray();
            int endPosition = _inBytePosition + oca.Length;
            for (int i = _inBytePosition; i < endPosition; i++)
            {
                rca[i] = oca[i - _inBytePosition];
            }
            var ba = BitStringToByteArray(new string(rca));
            Write(ba);
            //ba.ToList().ForEach(b => Console.WriteLine(b));
            //Console.WriteLine();

            //BaseStream.Position -= ((rca.Length + _inBytePosition) / 8) + 1;
            BaseStream.Position -= (rca.Length/8) - ((s.Length + _inBytePosition)/8);
            _inBytePosition = (s.Length + _inBytePosition)%8;
        }

        public static byte[] BitStringToByteArray(string s)
        {
            int count = s.Length / 8;
            byte[] ba = new byte[count];
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