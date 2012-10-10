using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;

namespace NBA_2K13_Roster_Editor
{
    public class NonByteAlignedBinaryReader : BinaryReader
    {
        private int _inBytePosition;

        public NonByteAlignedBinaryReader(Stream stream) : base(stream)
        {
        }

        public NonByteAlignedBinaryReader(Stream stream, Encoding encoding)
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

        public byte ReadNonByteAlignedByte()
        {
            if (_inBytePosition == 0)
            {
                return ReadByte();
            }
            else
            {
                var ba = new BitArray(ReadBytes(2));
                BaseStream.Position -= 1;
                var r = new BitArray(8);
                for (int i = _inBytePosition; i < _inBytePosition + 8; i++)
                {
                    int pos = 7 + i - 2*(i%8);
                    r[i - _inBytePosition] = ba[pos];
                }
                var temp = new BitArray(8);
                for (int i = 0; i < 8; i++)
                {
                    temp[i] = r[7 - i];
                }
                return BitArrayToByte(temp);
            }
        }

        public byte[] ReadNonByteAlignedBytes(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("count");

            if (_inBytePosition == 0)
            {
                return ReadBytes(count);
            }
            else
            {
                var bytes = new byte[count];
                for (int i = 0; i < count; i++)
                {
                    bytes[i] = ReadNonByteAlignedByte();
                }
                return bytes;
            }
        }

        public string ReadNonByteAlignedBits(int count)
        {
            if (count <= 0)
                throw new ArgumentOutOfRangeException("count");

            string s = "";
            int bytesToRead = (count - 1)/8;
            if (bytesToRead > 0)
            {
                byte[] bytes = ReadNonByteAlignedBytes(bytesToRead);
                s += ByteArrayToBitString(bytes);
            }

            int bitsToRead = (count%8);
            if (bitsToRead > 0)
            {
                byte rem = ReadNonByteAlignedByte();
                _inBytePosition += bitsToRead;
                if (_inBytePosition >= 8)
                {
                    _inBytePosition = _inBytePosition%8;
                }
                else
                {
                    BaseStream.Position -= 1;
                }

                s += ByteToBitString(rem, bitsToRead);
            }

            return s;
        }

        public static string ByteToBitString(byte b, int length = 8, int offset = 0)
        {
            if (length + offset > 8)
                throw new ArgumentOutOfRangeException();

            var ba = new BitArray(new[] {b});
            string s = "";
            for (int i = 7 - offset; i > 7 - length; i--)
            {
                if (ba[i])
                    s += "1";
                else
                    s += "0";
            }
            return s;
        }

        public static byte BitStringToByte(string s)
        {
            return BitStringToByteArray(s)[0];
        }

        public static byte[] BitStringToByteArray(string s)
        {
            int count = ((s.Length-1)/8)+1;
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

        public static string ByteArrayToBitString(byte[] bytes)
        {
            string s = "";
            foreach (byte b in bytes)
            {
                s += ByteToBitString(b);
            }
            return s;
        }

        public static byte[] BitArrayToByteArray(BitArray bits)
        {
            var ret = new byte[bits.Length/8];
            bits.CopyTo(ret, 0);
            return ret;
        }

        public static byte BitArrayToByte(BitArray bits)
        {
            if (bits.Length != 8)
                throw new ArgumentException("The BitArray parameter can't have a length other than 8 bits.");

            return BitArrayToByteArray(bits)[0];
        }

        public void MoveStreamPosition(int bytes, int bits)
        {
            if ((_inBytePosition + bits >= 8))
            {
                BaseStream.Position += (_inBytePosition + bits) / 8;
            }
            if (_inBytePosition + bits >= 0)
            {
                _inBytePosition = (_inBytePosition + bits) % 8;
            }
            else
            {
                BaseStream.Position += ((_inBytePosition + bits)/8) - 1;
                _inBytePosition = ((_inBytePosition + bits)%8) + 8;
            }
            BaseStream.Position += bytes;
        }
    }
}