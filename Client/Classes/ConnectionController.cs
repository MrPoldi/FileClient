using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Text;
using System.IO;
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
        private bool isBusy;

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

        public bool IsBusy
        {
            get { return isBusy; }
            set { isBusy = value; }
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

        //send local file to server and receive refreshed list of files DONE
        internal async Task SendFile()
        {
            await Task.Factory.StartNew(() =>
            {
                isBusy = true;

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
                        MessageBox.Show("Connection Error");
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

                isBusy = false;
            });

        }

        //download chosen file from server list 90% DONE - there is some bugs to fix (videos : serv -> client)
        internal async Task GetFile()
        {
            await Task.Factory.StartNew(() =>
            {
                isBusy = true;

                try
                {
                    SendRequest("b");

                    //sending name of wanted file
                    sender.Send(Encoding.ASCII.GetBytes(fileToDownload));

                    DownloadFile(fileToDownload);
                }
                catch (SocketException e)
                {
                    isConnected = false;
                    MessageBox.Show("SocketException " + e.ToString());
                }

                isBusy = false;
            });
        }
        
        //receiving list of files from server DONE
        internal List<String> ReturnServerFiles()
        {
            List<String> ServFiles = new List<String>();
            string nameFile;

            SendRequest("c");
            DownloadFile("ServerFiles.txt");

            StreamReader reader = new StreamReader("./Files/ServerFiles.txt");

            while(!reader.EndOfStream)
            {
                nameFile = reader.ReadLine();
                ServFiles.Add(nameFile);
            }

            reader.Close();

            return ServFiles;
        }

        //This method checks if IP/port are valid DONE
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

        //disconnect from server DONE
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

        //check if server is ready DONE
        private bool IsReady()
        {
            bool isReady;

            sender.Receive(buffer, sizeof(bool), 0);

            if (isReady = BitConverter.ToBoolean(buffer, 0))
                return true;
            else
                return false;
        }

        //send string option to server (for example: "a" = server will prepare to receive the file) DONE
        private void SendRequest(string request)
        {
            if(isConnected)
            {
                //option conversion
                buffer = Encoding.ASCII.GetBytes(request);
                //sending server option
                sender.Send(buffer);
            }
        }

        //create directory / file before download DONE
        private void CreateFile(string fileName)
        {
            try
            {
                //Create directory for files if not exist
                if (!Directory.Exists("./Files"))
                { 
                    Directory.CreateDirectory("./Files"); 
                }

                //Create file
                File.Create("./Files/" + fileName).Close();
            }
            catch(IOException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void DownloadFile(string fileName)
        {
            try
            {
                CreateFile(fileName);

                //creating empty buffer
                buffer = new byte[1325];

                //receiving file length
                int bytesToRcv = sender.Receive(buffer);
                long bytesRcv = 0;
                long fileSize = BitConverter.ToInt64(buffer, 0);

                //send true if ready to rcv file
                buffer = BitConverter.GetBytes(true);
                sender.Send(buffer);

                BinaryWriter writer = new BinaryWriter(File.OpenWrite("./Files/" + fileName));

                while (bytesRcv < fileSize)
                {
                    buffer = new byte[1325];
                    bytesToRcv = sender.Receive(buffer);
                    bytesRcv += bytesToRcv;
                    writer.Write(buffer, 0, buffer.Length);
                    writer.Flush();

                }
                writer.Close();
            }
            catch (SocketException e)
            {
                MessageBox.Show(e.ToString());
                isConnected = false;
            }
        }
    }
}