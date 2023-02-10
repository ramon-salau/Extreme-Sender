using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.IO;

namespace ES
{
    class Sender
    {
        static FileStream fileStream = null;
        /*
        static async void Connect(IPAddress ip, int port)
        {
            
        }
        */
        public static async void SendFile()
        {
            TcpClient client = new TcpClient();
            try
            {
                await client.ConnectAsync(IPAddress.Loopback, 1723);
            }
            catch
            {
                MessageBox.Show("Error connecting to destination");
                return;
            }
            NetworkStream ns = client.GetStream();
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;
            FileInfo file = null;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                file = new FileInfo(ofd.FileName);
                fileStream = file.OpenRead();
            }

            byte[] fileName = ASCIIEncoding.ASCII.GetBytes(file.Name);
            byte[] fileNameLength = BitConverter.GetBytes(fileName.Length);
            byte[] fileLength = BitConverter.GetBytes(file.Length);
            await ns.WriteAsync(fileLength, 0, fileLength.Length);
            await ns.WriteAsync(fileNameLength, 0, fileNameLength.Length);
            await ns.WriteAsync(fileName, 0, fileName.Length);

            int read; 
            int totalWritten = 0;
            byte[] buffer = new byte[32 * 1024]; // 32k chunks
            while ((read = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await ns.WriteAsync(buffer, 0, read);
                totalWritten += read;
            }
            fileStream.Dispose();
        }
    }   
}
