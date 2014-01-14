// Type: Game.Base.Packets.StreamProcessor
// Assembly: Game.Base, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 2F116361-6D58-476D-84B7-B30C3F614451
// Assembly location: C:\Users\Jhon\AppData\Local\Temp\Rar$DIa0.869\Game.Base.dll

using Game.Base;
using log4net;
using Road.Base.Packets;
using System;
using System.Collections;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Game.Base.Packets
{
    public class StreamProcessor
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static byte[] KEY = new byte[8]
    {
      (byte) 174,
      (byte) 191,
      (byte) 86,
      (byte) 120,
      (byte) 171,
      (byte) 205,
      (byte) 239,
      (byte) 241
    };
        protected readonly BaseClient m_client;
        private FSM send_fsm;
        private FSM receive_fsm;
        private SocketAsyncEventArgs send_event;
        protected byte[] m_tcpSendBuffer;
        protected Queue m_tcpQueue;
        protected bool m_sendingTcp;
        protected int m_firstPkgOffset;
        protected int m_sendBufferLength;

        static StreamProcessor()
        {
        }

        public StreamProcessor(BaseClient client)
        {
            this.m_client = client;
            this.m_client.resetKey();
            this.m_tcpSendBuffer = client.SendBuffer;
            this.m_tcpQueue = new Queue(256);
            this.send_event = new SocketAsyncEventArgs();
            this.send_event.UserToken = (object)this;
            this.send_event.Completed += new EventHandler<SocketAsyncEventArgs>(StreamProcessor.AsyncTcpSendCallback);
            this.send_event.SetBuffer(this.m_tcpSendBuffer, 0, 0);
            this.send_fsm = new FSM(2059198199, 1501, "send_fsm");
            this.receive_fsm = new FSM(2059198199, 1501, "receive_fsm");
        }

        public void SetFsm(int adder, int muliter)
        {
            this.send_fsm.Setup(adder, muliter);
            this.receive_fsm.Setup(adder, muliter);
        }

        public void SendTCP(GSPacketIn packet)
        {
            packet.WriteHeader();
            packet.Offset = 0;
            if (!this.m_client.Socket.Connected)
                return;
            try
            {
                Statistics.BytesOut += (long)packet.Length;
                ++Statistics.PacketsOut;
               // if (StreamProcessor.log.get_IsDebugEnabled())
                    StreamProcessor.log.Debug((object)Marshal.ToHexDump(string.Format("Send Pkg to {0} :", (object)this.m_client.TcpEndpoint), packet.Buffer, 0, packet.Length));
                lock (this.m_tcpQueue.SyncRoot)
                {
                    this.m_tcpQueue.Enqueue((object)packet);
                    if (this.m_sendingTcp)
                        return;
                    this.m_sendingTcp = true;
                }
                if (this.m_client.AsyncPostSend)
                    ThreadPool.QueueUserWorkItem(new WaitCallback(StreamProcessor.AsyncSendTcpImp), (object)this);
                else
                    StreamProcessor.AsyncTcpSendCallback((object)this, this.send_event);
            }
            catch (Exception ex)
            {
                StreamProcessor.log.Error((object)"SendTCP", ex);
                StreamProcessor.log.WarnFormat("It seems <{0}> went linkdead. Closing connection. (SendTCP, {1}: {2})", (object)this.m_client, (object)ex.GetType(), (object)ex.Message);
                this.m_client.Disconnect();
            }
        }

        private static void AsyncSendTcpImp(object state)
        {
            StreamProcessor streamProcessor = state as StreamProcessor;
            BaseClient baseClient = streamProcessor.m_client;
            try
            {
                StreamProcessor.AsyncTcpSendCallback((object)streamProcessor, streamProcessor.send_event);
            }
            catch (Exception ex)
            {
                StreamProcessor.log.Error((object)"AsyncSendTcpImp", ex);
                baseClient.Disconnect();
            }
        }

        private static void AsyncTcpSendCallback(object sender, SocketAsyncEventArgs e)
        {
            StreamProcessor streamProcessor = (StreamProcessor)e.UserToken;
            BaseClient baseClient = streamProcessor.m_client;
            try
            {
                Queue queue = streamProcessor.m_tcpQueue;
                if (queue != null && baseClient.Socket.Connected)
                {
                    int bytesTransferred = e.BytesTransferred;
                    byte[] dst = streamProcessor.m_tcpSendBuffer;
                    int num = 0;
                    if (bytesTransferred != e.Count && streamProcessor.m_sendBufferLength > bytesTransferred)
                    {
                        num = streamProcessor.m_sendBufferLength - bytesTransferred;
                        Array.Copy((Array)dst, bytesTransferred, (Array)dst, 0, num);
                    }
                    e.SetBuffer(0, 0);
                    int offset = streamProcessor.m_firstPkgOffset;
                    lock (queue.SyncRoot)
                    {
                        if (queue.Count > 0)
                        {
                            PacketIn local_8;
                            do
                            {
                                local_8 = (PacketIn)queue.Peek();
                                int local_9;
                                if (baseClient.Encryted)
                                {
                                    ++local_8.m_loop;
                                    local_9 = local_8.CopyTo3(dst, num, offset, baseClient.SEND_KEY, ref baseClient.numPacketProcces);
                                }
                                else
                                    local_9 = local_8.CopyTo(dst, num, offset);
                                offset += local_9;
                                num += local_9;
                                if (local_8.Length <= offset)
                                {
                                    queue.Dequeue();
                                    offset = 0;
                                    if (baseClient.Encryted)
                                    {
                                        streamProcessor.send_fsm.UpdateState();
                                        local_8.isSended = true;
                                    }
                                }
                                if (dst.Length != num)
                                {
                                    if (local_8.m_loop > 12)
                                        goto label_17;
                                }
                                else
                                    goto label_16;
                            }
                            while (queue.Count > 0);
                            goto label_18;
                        label_16:
                            local_8.m_loop = 0;
                            goto label_18;
                        label_17:
                            local_8.m_loop = 0;
                        }
                    label_18:
                        streamProcessor.m_firstPkgOffset = offset;
                        if (num <= 0)
                        {
                            streamProcessor.m_sendingTcp = false;
                            return;
                        }
                    }
                    streamProcessor.m_sendBufferLength = num;
                    e.SetBuffer(0, num);
                    if (!baseClient.SendAsync(e))
                        StreamProcessor.AsyncTcpSendCallback(sender, e);
                }
            }
            catch (Exception ex)
            {
                StreamProcessor.log.Error((object)"AsyncTcpSendCallback", ex);
                StreamProcessor.log.WarnFormat("It seems <{0}> went linkdead. Closing connection. (SendTCP, {1}: {2})", (object)baseClient, (object)ex.GetType(), (object)ex.Message);
                baseClient.Disconnect();
            }
        }

        public void ReceiveBytes(int numBytes)
        {
            Monitor.Enter((object)this);
            try
            {
                byte[] packetBuf = this.m_client.PacketBuf;
                int num1 = this.m_client.PacketBufSize + numBytes;
                if (num1 < 20)
                {
                    this.m_client.PacketBufSize = num1;
                }
                else
                {
                    this.m_client.PacketBufSize = 0;
                    int index = 0;
                    int count;
                    int length;
                    do
                    {
                        count = 0;
                        if (this.m_client.Encryted)
                        {
                            int num2 = this.receive_fsm.count;
                            byte[] numArray1 = StreamProcessor.cloneArrary(this.m_client.RECEIVE_KEY, 8);
                            for (; index + 4 < num1; ++index)
                            {
                                byte[] numArray2 = StreamProcessor.decryptBytes(packetBuf, index, 8, numArray1);
                                if (((int)numArray2[0] << 8) + (int)numArray2[1] == 29099)
                                {
                                    count = ((int)numArray2[2] << 8) + (int)numArray2[3];
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (; index + 4 < num1; ++index)
                            {
                                if (((int)packetBuf[index] << 8) + (int)packetBuf[index + 1] == 29099)
                                {
                                    count = ((int)packetBuf[index + 2] << 8) + (int)packetBuf[index + 3];
                                    break;
                                }
                            }
                        }
                        if ((count == 0 || count >= 20) && count <= 8192)
                        {
                            length = num1 - index;
                            if (length >= count && count != 0)
                            {
                                GSPacketIn pkg = new GSPacketIn(new byte[8192], 8192);
                                if (this.m_client.Encryted)
                                    pkg.CopyFrom3(packetBuf, index, 0, count, this.m_client.RECEIVE_KEY);
                                else
                                    pkg.CopyFrom(packetBuf, index, 0, count);
                                pkg.ReadHeader();
                                StreamProcessor.log.Debug((object)Marshal.ToHexDump("Recieve Packet:", pkg.Buffer, 0, count));
                                try
                                {
                                    this.m_client.OnRecvPacket(pkg);
                                }
                                catch (Exception ex)
                                {
                                   // if (StreamProcessor.log.get_IsErrorEnabled())
                                        StreamProcessor.log.Error((object)"HandlePacket(pak)", ex);
                                }
                                index += count;
                            }
                            else
                                goto label_27;
                        }
                        else
                            goto label_25;
                    }
                    while (num1 - 1 > index);
                    goto label_28;
                label_25:
                    StreamProcessor.log.Error((object)("packetLength:" + (object)count + ",GSPacketIn.HDR_SIZE:" + (string)(object)20 + ",offset:" + (string)(object)index + ",bufferSize:" + (string)(object)num1 + ",numBytes:" + (string)(object)numBytes));
                    StreamProcessor.log.ErrorFormat("Err pkg from {0}:", (object)this.m_client.TcpEndpoint);
                    this.m_client.PacketBufSize = 0;
                    if (this.m_client.Strict)
                    {
                        this.m_client.Disconnect();
                        goto label_30;
                    }
                    else
                        goto label_30;
                label_27:
                    Array.Copy((Array)packetBuf, index, (Array)packetBuf, 0, length);
                    this.m_client.PacketBufSize = length;
                label_28:
                    if (num1 - 1 == index)
                    {
                        packetBuf[0] = packetBuf[index];
                        this.m_client.PacketBufSize = 1;
                    }
                label_30: ;
                }
            }
            finally
            {
                Monitor.Exit((object)this);
            }
        }

        public static byte[] cloneArrary(byte[] arr, int length = 8)
        {
            byte[] numArray = new byte[length];
            for (int index = 0; index < length; ++index)
                numArray[index] = arr[index];
            return numArray;
        }

        public static string PrintArray(byte[] arr, int length = 8)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            for (int index = 0; index < length; ++index)
                stringBuilder.AppendFormat("{0} ", (object)arr[index]);
            stringBuilder.Append("]");
            return ((object)stringBuilder).ToString();
        }

        public static string PrintArray(byte[] arr, int first, int length)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("[");
            for (int index = first; index < first + length; ++index)
                stringBuilder.AppendFormat("{0} ", (object)arr[index]);
            stringBuilder.Append("]");
            return ((object)stringBuilder).ToString();
        }

        public static byte[] decryptBytes(byte[] param1, int curOffset, int param2, byte[] param3)
        {
            byte[] numArray = new byte[param2];
            for (int index = 0; index < param2; ++index)
                numArray[index] = param1[index];
            for (int index = 0; index < param2; ++index)
            {
                if (index > 0)
                {
                    param3[index % 8] = (byte)((int)param3[index % 8] + (int)param1[curOffset + index - 1] ^ index);
                    numArray[index] = (byte)((uint)param1[curOffset + index] - (uint)param1[curOffset + index - 1] ^ (uint)param3[index % 8]);
                }
                else
                    numArray[0] = (byte)((uint)param1[curOffset] ^ (uint)param3[0]);
            }
            return numArray;
        }

        public void Dispose()
        {
            this.send_event.Dispose();
            this.m_tcpQueue.Clear();
        }
    }
}
