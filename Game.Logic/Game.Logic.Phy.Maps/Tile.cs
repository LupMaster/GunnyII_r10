using System;
using System.Drawing;
using System.IO;
namespace Game.Logic.Phy.Maps
{
	public class Tile
	{
		private byte[] _data;
		private int _width;
		private int _height;
		private Rectangle _rect;
		private int _bw;
		private int _bh;
		private bool _digable;
		public Rectangle Bound
		{
			get
			{
				return this._rect;
			}
		}
		public byte[] Data
		{
			get
			{
				return this._data;
			}
		}
		public int Width
		{
			get
			{
				return this._width;
			}
		}
		public int Height
		{
			get
			{
				return this._height;
			}
		}
		public Tile(byte[] data, int width, int height, bool digable)
		{
			this._data = data;
			this._width = width;
			this._height = height;
			this._digable = digable;
			this._bw = this._width / 8 + 1;
			this._bh = this._height;
			this._rect = new Rectangle(0, 0, this._width, this._height);
			GC.AddMemoryPressure((long)data.Length);
		}
        public Tile(Bitmap bitmap, bool digable)
        {
            this._width = bitmap.Width;
            this._height = bitmap.Height;
            this._bw = this._width / 8 + 1;
            this._bh = this._height;
            this._data = new byte[this._bw * this._bh];
            this._digable = digable;
            for (int y = 0; y < bitmap.Height; ++y)
            {
                for (int x = 0; x < bitmap.Width; ++x)
                {
                    byte num = (int)bitmap.GetPixel(x, y).A <= 100 ? (byte)0 : (byte)1;
                    this._data[y * this._bw + x / 8] |= (byte)((uint)num << 7 - x % 8);
                }
            }
            this._rect = new Rectangle(0, 0, this._width, this._height);
            GC.AddMemoryPressure((long)this._data.Length);
        }/*
		public Tile(Bitmap bitmap, bool digable)
		{
			this._width = bitmap.Width;
			this._height = bitmap.Height;
			this._bw = this._width / 8 + 1;
			this._bh = this._height;
			this._data = new byte[this._bw * this._bh];
			this._digable = digable;
			for (int i = 0; i < bitmap.Height; i++)
			{
				for (int j = 0; j < bitmap.Width; j++)
				{
					byte b = (bitmap.GetPixel(j, i).A <= 100) ? 0 : 1;
					byte[] expr_92_cp_0 = this._data;
					int expr_92_cp_1 = i * this._bw + j / 8;
					expr_92_cp_0[expr_92_cp_1] |= (byte)(b << 7 - j % 8);
				}
			}
			this._rect = new Rectangle(0, 0, this._width, this._height);
			GC.AddMemoryPressure((long)this._data.Length);
		}*/
		public Tile(string file, bool digable)
		{
			FileStream input = File.Open(file, FileMode.Open);
			BinaryReader binaryReader = new BinaryReader(input);
			this._width = binaryReader.ReadInt32();
			this._height = binaryReader.ReadInt32();
			this._bw = this._width / 8 + 1;
			this._bh = this._height;
			this._data = binaryReader.ReadBytes(this._bw * this._bh);
			this._digable = digable;
			this._rect = new Rectangle(0, 0, this._width, this._height);
			binaryReader.Close();
			GC.AddMemoryPressure((long)this._data.Length);
		}
		public void Dig(int cx, int cy, Tile surface, Tile border)
		{
			if (this._digable && surface != null)
			{
				int x = cx - surface.Width / 2;
				int y = cy - surface.Height / 2;
				this.Remove(x, y, surface);
				if (border != null)
				{
					x = cx - border.Width / 2;
					y = cy - border.Height / 2;
					this.Add(x, y, surface);
				}
			}
		}
		protected void Add(int x, int y, Tile tile)
		{
		}
		protected void Remove(int x, int y, Tile tile)
		{
			byte[] data = tile._data;
			Rectangle bound = tile.Bound;
			bound.Offset(x, y);
			bound.Intersect(this._rect);
			if (bound.Width != 0 && bound.Height != 0)
			{
				bound.Offset(-x, -y);
				int num = bound.X / 8;
				int num2 = (bound.X + x) / 8;
				int y2 = bound.Y;
				int num3 = bound.Width / 8 + 1;
				int height = bound.Height;
				if (bound.X == 0)
				{
					if (num3 + num2 < this._bw)
					{
						num3++;
						num3 = ((num3 > tile._bw) ? tile._bw : num3);
					}
					int num4 = (bound.X + x) % 8;
					for (int i = 0; i < height; i++)
					{
						int num5 = 0;
						for (int j = 0; j < num3; j++)
						{
							int num6 = (i + y + y2) * this._bw + j + num2;
							int num7 = (i + y2) * tile._bw + j + num;
							int num8 = (int)data[num7];
							int num9 = num8 >> num4;
							int num10 = (int)this._data[num6];
							num10 &= ~(num10 & num9);
							if (num5 != 0)
							{
								num10 &= ~(num10 & num5);
							}
							this._data[num6] = (byte)num10;
							num5 = num8 << 8 - num4;
						}
					}
					return;
				}
				int num11 = bound.X % 8;
				for (int k = 0; k < height; k++)
				{
					for (int l = 0; l < num3; l++)
					{
						int num12 = (k + y + y2) * this._bw + l + num2;
						int num13 = (k + y2) * tile._bw + l + num;
						int num14 = (int)data[num13];
						int num15 = num14 << num11;
						int num16;
						if (l < num3 - 1)
						{
							num14 = (int)data[num13 + 1];
							num16 = num14 >> 8 - num11;
						}
						else
						{
							num16 = 0;
						}
						int num17 = (int)this._data[num12];
						num17 &= ~(num17 & num15);
						if (num16 != 0)
						{
							num17 &= ~(num17 & num16);
						}
						this._data[num12] = (byte)num17;
					}
				}
			}
		}
		public bool IsEmpty(int x, int y)
		{
			if (x >= 0 && x < this._width && y >= 0 && y < this._height)
			{
				byte b = (byte)(1 << 7 - x % 8);
				return (this._data[y * this._bw + x / 8] & b) == 0;
			}
			return true;
		}
		public bool IsYLineEmtpy(int x, int y, int h)
		{
			if (x >= 0 && x < this._width)
			{
				y = ((y < 0) ? 0 : y);
				h = ((y + h > this._height) ? (this._height - y) : h);
				for (int i = 0; i < h; i++)
				{
					if (!this.IsEmpty(x, y + i))
					{
						return false;
					}
				}
				return true;
			}
			return true;
		}
		public bool IsRectangleEmptyQuick(Rectangle rect)
		{
			rect.Intersect(this._rect);
			return this.IsEmpty(rect.Right, rect.Bottom) && this.IsEmpty(rect.Left, rect.Bottom) && this.IsEmpty(rect.Right, rect.Top) && this.IsEmpty(rect.Left, rect.Top);
		}
		public Point FindNotEmptyPoint(int x, int y, int h)
		{
			if (x >= 0 && x < this._width)
			{
				y = ((y < 0) ? 0 : y);
				h = ((y + h > this._height) ? (this._height - y) : h);
				for (int i = 0; i < h; i++)
				{
					if (!this.IsEmpty(x, y + i))
					{
						return new Point(x, y + i);
					}
				}
				return new Point(-1, -1);
			}
			return new Point(-1, -1);
		}
		public Bitmap ToBitmap()
		{
			Bitmap bitmap = new Bitmap(this._width, this._height);
			for (int i = 0; i < this._height; i++)
			{
				for (int j = 0; j < this._width; j++)
				{
					if (this.IsEmpty(j, i))
					{
						bitmap.SetPixel(j, i, Color.FromArgb(0, 0, 0, 0));
					}
					else
					{
						bitmap.SetPixel(j, i, Color.FromArgb(255, 0, 0, 0));
					}
				}
			}
			return bitmap;
		}
		public Tile Clone()
		{
			return new Tile(this._data.Clone() as byte[], this._width, this._height, this._digable);
		}
	}
}
