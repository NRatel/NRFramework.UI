using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;

namespace NRFramework
{
    abstract public class TcpSock
    {
        private class Packet
        {
            public byte[] buffer;
            public int offset = 0;
        }

        private Socket m_Socket;

        //接收缓冲区应足够大，减少一条消息分多次接收的可能性。
        //这里取两字节能保存的最大长度
        //注意，这样仍然不能保证每次接收到的是完整消息，最终应做消息拼接。  
        private byte[] m_ReveiveBuffer = new byte[65536];  

        public Action onConnected;
        public Action<byte[], int> onSent;
        public Action<byte[], int> onReceived;
        public Action onDisconnected;
        public Action onClosed;
        public Action<SocketError> onSocketError;
        public Action<Exception> onException;

        public TcpSock Connect(string host, int port)
        {
            try
            {
                IPHostEntry iPHostEntry = Dns.GetHostEntry(host);
                IPAddress ipAddress = iPHostEntry.AddressList[0];
                m_Socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                m_Socket.NoDelay = true;
                //At the very minimum, you must pass the Socket to BeginConnect through the state parameter.   //TODO 这个真的必须吗？
                m_Socket.BeginConnect(ipAddress, port, new AsyncCallback(ConnectCallback), m_Socket);
            }
            catch (Exception e)
            {
                if (onException != null)
                {
                    onException(e);
                }
            }
            return this;
        }

        public void Send(byte[] data)
        {
            try
            {
                Packet packet = new Packet() { buffer = data, offset = 0 };
                m_Socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), packet);
            }
            catch (Exception e)
            {
                if (onException != null)
                {
                    onException(e);
                }
            }
        }

        public void Disconnect()
        {
            try
            {
                m_Socket.BeginDisconnect(false, new AsyncCallback(DisconnectCallback), m_Socket);
            }
            catch (Exception e)
            {
                if (onException != null)
                {
                    onException(e);
                }
            }
        }

        public void Close()
        {
            m_Socket.Shutdown(SocketShutdown.Both);
            m_Socket.Close();
            if (onClosed != null)
            {
                onClosed();
            }
        }

        private void ConnectCallback(IAsyncResult result)
        {
            try
            {
                m_Socket.EndConnect(result);
                if (onConnected != null)
                {
                    onConnected();
                }
                m_Socket.BeginReceive(m_ReveiveBuffer, 0, m_ReveiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            }
            catch (Exception e)
            {
                if (onException != null)
                {
                    onException(e);
                }
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                SocketError errorCode;
                int receivedSize = m_Socket.EndReceive(result, out errorCode);
                if (errorCode == SocketError.Success)
                {
                    if (receivedSize > 0)
                    {
                        if (onReceived != null)
                        {
                            byte[] buffer = new byte[receivedSize];
                            Array.Copy(m_ReveiveBuffer, 0, buffer, 0, receivedSize);
                            if (onReceived != null)
                            {
                                onReceived(buffer, receivedSize);
                            }
                        }
                        m_Socket.BeginReceive(m_ReveiveBuffer, 0, m_ReveiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
                    }
                    else
                    {
                        //If the remote host shuts down the Socket connection with the Shutdown method, and all available data has been received, the EndReceive method will complete immediately and return zero bytes. 
                        //被动关闭
                        Close();
                    }
                }
                else
                {
                    if (onSocketError != null)
                    {
                        onSocketError(errorCode);
                    }
                }
            }
            catch (Exception e)
            {
                if (onException != null)
                {
                    onException(e);
                }
            }
        }

        private void SendCallback(IAsyncResult result)
        {
            Packet packet = (Packet)result.AsyncState;
            try
            {
                SocketError errorCode;
                int sentSize = m_Socket.EndSend(result, out errorCode);
                if (errorCode == SocketError.Success)
                {
                    if (onSent != null)
                    {
                        onSent(packet.buffer, sentSize);
                    }

                    //If you are using a connectionless protocol, EndSend will block until the datagram is sent.
                    //If you are using a connection-oriented protocol, EndSend will block until some of the buffer was sent. 
                    //If the return value from EndSend indicates that the buffer was not completely sent, call the BeginSend method again, modifying the buffer to hold the unsent data.
                    if (sentSize < packet.buffer.Length)
                    {
                        packet.offset += sentSize;
                        m_Socket.BeginSend(packet.buffer, packet.offset, packet.buffer.Length - sentSize, SocketFlags.None, new AsyncCallback(SendCallback), packet);
                    }
                }
                else
                {
                    if (onSocketError != null)
                    {
                        onSocketError(errorCode);
                    }
                }
            }
            catch (Exception e)
            {
                if (onException != null)
                {
                    onException(e);
                }
            }
        }

        private void DisconnectCallback(IAsyncResult result)
        {
            try
            {
                m_Socket.EndDisconnect(result);
                if (onDisconnected != null)
                {
                    onDisconnected();
                }
            }
            catch (Exception e)
            {
                if (onException != null)
                {
                    onException(e);
                }
            }
        }
    }
}