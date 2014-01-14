using System;
using System.Data;
using System.Data.SqlClient;

namespace Bussiness
{
    public class CookieInfoBussiness : BaseBussiness
    {
        public bool GetFromDbByUser(string bdSigUser, ref string bdSigPortrait, ref string bdSigSessionKey)
        {
            SqlDataReader ResultDataReader = (SqlDataReader)null;
            bool flag;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[1]
        {
          new SqlParameter("@BdSigUser", (object) bdSigUser)
        };
                this.db.GetReader(ref ResultDataReader, "SP_Cookie_Info_QueryByUser", SqlParameters);
                while (ResultDataReader.Read())
                {
                    bdSigPortrait = ResultDataReader["BdSigPortrait"] == null ? "" : ResultDataReader["BdSigPortrait"].ToString();
                    bdSigSessionKey = ResultDataReader["BdSigSessionKey"] == null ? "" : ResultDataReader["BdSigSessionKey"].ToString();
                }
                flag = !string.IsNullOrEmpty(bdSigPortrait) && !string.IsNullOrEmpty(bdSigSessionKey);
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
                flag = false;
            }
            finally
            {
                if (ResultDataReader != null && !ResultDataReader.IsClosed)
                    ResultDataReader.Close();
            }
            return flag;
        }

        public bool AddCookieInfo(string bdSigUser, string bdSigPortrait, string bdSigSessionKey)
        {
            bool flag = false;
            try
            {
                SqlParameter[] SqlParameters = new SqlParameter[4]
        {
          new SqlParameter("@BdSigUser", (object) bdSigUser),
          new SqlParameter("@BdSigPortrait", (object) bdSigPortrait),
          new SqlParameter("@BdSigSessionKey", (object) bdSigSessionKey),
          new SqlParameter("@Result", SqlDbType.Int)
        };
                SqlParameters[3].Direction = ParameterDirection.ReturnValue;
                this.db.RunProcedure("SP_Cookie_Info_Insert", SqlParameters);
                flag = (int)SqlParameters[3].Value == 0;
            }
            catch (Exception ex)
            {
                if (BaseBussiness.log.IsErrorEnabled)
                    BaseBussiness.log.Error((object)"Init", ex);
            }
            return flag;
        }
    }
}
