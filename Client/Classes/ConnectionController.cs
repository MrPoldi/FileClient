using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace Client.Classes
{
    public class ConnectionController
    {
        private IPAddress ipAddr;
        private IPEndPoint endPoint;
        private Socket sender;
        private bool isConnected;

        public bool IsConnected
        {
            get { return isConnected; }
        }

        //try to connect a servert and get list of files IN PROGRESS
        internal void GetListOfFiles()
        {
            try
            {
                sender.Connect(endPoint);
                isConnected = true;
                //sender.Shutdown(SocketShutdown.Both);
                sender.Close();
            }

            catch(SocketException se)
            {
                isConnected = false;
                MessageBox.Show("SocketException " + se.ToString());
            }
        }

        //send local file to server and receive refreshed list of files
        internal void SendFile()
        {
            throw new NotImplementedException();
        }

        //download chosen file from server list IN PROGRESS
        internal void GetFile()
        {
            throw new NotImplementedException();
        }

        //This method checks if IP/port are valid
        internal bool CheckIP(string ip, int port)
        {
            try
            {
                ipAddr = IPAddress.Parse(ip);
                endPoint = new IPEndPoint(ipAddr, port);
                sender = new Socket(ipAddr.AddressFamily,
                                    SocketType.Stream,
                                    ProtocolType.Tcp);
                return true;
            }
            catch
            { 
                MessageBox.Show("Enter correct port and IP address");
                isConnected = false;
                return false;
            }
        }

    }
}