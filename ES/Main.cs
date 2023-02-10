using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ES
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        private void Main_Load(object sender, EventArgs e)
        {
        }

        private void recv_Click(object sender, EventArgs e)
        {
            Reciever.ReciveFile();
        }

        private void send_Click(object sender, EventArgs e)
        {
            Sender.SendFile();
        }
    }
}
