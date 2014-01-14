using System;
namespace SqlDataProvider.Data
{
	public class TreasureDataInfo : DataObject
	{
		private int _ID;
		private int _UserID;
		private int _TemplateID;
		private int _ValidDate;
		private int _Count;
		private int _pos;
		private DateTime _BeginDate;
		private bool _IsExit;
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				this._ID = value;
				this._isDirty = true;
			}
		}
		public int UserID
		{
			get
			{
				return this._UserID;
			}
			set
			{
				this._UserID = value;
				this._isDirty = true;
			}
		}
		public int TemplateID
		{
			get
			{
				return this._TemplateID;
			}
			set
			{
				this._TemplateID = value;
				this._isDirty = true;
			}
		}
		public int ValidDate
		{
			get
			{
				return this._ValidDate;
			}
			set
			{
				this._ValidDate = value;
				this._isDirty = true;
			}
		}
		public int Count
		{
			get
			{
				return this._Count;
			}
			set
			{
				this._Count = value;
				this._isDirty = true;
			}
		}
		public int pos
		{
			get
			{
				return this._pos;
			}
			set
			{
				this._pos = value;
				this._isDirty = true;
			}
		}
		public DateTime BeginDate
		{
			get
			{
				return this._BeginDate;
			}
			set
			{
				this._BeginDate = value;
				this._isDirty = true;
			}
		}
		public bool IsExit
		{
			get
			{
				return this._IsExit;
			}
			set
			{
				this._IsExit = value;
				this._isDirty = true;
			}
		}
	}
}
