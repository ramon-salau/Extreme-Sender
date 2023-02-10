using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ES
{
    class Reciever
    {
        const int PORT = 1723;
        public static async void ReciveFile()
        {
            TcpListener listener = TcpListener.Create(PORT);
            listener.Start();
            TcpClient client = await listener.AcceptTcpClientAsync();
            NetworkStream ns = client.GetStream();
            // Get file info
            long fileLength = 0;
            string fileName = null;
            while(ns.DataAvailable == true){
                byte[] fileNameBytes = null;
                byte[] fileNameLengthBytes = new byte[4]; //int32
                byte[] fileLengthBytes = new byte[8]; //int64

                await ns.ReadAsync(fileLengthBytes, 0, 8); // int64
                await ns.ReadAsync(fileNameLengthBytes, 0, 4); // int32
                fileNameBytes = new byte[BitConverter.ToInt32(fileNameLengthBytes, 0)];
                await ns.ReadAsync(fileNameBytes, 0, fileNameBytes.Length);

                fileLength = BitConverter.ToInt64(fileLengthBytes, 0);
                fileName = ASCIIEncoding.ASCII.GetString(fileNameBytes);
            }
            // Set save location
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.CreatePrompt = false;
            sfd.OverwritePrompt = true;
            sfd.FileName = fileName;
            if (sfd.ShowDialog() != DialogResult.OK)
            {
                ns.WriteByte(0); // Permission denied
                return;
            }
            ns.WriteByte(1); // Permission grantedd
            FileStream fileStream = File.Open(sfd.FileName, FileMode.Create);

            // Receive
            int read;
            int totalRead = 0;
            byte[] buffer = new byte[32 * 1024]; // 32k chunks
            while ((read = await ns.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, read);
                totalRead += read;
            }

            fileStream.Dispose();
            MessageBox.Show("File successfully received");
        }

        static void CloseConnection()
        {
        }
    }
}
