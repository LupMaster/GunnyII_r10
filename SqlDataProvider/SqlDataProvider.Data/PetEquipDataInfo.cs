using System;
namespace SqlDataProvider.Data
{
	public class PetEquipDataInfo : DataObject
	{
		private ItemTemplateInfo _template;
		private int _ID;
		private int _userID;
		private int _petID;
		private int _eqtemplateID;
		private int _eqType;
		private DateTime _startTime;
		private int _validDate;
		private bool _isExit;
		public ItemTemplateInfo Template
		{
			get
			{
				return this._template;
			}
		}
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
				return this._userID;
			}
			set
			{
				this._userID = value;
				this._isDirty = true;
			}
		}
		public int PetID
		{
			get
			{
				return this._petID;
			}
			set
			{
				this._petID = value;
				this._isDirty = true;
			}
		}
		public int eqTemplateID
		{
			get
			{
				return this._eqtemplateID;
			}
			set
			{
				this._eqtemplateID = value;
				this._isDirty = true;
			}
		}
		public int eqType
		{
			get
			{
				return this._eqType;
			}
			set
			{
				this._eqType = value;
				this._isDirty = true;
			}
		}
		public DateTime startTime
		{
			get
			{
				return this._startTime;
			}
			set
			{
				this._startTime = value;
				this._isDirty = true;
			}
		}
		public int ValidDate
		{
			get
			{
				return this._validDate;
			}
			set
			{
				this._validDate = value;
				this._isDirty = true;
			}
		}
		public bool IsExit
		{
			get
			{
				return this._isExit;
			}
			set
			{
				this._isExit = value;
				this._isDirty = true;
			}
		}
		public PetEquipDataInfo(ItemTemplateInfo temp)
		{
			this._template = temp;
		}
		public PetEquipDataInfo addTempalte(ItemTemplateInfo Template)
		{
			return new PetEquipDataInfo(Template)
			{
				_ID = this._ID,
				_userID = this._userID,
				_petID = this._petID,
				_eqType = this._eqType,
				_eqtemplateID = this._eqtemplateID,
				_validDate = this._validDate,
				_startTime = this._startTime,
				_isExit = this._isExit
			};
		}
		public bool IsValidate()
		{
			return this._validDate == 0 || DateTime.Compare(this._startTime.AddDays((double)this._validDate), DateTime.Now) > 0;
		}
	}
}
