using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Text;
using System.IO;

namespace Client.Classes
{
    public class ConnectionController
    {
        private long rcv;
        private byte[] buffer;
        private string opt;
        private IPAddress ipAddr;
        private IPEndPoint endPoint;
        private Socket sender;
        private long fileToSendSize;
        private string fileTSPath;
        private string fileTSName;
        private bool isConnected;
        private string fileToDownload;

        public long FileTSSize
        {
            set { fileToSendSize = value; }
        }

        public string FileTSPath
        {
            set { fileTSPath = value; }
        }

        public string FileTSName
        {
            set { fileTSName = value; }    
        }

        public string FileToDownload
        {
            set { fileToDownload = value; }
        }

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
            try
            {
                opt = "a";
                buffer = Encoding.ASCII.GetBytes(opt);
                sender.Send(buffer);
                buffer = Encoding.ASCII.GetBytes(fileTSName);
                sender.Send(buffer);
                sender.Receive(buffer, sizeof(long), SocketFlags.None);
                buffer = BitConverter.GetBytes(fileToSendSize);
                sender.Send(buffer);
                sender.Receive(buffer, sizeof(long), SocketFlags.None);

                try
                {
                    using (StreamReader sr = new StreamReader(this.fileTSPath))
                    {
                        String line = sr.ReadLine();
                        while (line != null)
                        {
                            buffer = Encoding.ASCII.GetBytes(line);
                            sender.Send(buffer);
                            line = sr.ReadLine() + "\n";
                        }
                    }
                }
                catch (IOException e)
                {
                    Console.WriteLine("The file could not be read:");
                    Console.WriteLine(e.Message);
                }

            }
            catch(SocketException se)
            {
                isConnected = false;
                MessageBox.Show("SocketException " + se.ToString());
            }

        }

        //download chosen file from server list IN PROGRESS
        internal void GetFile()
        {
            sender.Connect(endPoint);

            sender.Close();
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

//sender.Receive(buffer, sizeof(long), SocketFlags.None);
//long test1 = BitConverter.ToInt64(buffer, 0);
//sender.Receive(buffer, 1024, SocketFlags.None);
//string test2 = Encoding.ASCII.GetString(buffer);
//MessageBox.Show("String : " + test2 + "\nLong : " + test1.ToString());