using Bussiness.CenterService;
using SqlDataProvider.Data;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Bussiness
{
    public class ManageBussiness : BaseBussiness
    {
        public int KitoffUserByUserName(string name, string msg)
        {
            int num = 1;
            using (PlayerBussiness playerBussiness = new PlayerBussiness())
            {
                PlayerInfo singleByUserName = playerBussiness.GetUserSingleByUserName(name);
                if (singleByUserName == null)
                    return 2;
                num = this.KitoffUser(singleByUserName.ID, msg);
            }
            return num;
        }

        public int KitoffUserByNickName(string name, string msg)
        {
            int num = 1;
            using (PlayerBussiness playerBussiness = new PlayerBussiness())
            {
                PlayerInfo singleByNickName = playerBussiness.GetUserSingleByNickName(name);
                if (singleByNickName == null)
                    return 2;
                num = this.KitoffUser(singleByNickName.ID, msg);
            }
            return num;
        }

        public int KitoffUser(int id, string msg)
        {
            int num;
            try
            {
                using (CenterServiceClient centerServiceClient = new CenterServiceClient())
                    num = !centerServiceClient.KitoffUser(id, msg) ? 3 : 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"KitoffUser", ex);
                num = 1;
            }
            return num;
        }

        public bool SystemNotice(string msg)
        {
            bool flag = false;
            try
            {
                if (!string.IsNullOrEmpty(msg))
                {
                    using (CenterServiceClient centerServiceClient = new CenterServiceClient())
                    {
                        if (centerServiceClient.SystemNotice(msg))
                            flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"SystemNotice", ex);
            }
            return flag;
        }

        private bool ForbidPlayer(string userName, string nickName, int userID, DateTime forbidDate, bool isExist)
        {
            return this.ForbidPlayer(userName, nickName, userID, forbidDate, isExist, "");
        }

        private bool ForbidPlayer(string userName, string nickName, int userID, DateTime forbidDate, bool isExist, string ForbidReason)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[6]
        {
          new SqlParameter("@UserName", (object) userName),
          new SqlParameter("@NickName", (object) nickName),
          new SqlParameter("@UserID", (object) userID),
          null,
          null,
          null
        };
                SqlParameters[2].Direction = ParameterDirection.InputOutput;
                SqlParameters[3] = new SqlParameter("@ForbidDate", (object)forbidDate);
                SqlParameters[4] = new SqlParameter("@IsExist", (object)(int)(isExist ? 1 : 0));
                SqlParameters[5] = new SqlParameter("@ForbidReason", (object)ForbidReason);
                this.db.RunProcedure("SP_Admin_ForbidUser", SqlParameters);
                userID = (int)SqlParameters[2].Value;
                if (userID > 0)
                {
                    flag = true;
                    if (!isExist)
                        this.KitoffUser(userID, "You are kicking out by GM!!");
                }
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }

        public bool ForbidPlayerByUserName(string userName, DateTime date, bool isExist)
        {
            return this.ForbidPlayer(userName, "", 0, date, isExist);
        }

        public bool ForbidPlayerByNickName(string nickName, DateTime date, bool isExist)
        {
            return this.ForbidPlayer("", nickName, 0, date, isExist);
        }

        public bool ForbidPlayerByUserID(int userID, DateTime date, bool isExist)
        {
            return this.ForbidPlayer("", "", userID, date, isExist);
        }

        public bool ForbidPlayerByUserName(string userName, DateTime date, bool isExist, string ForbidReason)
        {
            return this.ForbidPlayer(userName, "", 0, date, isExist, ForbidReason);
        }

        public bool ForbidPlayerByNickName(string nickName, DateTime date, bool isExist, string ForbidReason)
        {
            return this.ForbidPlayer("", nickName, 0, date, isExist, ForbidReason);
        }

        public bool ForbidPlayerByUserID(int userID, DateTime date, bool isExist, string ForbidReason)
        {
            return this.ForbidPlayer("", "", userID, date, isExist, ForbidReason);
        }

        public bool ReLoadServerList()
        {
            bool flag = false;
            try
            {
                using (CenterServiceClient centerServiceClient = new CenterServiceClient())
                {
                    if (centerServiceClient.ReLoadServerList())
                        flag = true;
                }
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"ReLoadServerList", ex);
            }
            return flag;
        }

        public int GetConfigState(int type)
        {
            int num = 2;
            try
            {
                using (CenterServiceClient centerServiceClient = new CenterServiceClient())
                    return centerServiceClient.GetConfigState(type);
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"GetConfigState", ex);
            }
            return num;
        }

        public bool UpdateConfigState(int type, bool state)
        {
            bool flag = false;
            try
            {
                using (CenterServiceClient centerServiceClient = new CenterServiceClient())
                    return centerServiceClient.UpdateConfigState(type, state);
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"UpdateConfigState", ex);
            }
            return flag;
        }

        public bool Reload(string type)
        {
            bool flag = false;
            try
            {
                using (CenterServiceClient centerServiceClient = new CenterServiceClient())
                    return centerServiceClient.Reload(type);
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Reload", ex);
            }
            return flag;
        }
    }
}
