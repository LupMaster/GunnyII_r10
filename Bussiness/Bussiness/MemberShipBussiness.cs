using log4net;
using SqlDataProvider.BaseClass;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Bussiness
{
    public class MemberShipBussiness : IDisposable
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected Sql_DbObject db;

        static MemberShipBussiness()
        {
        }

        public MemberShipBussiness()
        {
            this.db = new Sql_DbObject("AppConfig", "membershipDb");
        }

        public bool CheckUsername(string applicationname, string username, string password)
        {
            SqlParameter[] SqlParameters = new SqlParameter[4]
      {
        new SqlParameter("@ApplicationName", (object) applicationname),
        new SqlParameter("@UserName", (object) username),
        new SqlParameter("@password", (object) password),
        new SqlParameter("@UserId", SqlDbType.Int)
      };
            SqlParameters[3].Direction = ParameterDirection.Output;
            this.db.RunProcedure("Mem_Users_Accede", SqlParameters);
            int result = 0;
            int.TryParse(SqlParameters[3].Value.ToString(), out result);
            return result > 0;
        }

        public bool CreateUsername(string applicationname, string username, string password, string email, string passwordformat, string passwordsalt, bool usersex)
        {
            SqlParameter[] SqlParameters = new SqlParameter[8]
      {
        new SqlParameter("@ApplicationName", (object) applicationname),
        new SqlParameter("@UserName", (object) username),
        new SqlParameter("@password", (object) password),
        new SqlParameter("@email", (object) email),
        new SqlParameter("@PasswordFormat", (object) passwordformat),
        new SqlParameter("@PasswordSalt", (object) passwordsalt),
        new SqlParameter("@UserSex", (object) (int) (usersex ? 1 : 0)),
        new SqlParameter("@UserId", SqlDbType.Int)
      };
            SqlParameters[7].Direction = ParameterDirection.Output;
            bool flag = this.db.RunProcedure("Mem_Users_CreateUser", SqlParameters);
            if (flag)
                flag = (int)SqlParameters[7].Value > 0;
            return flag;
        }

        public void Dispose()
        {
            this.db.Dispose();
            GC.SuppressFinalize((object)this);
        }

        public bool ExistsUsername(string username)
        {
            SqlParameter[] SqlParameters = new SqlParameter[2]
      {
        new SqlParameter("@UserName", (object) username),
        new SqlParameter("@UserCOUNT", SqlDbType.Int)
      };
            SqlParameters[1].Direction = ParameterDirection.Output;
            this.db.RunProcedure("Mem_UserInfo_SearchName", SqlParameters);
            return (int)SqlParameters[1].Value > 0;
        }
    }
}
