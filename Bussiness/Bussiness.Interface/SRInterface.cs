using Bussiness.WebLogin;
using System;

namespace Bussiness.Interface
{
    public class SRInterface : BaseInterface
    {
        public override bool GetUserSex(string name)
        {
            bool flag;
            try
            {
                flag = new PassPortSoapClient().Get_UserSex(string.Empty, name).Value;
            }
            catch (Exception ex)
            {
                BaseInterface.log.Error((object)"获取性别失败", ex);
                flag = true;
            }
            return flag;
        }
    }
}
