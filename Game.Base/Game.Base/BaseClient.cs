using Game.Base.Packets;
using log4net;
using System;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

namespace Game.Base
{
    public class BaseClient
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public bool IsClientPacketSended = true;
        protected Socket m_sock;
        protected byte[] m_readBuffer;
        protected int m_readBufEnd;
        private SocketAsyncEventArgs rc_event;
        private int m_isConnected;
        public int numPacketProcces;
        protected byte[] m_sendBuffer;
        private bool m_encryted;
        private bool m_strict;
        private bool m_asyncPostSend;
        public StreamProcessor m_processor;
        public byte[] RECEIVE_KEY;
        public byte[] SEND_KEY;

        public Socket Socket
        {
            get
            {
                return this.m_sock;
            }
            set
            {
                this.m_sock = value;
            }
        }

        public byte[] PacketBuf
        {
            get
            {
                return this.m_readBuffer;
            }
        }

        public bool IsConnected
        {
            get
            {
                return this.m_isConnected == 1;
            }
        }

        public int PacketBufSize
        {
            get
            {
                return this.m_readBufEnd;
            }
            set
            {
                this.m_readBufEnd = value;
            }
        }

        public string TcpEndpoint
        {
            get
            {
                Socket socket = this.m_sock;
                if (socket != null && socket.Connected && socket.RemoteEndPoint != null)
                    return socket.RemoteEndPoint.ToString();
                else
                    return "not connected";
            }
        }

        public byte[] SendBuffer
        {
            get
            {
                return this.m_sendBuffer;
            }
        }

        public bool Encryted
        {
            get
            {
                return this.m_encryted;
            }
            set
            {
                this.m_encryted = value;
            }
        }

        public bool Strict
        {
            get
            {
                return this.m_strict;
            }
            set
            {
                this.m_strict = value;
            }
        }

        public bool AsyncPostSend
        {
            get
            {
                return this.m_asyncPostSend;
            }
            set
            {
                this.m_asyncPostSend = value;
            }
        }

        public event ClientEventHandle Connected;

        public event ClientEventHandle Disconnected;

        static BaseClient()
        {
        }

        public BaseClient(byte[] readBuffer, byte[] sendBuffer)
        {
            this.m_readBuffer = readBuffer;
            this.m_sendBuffer = sendBuffer;
            this.m_readBufEnd = 0;
            this.rc_event = new SocketAsyncEventArgs();
            this.rc_event.Completed += new EventHandler<SocketAsyncEventArgs>(this.RecvEventCallback);
            this.m_processor = new StreamProcessor(this);
            this.m_encryted = false;
            this.m_strict = true;
        }

        public virtual void OnRecv(int num_bytes)
        {
            this.m_processor.ReceiveBytes(num_bytes);
        }

        public virtual void OnRecvPacket(GSPacketIn pkg)
        {
        }

        protected virtual void OnConnect()
        {
            if (Interlocked.Exchange(ref this.m_isConnected, 1) != 0 || this.Connected == null)
                return;
            this.Connected(this);
        }

        protected virtual void OnDisconnect()
        {
            if (this.Disconnected == null)
                return;
            this.Disconnected(this);
        }

        public void SetFsm(int adder, int muliter)
        {
            this.m_processor.SetFsm(adder, muliter);
        }

        public void ReceiveAsync()
        {
            this.ReceiveAsyncImp(this.rc_event);
        }

        private void ReceiveAsyncImp(SocketAsyncEventArgs e)
        {
            if (this.m_sock != null && this.m_sock.Connected)
            {
                int length = this.m_readBuffer.Length;
                if (this.m_readBufEnd >= length)
                {
                    if (BaseClient.log.IsErrorEnabled)
                    {
                        BaseClient.log.Error((object)(this.TcpEndpoint + " disconnected because of buffer overflow!"));
                        BaseClient.log.Error((object)string.Concat(new object[4]
            {
              (object) "m_pBufEnd=",
              (object) this.m_readBufEnd,
              (object) "; buf size=",
              (object) length
            }));
                        BaseClient.log.Error((object)this.m_readBuffer);
                    }
                    this.Disconnect();
                }
                else
                {
                    e.SetBuffer(this.m_readBuffer, this.m_readBufEnd, length - this.m_readBufEnd);
                    if (this.m_sock.ReceiveAsync(e))
                        return;
                    this.RecvEventCallback((object)this.m_sock, e);
                }
            }
            else
                this.Disconnect();
        }

        private void RecvEventCallback(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                int bytesTransferred = e.BytesTransferred;
                if (bytesTransferred > 0)
                {
                    this.OnRecv(bytesTransferred);
                    this.ReceiveAsyncImp(e);
                }
                else
                {
                    BaseClient.log.InfoFormat("Disconnecting client ({0}), received bytes={1}", (object)this.TcpEndpoint, (object)bytesTransferred);
                    this.Disconnect();
                }
            }
            catch (Exception ex)
            {
                BaseClient.log.ErrorFormat("{0} RecvCallback:{1}", (object)this.TcpEndpoint, (object)ex);
                this.Disconnect();
            }
        }

        public virtual void SendTCP(GSPacketIn pkg)
        {
            this.m_processor.SendTCP(pkg);
        }

        public bool SendAsync(SocketAsyncEventArgs e)
        {
            int tickCount = Environment.TickCount;
            BaseClient.log.Debug((object)string.Format("Send To ({0}) {1} bytes", (object)this.TcpEndpoint, (object)e.Count));
            bool flag = true;
            if (this.m_sock.Connected)
                flag = this.m_sock.SendAsync(e);
            int num = Environment.TickCount - tickCount;
            if (num > 100)
                BaseClient.log.WarnFormat("AsyncTcpSendCallback.BeginSend took {0}ms! (TCP to client: {1})", (object)num, (object)this.TcpEndpoint);
            return flag;
        }

        protected void CloseConnections()
        {
            if (this.m_sock == null)
                return;
            try
            {
                this.m_sock.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }
            try
            {
                this.m_sock.Close();
            }
            catch
            {
            }
        }

        public virtual bool Connect(Socket connectedSocket)
        {
            this.m_sock = connectedSocket;
            if (!this.m_sock.Connected)
                return false;
            if (Interlocked.Exchange(ref this.m_isConnected, 1) == 0)
                this.OnConnect();
            return true;
        }

        public virtual void Disconnect()
        {
            try
            {
                if (Interlocked.Exchange(ref this.m_isConnected, 0) != 1)
                    return;
                this.CloseConnections();
                this.OnDisconnect();
                this.rc_event.Dispose();
                this.m_processor.Dispose();
            }
            catch (Exception ex)
            {
                if (!BaseClient.log.IsErrorEnabled)
                    return;
                BaseClient.log.Error((object)"Exception", ex);
            }
        }

        public virtual void DisplayMessage(string msg)
        {
        }

        public virtual void resetKey()
        {
            this.RECEIVE_KEY = StreamProcessor.cloneArrary(StreamProcessor.KEY, 8);
            this.SEND_KEY = StreamProcessor.cloneArrary(StreamProcessor.KEY, 8);
        }

        public virtual void setKey(byte[] data)
        {
            for (int index = 0; index < 8; ++index)
            {
                this.RECEIVE_KEY[index] = data[index];
                this.SEND_KEY[index] = data[index];
            }
        }
    }
}
