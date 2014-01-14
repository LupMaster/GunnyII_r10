using System;
namespace SqlDataProvider.Data
{
	public class UserLabyrinthInfo : DataObject
	{
		private int _userID;
		private int _myProgress;
		private int _myRanking;
		private bool _completeChallenge;
		private bool _isDoubleAward;
		private int _currentFloor;
		private int _accumulateExp;
		private int _remainTime;
		private int _currentRemainTime;
		private int _cleanOutAllTime;
		private int _cleanOutGold;
		private bool _tryAgainComplete;
		private bool _isInGame;
		private bool _isCleanOut;
		private bool _serverMultiplyingPower;
		private DateTime _lastDate;
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
		public int myProgress
		{
			get
			{
				return this._myProgress;
			}
			set
			{
				this._myProgress = value;
				this._isDirty = true;
			}
		}
		public int myRanking
		{
			get
			{
				return this._myRanking;
			}
			set
			{
				this._myRanking = value;
				this._isDirty = true;
			}
		}
		public bool completeChallenge
		{
			get
			{
				return this._completeChallenge;
			}
			set
			{
				this._completeChallenge = value;
				this._isDirty = true;
			}
		}
		public bool isDoubleAward
		{
			get
			{
				return this._isDoubleAward;
			}
			set
			{
				this._isDoubleAward = value;
				this._isDirty = true;
			}
		}
		public int currentFloor
		{
			get
			{
				return this._currentFloor;
			}
			set
			{
				this._currentFloor = value;
				this._isDirty = true;
			}
		}
		public int accumulateExp
		{
			get
			{
				return this._accumulateExp;
			}
			set
			{
				this._accumulateExp = value;
				this._isDirty = true;
			}
		}
		public int remainTime
		{
			get
			{
				return this._remainTime;
			}
			set
			{
				this._remainTime = value;
				this._isDirty = true;
			}
		}
		public int currentRemainTime
		{
			get
			{
				return this._currentRemainTime;
			}
			set
			{
				this._currentRemainTime = value;
				this._isDirty = true;
			}
		}
		public int cleanOutAllTime
		{
			get
			{
				return this._cleanOutAllTime;
			}
			set
			{
				this._cleanOutAllTime = value;
				this._isDirty = true;
			}
		}
		public int cleanOutGold
		{
			get
			{
				return this._cleanOutGold;
			}
			set
			{
				this._cleanOutGold = value;
				this._isDirty = true;
			}
		}
		public bool tryAgainComplete
		{
			get
			{
				return this._tryAgainComplete;
			}
			set
			{
				this._tryAgainComplete = value;
				this._isDirty = true;
			}
		}
		public bool isInGame
		{
			get
			{
				return this._isInGame;
			}
			set
			{
				this._isInGame = value;
				this._isDirty = true;
			}
		}
		public bool isCleanOut
		{
			get
			{
				return this._isCleanOut;
			}
			set
			{
				this._isCleanOut = value;
				this._isDirty = true;
			}
		}
		public bool serverMultiplyingPower
		{
			get
			{
				return this._serverMultiplyingPower;
			}
			set
			{
				this._serverMultiplyingPower = value;
				this._isDirty = true;
			}
		}
		public DateTime LastDate
		{
			get
			{
				return this._lastDate;
			}
			set
			{
				this._lastDate = value;
				this._isDirty = true;
			}
		}
		public bool isValidDate()
		{
			return this._lastDate.Date < DateTime.Now.Date;
		}
	}
}
