using log4net;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace Game.Base
{
    public class BaseConnector : BaseClient
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly int RECONNECT_INTERVAL = 10000;
        private SocketAsyncEventArgs e;
        private IPEndPoint _remoteEP;
        private bool _autoReconnect;
        private System.Threading.Timer timer;

        public IPEndPoint RemoteEP
        {
            get
            {
                return this._remoteEP;
            }
        }

        static BaseConnector()
        {
        }

        public BaseConnector(string ip, int port, bool autoReconnect, byte[] readBuffer, byte[] sendBuffer)
            : base(readBuffer, sendBuffer)
        {
            this._remoteEP = new IPEndPoint(IPAddress.Parse(ip), port);
            this._autoReconnect = autoReconnect;
            this.e = new SocketAsyncEventArgs();
        }

        public bool Connect()
        {
            try
            {
                this.m_sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this.m_readBufEnd = 0;
                this.m_sock.Connect((EndPoint)this._remoteEP);
                BaseConnector.log.InfoFormat("Connected to {0}", (object)this._remoteEP);
            }
            catch
            {
                BaseConnector.log.ErrorFormat("Connect {0} failed!", (object)this._remoteEP);
                this.m_sock = (Socket)null;
                return false;
            }
            this.OnConnect();
            this.ReceiveAsync();
            return true;
        }

        private void TryReconnect()
        {
            if (this.Connect())
            {
                if (this.timer != null)
                {
                    this.timer.Dispose();
                    this.timer = (System.Threading.Timer)null;
                }
                this.ReceiveAsync();
            }
            else
            {
                BaseConnector.log.ErrorFormat("Reconnect {0} failed:", (object)this._remoteEP);
                BaseConnector.log.ErrorFormat("Retry after {0} ms!", (object)BaseConnector.RECONNECT_INTERVAL);
                if (this.timer == null)
                    this.timer = new System.Threading.Timer(new TimerCallback(BaseConnector.RetryTimerCallBack), (object)this, -1, -1);
                this.timer.Change(BaseConnector.RECONNECT_INTERVAL, -1);
            }
        }

        private static void RetryTimerCallBack(object target)
        {
            BaseConnector baseConnector = target as BaseConnector;
            if (baseConnector != null)
                baseConnector.TryReconnect();
            else
                BaseConnector.log.Error((object)"BaseConnector retryconnect timer return NULL!");
        }
    }
}
