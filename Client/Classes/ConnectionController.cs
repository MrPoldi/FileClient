using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Text;
using System.IO;
using Client.Classes;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Client.Classes
{
    public class ConnectionController
    {
        //array of bytes
        private byte[] buffer;

        //info about server
        private IPAddress ipAddr;
        private IPEndPoint endPoint;
        private Socket sender;

        //info about files
        private FileInfo fInfo;
        private long fileToSendSize;
        private string fileTSPath;
        private string fileTSName;
        private string fileToDownload;

        //info about connection
        private bool isConnected;

        //setters are used to change variables from MainWindow level
        public bool IsConnected
        {
            get { return isConnected; }
        }

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

        //try to connect a servert (DONE)
        internal void Connect()
        {
            if (!isConnected)
            {
                try
                {
                    sender.Connect(endPoint);
                    isConnected = true;
                }

                catch (SocketException se)
                {
                    isConnected = false;
                    MessageBox.Show("SocketException " + se.ToString());
                }
            }
        }

        //send local file to server and receive refreshed list of files (RECEIVING LIST OF FILES IN PROGRESS)
        internal async Task SendFile()
        {
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    SendRequest("a");

                    //sending file name
                    buffer = Encoding.ASCII.GetBytes(fileTSName);
                    sender.Send(buffer);

                    //sending file length if server is ready
                    if (IsReady())
                    {
                        fInfo = new FileInfo(fileTSPath);
                        buffer = BitConverter.GetBytes(fInfo.Length);
                        sender.Send(buffer);
                    }

                    //sending file if server is ready
                    if (IsReady())
                    {
                        sender.SendFile(fileTSPath);
                    }

                    if (IsReady())
                        MessageBox.Show("File sent");
                    else
                    {
                        MessageBox.Show("Server didn't save file : " + fileTSName + ". \nYou are disconnected.");
                    }

                }

                catch (SocketException se)
                {
                    if(isConnected)
                    {
                        MessageBox.Show(se.ToString()); ;
                    }
                    else
                    {
                        MessageBox.Show("Connection failed.");
                    }
                    isConnected = false;
                }
            });

        }

        //download chosen file from server list IN PROGRESS
        internal void GetFile()
        {
            try
            { 
                SendRequest("b");

                fileToDownload = "cat1.jpg";

                //sending name of wanted file
                sender.Send(Encoding.ASCII.GetBytes(fileToDownload));
                
                //receiving file length
                sender.Receive(buffer);

                try
                {
                    long fileSize = 28040;

                    buffer = BitConverter.GetBytes(fileSize);
                    fileSize = BitConverter.ToInt64(buffer, 0);


                    MessageBox.Show(fileSize.ToString());
                }
                catch(ArgumentException e)
                {
                    MessageBox.Show(e.ToString());
                }

            }
            catch(SocketException e)
            {
                isConnected = false;
                MessageBox.Show("SocketException " + e.ToString());
            }
        }
        
        //receiving list of files from server (IN PROGRESS)
        internal List<String> ReturnServerFiles()
        {
            List<String> ServFiles = new List<String>();
            string nameFile;

            SendRequest("c");
            sender.Receive(buffer);

            nameFile = Encoding.ASCII.GetString(buffer);
            ServFiles.Add(nameFile);

            return ServFiles;
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

        //disconnect from server
        internal bool Disconnect()
        {
            if (isConnected)
            {
                try
                {
                    buffer = Encoding.ASCII.GetBytes("d");
                    sender.Send(buffer);
                    sender.Shutdown(SocketShutdown.Both);
                    isConnected = false;
                    return true;
                }
                catch(SocketException e)
                {
                    MessageBox.Show(e.ToString());
                    isConnected = false;
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        //check if server is ready 
        private bool IsReady()
        {
            bool isReady;

            sender.Receive(buffer, sizeof(bool), 0);

            if (isReady = BitConverter.ToBoolean(buffer, 0))
                return true;
            else
                return false;
        }

        //send string option to server (for example: "a" = server will prepare to receive the file)
        private void SendRequest(string x)
        {
            if(isConnected)
            {
                //option conversion
                buffer = Encoding.ASCII.GetBytes(x);
                //sending server option
                sender.Send(buffer);
            }
        }
    }
}