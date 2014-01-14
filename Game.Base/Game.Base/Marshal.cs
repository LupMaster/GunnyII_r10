using System;
using System.IO;
using System.Text;
using zlib;

namespace Game.Base
{
    public class Marshal
    {
        public static string ConvertToString(byte[] cstyle)
        {
            if (cstyle == null)
                return (string)null;
            for (int count = 0; count < cstyle.Length; ++count)
            {
                if ((int)cstyle[count] == 0)
                    return Encoding.Default.GetString(cstyle, 0, count);
            }
            return Encoding.Default.GetString(cstyle);
        }

        public static int ConvertToInt32(byte[] val)
        {
            return Marshal.ConvertToInt32(val, 0);
        }

        public static long ConvertToInt64(int v1, uint v2)
        {
            int num = 1;
            if (v1 < 0)
                num = -1;
            return (long)((double)num * (Math.Abs((double)v1 * Math.Pow(2.0, 32.0)) + (double)v2));
        }

        public static int ConvertToInt32(byte[] val, int startIndex)
        {
            return Marshal.ConvertToInt32(val[startIndex], val[startIndex + 1], val[startIndex + 2], val[startIndex + 3]);
        }

        public static int ConvertToInt32(byte v1, byte v2, byte v3, byte v4)
        {
            return (int)v1 << 24 | (int)v2 << 16 | (int)v3 << 8 | (int)v4;
        }

        public static uint ConvertToUInt32(byte v1, byte v2, byte v3, byte v4)
        {
            return (uint)((int)v1 << 24 | (int)v2 << 16 | (int)v3 << 8) | (uint)v4;
        }

        public static uint ConvertToUInt32(byte[] val)
        {
            return Marshal.ConvertToUInt32(val, 0);
        }

        public static uint ConvertToUInt32(byte[] val, int startIndex)
        {
            return Marshal.ConvertToUInt32(val[startIndex], val[startIndex + 1], val[startIndex + 2], val[startIndex + 3]);
        }

        public static short ConvertToInt16(byte[] val)
        {
            return Marshal.ConvertToInt16(val, 0);
        }

        public static short ConvertToInt16(byte[] val, int startIndex)
        {
            return Marshal.ConvertToInt16(val[startIndex], val[startIndex + 1]);
        }

        public static short ConvertToInt16(byte v1, byte v2)
        {
            return (short)((int)v1 << 8 | (int)v2);
        }

        public static ushort ConvertToUInt16(byte[] val)
        {
            return Marshal.ConvertToUInt16(val, 0);
        }

        public static ushort ConvertToUInt16(byte[] val, int startIndex)
        {
            return Marshal.ConvertToUInt16(val[startIndex], val[startIndex + 1]);
        }

        public static ushort ConvertToUInt16(byte v1, byte v2)
        {
            return (ushort)((uint)v2 | (uint)v1 << 8);
        }

        public static string ToHexDump(string description, byte[] dump)
        {
            return Marshal.ToHexDump(description, dump, 0, dump.Length);
        }

        public static string ToHexDump(string description, byte[] dump, int start, int count)
        {
            StringBuilder stringBuilder1 = new StringBuilder();
            if (description != null)
                stringBuilder1.Append(description).Append("\n");
            int num1 = start + count;
            int num2 = start;
            while (num2 < num1)
            {
                StringBuilder stringBuilder2 = new StringBuilder();
                StringBuilder stringBuilder3 = new StringBuilder();
                stringBuilder3.Append(num2.ToString("X4"));
                stringBuilder3.Append(": ");
                for (int index = 0; index < 16; ++index)
                {
                    if (index + num2 < num1)
                    {
                        byte num3 = dump[index + num2];
                        stringBuilder3.Append(dump[index + num2].ToString("X2"));
                        stringBuilder3.Append(" ");
                        if ((int)num3 >= 32 && (int)num3 <= (int)sbyte.MaxValue)
                            stringBuilder2.Append((char)num3);
                        else
                            stringBuilder2.Append(".");
                    }
                    else
                    {
                        stringBuilder3.Append("   ");
                        stringBuilder2.Append(" ");
                    }
                }
                stringBuilder3.Append("  ");
                stringBuilder3.Append(((object)stringBuilder2).ToString());
                stringBuilder3.Append('\n');
                stringBuilder1.Append(((object)stringBuilder3).ToString());
                num2 += 16;
            }
            return ((object)stringBuilder1).ToString();
        }

        public static byte[] Compress(byte[] src)
        {
            return Marshal.Compress(src, 0, src.Length);
        }

        public static byte[] Compress(byte[] src, int offset, int length)
        {
            MemoryStream memoryStream = new MemoryStream();
            Stream stream = (Stream)new ZOutputStream((Stream)memoryStream, 9);
            stream.Write(src, offset, length);
            stream.Close();
            return memoryStream.ToArray();
        }

        public static byte[] Uncompress(byte[] src)
        {
            MemoryStream memoryStream = new MemoryStream();
            Stream stream = (Stream)new ZOutputStream((Stream)memoryStream);
            stream.Write(src, 0, src.Length);
            stream.Close();
            return memoryStream.ToArray();
        }
    }
}
