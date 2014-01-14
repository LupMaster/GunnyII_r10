using System;
namespace SqlDataProvider.Data
{
	public class RuneTemplateInfo
	{
		public int TemplateID
		{
			get;
			set;
		}
		public int NextTemplateID
		{
			get;
			set;
		}
		public string Name
		{
			get;
			set;
		}
		public int BaseLevel
		{
			get;
			set;
		}
		public int MaxLevel
		{
			get;
			set;
		}
		public int Type1
		{
			get;
			set;
		}
		public string Attribute1
		{
			get;
			set;
		}
		public int Turn1
		{
			get;
			set;
		}
		public int Rate1
		{
			get;
			set;
		}
		public int Type2
		{
			get;
			set;
		}
		public string Attribute2
		{
			get;
			set;
		}
		public int Turn2
		{
			get;
			set;
		}
		public int Rate2
		{
			get;
			set;
		}
		public int Type3
		{
			get;
			set;
		}
		public string Attribute3
		{
			get;
			set;
		}
		public int Turn3
		{
			get;
			set;
		}
		public int Rate3
		{
			get;
			set;
		}
		public bool IsAttack()
		{
			switch ((this.Type1 == 37) ? this.Type2 : this.Type1)
			{
			case 1:
			case 4:
			case 5:
			case 8:
			case 9:
			case 11:
			case 12:
			case 14:
			case 16:
			case 17:
			case 18:
			case 21:
			case 22:
			case 23:
			case 24:
			case 25:
				return true;
			}
			return false;
		}
		public bool IsDefend()
		{
			int num = (this.Type1 == 39) ? this.Type2 : this.Type1;
			int num2 = num;
			if (num2 <= 10)
			{
				if (num2 != 2 && num2 != 6 && num2 != 10)
				{
					return false;
				}
			}
			else
			{
				switch (num2)
				{
				case 13:
				case 15:
					break;

				case 14:
					return false;

				default:
					if (num2 != 19 && num2 != 26)
					{
						return false;
					}
					break;
				}
			}
			return true;
		}
		public bool IsProp()
		{
			switch (this.Type1)
			{
			case 31:
			case 32:
			case 33:
			case 34:
			case 35:
			case 36:
				return true;

			default:
				return false;
			}
		}
	}
}
