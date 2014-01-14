using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Game.Base.Packets;
using System.Threading;
using log4net;
using System.Reflection;
namespace Game.Base
{
	public class PacketIn
	{
		protected byte[] m_buffer;
		protected int m_length;
		protected int m_offset;
		public static int[] SEND_KEY = new int[]
		{
			174,
			191,
			86,
			120,
			171,
			205,
			239,
			241
		};
		public volatile bool isSended = true;
		public volatile int m_sended;
		public volatile int packetNum;
		public volatile int m_loop;
		public byte[] Buffer
		{
			get
			{
				return this.m_buffer;
			}
		}
		public int Length
		{
			get
			{
				return this.m_length;
			}
		}
		public int Offset
		{
			get
			{
				return this.m_offset;
			}
			set
			{
				this.m_offset = value;
			}
		}
		public int DataLeft
		{
			get
			{
				return this.m_length - this.m_offset;
			}
		}
		public PacketIn(byte[] buf, int len)
		{
			this.m_buffer = buf;
			this.m_length = len;
			this.m_offset = 0;
		}
		public void Skip(int num)
		{
			this.m_offset += num;
		}
		public virtual bool ReadBoolean()
		{
			return this.m_buffer[this.m_offset++] != 0;
		}
		public virtual byte ReadByte()
		{
			return this.m_buffer[this.m_offset++];
		}
		public virtual short ReadShort()
		{
			byte v = this.ReadByte();
			byte v2 = this.ReadByte();
			return Marshal.ConvertToInt16(v, v2);
		}
		public virtual short ReadShortLowEndian()
		{
			byte v = this.ReadByte();
			byte v2 = this.ReadByte();
			return Marshal.ConvertToInt16(v2, v);
		}
		public virtual int ReadInt()
		{
			byte v = this.ReadByte();
			byte v2 = this.ReadByte();
			byte v3 = this.ReadByte();
			byte v4 = this.ReadByte();
			return Marshal.ConvertToInt32(v, v2, v3, v4);
		}
		public virtual uint ReadUInt()
		{
			byte v = this.ReadByte();
			byte v2 = this.ReadByte();
			byte v3 = this.ReadByte();
			byte v4 = this.ReadByte();
			return Marshal.ConvertToUInt32(v, v2, v3, v4);
		}
		public virtual long ReadLong()
		{
			int v = this.ReadInt();
			uint v2 = this.ReadUInt();
			return Marshal.ConvertToInt64(v, v2);
		}
		public virtual float ReadFloat()
		{
			byte[] array = new byte[4];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.ReadByte();
			}
			return BitConverter.ToSingle(array, 0);
		}
		public virtual double ReadDouble()
		{
			byte[] array = new byte[8];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = this.ReadByte();
			}
			return BitConverter.ToDouble(array, 0);
		}
		public virtual string ReadString()
		{
			short num = this.ReadShort();
			string @string = Encoding.UTF8.GetString(this.m_buffer, this.m_offset, (int)num);
			this.m_offset += (int)num;
			return @string.Replace("\0", "");
		}
		public virtual byte[] ReadBytes(int maxLen)
		{
			byte[] array = new byte[maxLen];
			Array.Copy(this.m_buffer, this.m_offset, array, 0, maxLen);
			this.m_offset += maxLen;
			return array;
		}
		public virtual byte[] ReadBytes()
		{
			return this.ReadBytes(this.m_length - this.m_offset);
		}
		public DateTime ReadDateTime()
		{
			return new DateTime((int)this.ReadShort(), (int)this.ReadByte(), (int)this.ReadByte(), (int)this.ReadByte(), (int)this.ReadByte(), (int)this.ReadByte());
		}
		public virtual int CopyTo(byte[] dst, int dstOffset, int offset)
		{
			int num = (this.m_length - offset < dst.Length - dstOffset) ? (this.m_length - offset) : (dst.Length - dstOffset);
			if (num > 0)
			{
				System.Buffer.BlockCopy(this.m_buffer, offset, dst, dstOffset, num);
			}
			return num;
		}
		public virtual int CopyTo(byte[] dst, int dstOffset, int offset, int key)
		{
			int num = (this.m_length - offset < dst.Length - dstOffset) ? (this.m_length - offset) : (dst.Length - dstOffset);
			if (num > 0)
			{
				key = (key & 16711680) >> 16;
				for (int i = 0; i < num; i++)
				{
					dst[dstOffset + i] = (byte)((int)this.m_buffer[offset + i] ^ key);
				}
			}
			return num;
        }
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public virtual int CopyTo3(byte[] dst, int dstOffset, int offset, byte[] key, ref int packetArrangeSend)
		{
			int num = (this.m_length - offset < dst.Length - dstOffset) ? (this.m_length - offset) : (dst.Length - dstOffset);
			string arg_24_0 = string.Empty;
			/*Console.WriteLine(string.Format("?????????dst.length {0} dstOffset {1} offset {2} headShot {3}", new object[]
			{
				num,
				dstOffset,
				offset,
				m_loop
			}));*/
            lock (this)
            {
                if (num > 0)
                {
                    int i = this.m_sended + dstOffset;
                    if (isSended)
                    {
                        packetNum = Interlocked.Increment(ref packetArrangeSend);
                        packetArrangeSend = packetNum;
                        m_sended = 0;
                        isSended = false;
                        i = m_sended + dstOffset;
                    }
                    else
                    {
                        i = 8192;
                    }
                    if (packetNum != packetArrangeSend)
                    {
                        Console.WriteLine("____LoiGameBase____");
                        return 0;
                    }
                    for (int j = 0; j < num; j++)
                    {
                        int num2 = offset + j;
                        while (i > 8192)
                        {
                            i -= 8192;
                        }
                        if (this.m_sended == 0)
                        {
                            dst[dstOffset] = (byte)(m_buffer[num2] ^ key[m_sended % 8]);
                        }
                        else
                        {
                            key[m_sended % 8] = (byte)((int)(key[m_sended % 8] + dst[i - 1]) ^ m_sended);
                            dst[dstOffset + j] = (byte)(((uint)m_buffer[num2] ^ (uint)key[m_sended % 8]) + (uint)dst[i - 1]);
                        }
                        m_sended++;
                        i++;
                    }
                }
            }
			return num;
		}
		public virtual int CopyFrom(byte[] src, int srcOffset, int offset, int count)
		{
			if (count < this.m_buffer.Length && count - srcOffset < src.Length)
			{
				System.Buffer.BlockCopy(src, srcOffset, this.m_buffer, offset, count);
				return count;
			}
			return -1;
		}
		public virtual int CopyFrom(byte[] src, int srcOffset, int offset, int count, int key)
		{
			if (count < this.m_buffer.Length && count - srcOffset < src.Length)
			{
				key = (key & 16711680) >> 16;
				for (int i = 0; i < count; i++)
				{
					this.m_buffer[offset + i] = (byte)((int)src[srcOffset + i] ^ key);
				}
				return count;
			}
			return -1;
		}
		public virtual int[] CopyFrom3(byte[] src, int srcOffset, int offset, int count, byte[] key)
		{
			int[] result = new int[count];
			for (int i = 0; i < count; i++)
			{
				this.m_buffer[i] = src[i];
			}
			if (count < this.m_buffer.Length && count - srcOffset < src.Length)
			{
                this.m_buffer[0] = (byte)(src[srcOffset] ^ key[0]);
				for (int j = 1; j < count; j++)
				{
					key[j % 8] = (byte)((int)(key[j % 8] + src[srcOffset + j - 1]) ^ j);
                    this.m_buffer[j] = (byte)(src[srcOffset + j] - src[srcOffset + j - 1] ^ key[j % 8]);
				}
			}
			return result;
		}
		public virtual void WriteBoolean(bool val)
		{
            this.m_buffer[this.m_offset++] = (byte)(val ? 1 : 0);
			this.m_length = ((this.m_offset > this.m_length) ? this.m_offset : this.m_length);
		}
		public virtual void WriteByte(byte val)
		{
			this.m_buffer[this.m_offset++] = val;
			this.m_length = ((this.m_offset > this.m_length) ? this.m_offset : this.m_length);
		}
		public virtual void Write(byte[] src)
		{
			this.Write(src, 0, src.Length);
		}
		public virtual void Write(byte[] src, int offset, int len)
		{
			Array.Copy(src, offset, this.m_buffer, this.m_offset, len);
			this.m_offset += len;
			this.m_length = ((this.m_offset > this.m_length) ? this.m_offset : this.m_length);
		}
		public virtual void WriteShort(short val)
		{
			this.WriteByte((byte)(val >> 8));
			this.WriteByte((byte)(val & 255));
		}
		public virtual void WriteShortLowEndian(short val)
		{
			this.WriteByte((byte)(val & 255));
			this.WriteByte((byte)(val >> 8));
		}
		public virtual void WriteInt(int val)
		{
			this.WriteByte((byte)(val >> 24));
			this.WriteByte((byte)(val >> 16 & 255));
			this.WriteByte((byte)((val & 65535) >> 8));
			this.WriteByte((byte)(val & 65535 & 255));
		}
		public virtual void WriteUInt(uint val)
		{
			this.WriteByte((byte)(val >> 24));
			this.WriteByte((byte)(val >> 16 & 255u));
			this.WriteByte((byte)((val & 65535u) >> 8));
			this.WriteByte((byte)(val & 65535u & 255u));
		}
        public virtual void WriteLong(long val)
        {
            long num = val;
            int val1 = (int)num;
            string str1 = Convert.ToString(num, 2);
            string str2 = str1.Length <= 32 ? "" : str1.Substring(0, str1.Length - 32);
            int val2 = 0;
            for (int index = 0; index < str2.Length; ++index)
            {
                string str3 = str2.Substring(str2.Length - (index + 1));
                if (!(str3 == "0"))
                {
                    if (str3 == "1")
                        val2 += 1 << index;
                    else
                        break;
                }
            }
            this.WriteInt(val2);
            this.WriteInt(val1);
        }
		public virtual void WriteFloat(float val)
		{
			byte[] bytes = BitConverter.GetBytes(val);
			this.Write(bytes);
		}
		public virtual void WriteDouble(double val)
		{
			byte[] bytes = BitConverter.GetBytes(val);
			this.Write(bytes);
		}
		public virtual void Fill(byte val, int num)
		{
			for (int i = 0; i < num; i++)
			{
				this.WriteByte(val);
			}
		}
		public virtual void WriteString(string str)
		{
			if (!string.IsNullOrEmpty(str))
			{
				byte[] bytes = Encoding.UTF8.GetBytes(str);
				this.WriteShort((short)(bytes.Length + 1));
				this.Write(bytes, 0, bytes.Length);
				this.WriteByte(0);
				return;
			}
			this.WriteShort(1);
			this.WriteByte(0);
		}
		public virtual void WriteString(string str, int maxlen)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(str);
			int num = (bytes.Length < maxlen) ? bytes.Length : maxlen;
			this.WriteShort((short)num);
			this.Write(bytes, 0, num);
		}
		public void WriteDateTime(DateTime date)
		{
			this.WriteShort((short)date.Year);
			this.WriteByte((byte)date.Month);
			this.WriteByte((byte)date.Day);
			this.WriteByte((byte)date.Hour);
			this.WriteByte((byte)date.Minute);
			this.WriteByte((byte)date.Second);
		}
	}
}
