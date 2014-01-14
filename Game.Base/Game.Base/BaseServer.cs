using log4net;
using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace Game.Base
{
    public class BaseServer
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly int SEND_BUFF_SIZE = 30720;
        protected readonly HybridDictionary _clients = new HybridDictionary();
        protected Socket _linstener;
        protected SocketAsyncEventArgs ac_event;

        public int ClientCount
        {
            get
            {
                return this._clients.Count;
            }
        }

        static BaseServer()
        {
        }

        public BaseServer()
        {
            this.ac_event = new SocketAsyncEventArgs();
            this.ac_event.Completed += new EventHandler<SocketAsyncEventArgs>(this.AcceptAsyncCompleted);
        }

        private void AcceptAsync()
        {
            try
            {
                if (this._linstener == null)
                    return;
                SocketAsyncEventArgs e = new SocketAsyncEventArgs();
                e.Completed += new EventHandler<SocketAsyncEventArgs>(this.AcceptAsyncCompleted);
                this._linstener.AcceptAsync(e);
            }
            catch (Exception ex)
            {
                BaseServer.log.Error((object)"AcceptAsync is error!", ex);
            }
        }

        private void AcceptAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            Socket connectedSocket = (Socket)null;
            try
            {
                connectedSocket = e.AcceptSocket;
                connectedSocket.SendBufferSize = BaseServer.SEND_BUFF_SIZE;
                BaseClient newClient = this.GetNewClient();
                try
                {
                    if (BaseServer.log.IsInfoEnabled)
                    {
                        string str = connectedSocket.Connected ? connectedSocket.RemoteEndPoint.ToString() : "socket disconnected";
                        BaseServer.log.Info((object)("Incoming connection from " + str));
                    }
                    lock (this._clients.SyncRoot)
                    {
                        this._clients.Add((object)newClient, (object)newClient);
                        newClient.Disconnected += new ClientEventHandle(this.client_Disconnected);
                    }
                    newClient.Connect(connectedSocket);
                    newClient.ReceiveAsync();
                }
                catch (Exception ex)
                {
                    BaseServer.log.ErrorFormat("create client failed:{0}", (object)ex);
                    newClient.Disconnect();
                }
            }
            catch
            {
                if (connectedSocket == null)
                    return;
                try
                {
                    connectedSocket.Close();
                }
                catch
                {
                }
            }
            finally
            {
                e.Dispose();
                this.AcceptAsync();
            }
        }

        private void client_Disconnected(BaseClient client)
        {
            client.Disconnected -= new ClientEventHandle(this.client_Disconnected);
            this.RemoveClient(client);
        }

        protected virtual BaseClient GetNewClient()
        {
            return new BaseClient(new byte[30720], new byte[30720]);
        }

        public virtual bool InitSocket(IPAddress ip, int port)
        {
            try
            {
                this._linstener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                this._linstener.Bind((EndPoint)new IPEndPoint(ip, port));
            }
            catch (Exception ex)
            {
                BaseServer.log.Error((object)"InitSocket", ex);
                return false;
            }
            return true;
        }

        public virtual bool Start()
        {
            if (this._linstener == null)
                return false;
            try
            {
                this._linstener.Listen(100);
                this.AcceptAsync();
                if (BaseServer.log.IsDebugEnabled)
                    BaseServer.log.Debug((object)"Server is now listening to incoming connections!");
            }
            catch (Exception ex)
            {
                if (BaseServer.log.IsErrorEnabled)
                    BaseServer.log.Error((object)"Start", ex);
                if (this._linstener != null)
                    this._linstener.Close();
                return false;
            }
            return true;
        }

        public virtual void Stop()
        {
            BaseServer.log.Debug((object)"Stopping server! - Entering method");
            try
            {
                if (this._linstener != null)
                {
                    Socket socket = this._linstener;
                    this._linstener = (Socket)null;
                    socket.Close();
                    BaseServer.log.Debug((object)"Server is no longer listening for incoming connections!");
                }
            }
            catch (Exception ex)
            {
                BaseServer.log.Error((object)"Stop", ex);
            }
            if (this._clients != null)
            {
                lock (this._clients.SyncRoot)
                {
                    try
                    {
                        BaseClient[] local_3 = new BaseClient[this._clients.Keys.Count];
                        this._clients.Keys.CopyTo((Array)local_3, 0);
                        foreach (BaseClient item_0 in local_3)
                            item_0.Disconnect();
                        BaseServer.log.Debug((object)"Stopping server! - Cleaning up client list!");
                    }
                    catch (Exception exception_0)
                    {
                        BaseServer.log.Error((object)"Stop", exception_0);
                    }
                }
            }
            BaseServer.log.Debug((object)"Stopping server! - End of method!");
        }

        public virtual void RemoveClient(BaseClient client)
        {
            lock (this._clients.SyncRoot)
                this._clients.Remove((object)client);
        }

        public BaseClient[] GetAllClients()
        {
            BaseClient[] baseClientArray;
            lock (this._clients.SyncRoot)
            {
                BaseClient[] local_2 = new BaseClient[this._clients.Count];
                this._clients.Keys.CopyTo((Array)local_2, 0);
                baseClientArray = local_2;
            }
            return baseClientArray;
        }

        public void Dispose()
        {
            this.ac_event.Dispose();
        }
    }
}
