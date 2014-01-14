using Bussiness;
using Bussiness.Interface;
using log4net;
using Road.Flash;
using System;
using System.Configuration;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Xml.Linq;
namespace Tank.Request
{
    [WebService(Namespace = "http://tempuri.org/"), WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class RenameConsortiaName : IHttpHandler
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        public void ProcessRequest(HttpContext context)
        {
            LanguageMgr.Setup(ConfigurationManager.AppSettings["ReqPath"]);
            bool flag = false;
            string translation = LanguageMgr.GetTranslation("Tank.Request.RenameConsortiaName.Fail1", new object[0]);
            System.Xml.Linq.XElement xElement = new System.Xml.Linq.XElement("Result");
            try
            {
                BaseInterface.CreateInterface();
                string text = context.Request["p"];
                if (context.Request["site"] != null)
                {
                    HttpUtility.UrlDecode(context.Request["site"]);
                }
                string arg_82_0 = context.Request.UserHostAddress;
                if (!string.IsNullOrEmpty(text))
                {
                    byte[] array = CryptoHelper.RsaDecryt2(StaticFunction.RsaCryptor, text);
                    string[] array2 = System.Text.Encoding.UTF8.GetString(array, 7, array.Length - 7).Split(new char[]
					{
						','
					});
                    if (array2.Length == 5)
                    {
                        string text2 = array2[0];
                        string pass = array2[1];
                        string pass2 = array2[2];
                        string nickName = array2[3];
                        string consortiaName = array2[4];
                        if (PlayerManager.Login(text2, pass))
                        {
                            using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
                            {
                                if (consortiaBussiness.RenameConsortiaName(text2, nickName, consortiaName, ref translation))
                                {
                                    PlayerManager.Update(text2, pass2);
                                    flag = true;
                                    translation = LanguageMgr.GetTranslation("Tank.Request.RenameConsortiaName.Success", new object[0]);
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception exception)
            {
                RenameConsortiaName.log.Error("RenameConsortiaName", exception);
                flag = false;
                translation = LanguageMgr.GetTranslation("Tank.Request.RenameConsortiaName.Fail2", new object[0]);
            }
            xElement.Add(new System.Xml.Linq.XAttribute("value", flag));
            xElement.Add(new System.Xml.Linq.XAttribute("message", translation));
            context.Response.ContentType = "text/plain";
            context.Response.Write(xElement.ToString(false));
        }
    }
}
