using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using System.Data.OleDb ;
using System.IO.Ports;

namespace Modbus_Poll_CS
{
    public partial class Form1 : Form
    {
        public static  string PATH = Application.StartupPath + "\\";   
        public OleDbConnection conn = new OleDbConnection("PROVIDER=Microsoft.Jet.OLEDB.4.0; Data Source=" + PATH + "General.mdb"); 
        public string mod1="00.00";
        public string mod2 = "00.00";
        public string mod3 = "00.00";
        public string comn;
        public int   Bd=9600;
        public int Addr;
        public int cnt = 0;
        public Parity   p;
        public StopBits  stopbt;
        public int pollStart = 138; //Convert.ToUInt16(txtStartAddr.Text); // 138;
        
        public static String[] sysa = new String[80];

        modbus mb = new modbus();

        public byte Id = 1;
       

        public Form1()
        {
            InitializeComponent();
           
        }

        #region Start and Stop Procedures
        public  void StartPoll()
        {            
            if (mb.Open(comn ,Bd,8,p,stopbt))
            {
                PollFunction();                            
            }
            lblStatus.Text = mb.modbusStatus;  
        }
        public  void StopPoll()
        {           
            tmrmod.Enabled = false;
            mb.Close();
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            comn = lstPorts.Text;
            Bd = int.Parse(lstBaudrate.Text);
            p = (Parity)Enum.Parse(typeof(Parity), lstParity.Text);
            stopbt = (StopBits)Enum.Parse(typeof(StopBits), "1");

            Addr = Convert.ToInt16(txtStartAddr.Text);
           
            if (tmrmod.Enabled == false) tmrmod.Start();
            StartPoll(); 
           
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (tmrmod.Enabled == true) tmrmod.Stop();
            StopPoll();
            
        }
        #endregion

        #region Poll Function
        
        public void PollFunction()
        {          
            short[] values = new short[Convert.ToInt32(1)];
            //int pollStart = Convert.ToUInt16(txtStartAddr.Text); // 138;
            string itemString;

            try
            {
                if (pollStart == 0)
                    pollStart = 138;
                else if (pollStart == 138)
                    pollStart = 0;
                
                Id = Convert.ToByte(txtSlaveID.Text);
                double l;
                mb.Open(comn, Bd, 8, p, stopbt);
                l = 00.00;
                while (!mb.SendFc3(Convert.ToByte(txtSlaveID.Text), Convert. ToUInt16(pollStart) , 1, ref values) == false)  // pollStart
                {

                    itemString = values[0].ToString();
                    l = Double.Parse(itemString) / 10;
                    mb.Close();
                    goto a;
                }
    a:            if ((pollStart == 138))
                {
                    label10.Text = l.ToString("00.0");
                    mod1 = l.ToString("00.0");
                    
                }
                else if ((pollStart == 0))
                {
                    label11.Text = l.ToString("00.0");
                    mod2 = l.ToString("00.0");

                }

                //else if ((Id == 3))
                //{
                //    label1.Text = l.ToString("00.0");
                //    mod3 = l.ToString("00.0");
                //}
                //lblStatus.Text = cnt.ToString();
                //Id += 1;   
               
                  
                    //if (conn.State == ConnectionState.Closed) conn.Open();
                    //OleDbCommand cmd = new OleDbCommand("Update Modbus set Modb1= '" + mod1 + "',Modb2='" + mod2 + "',Modb3='" + mod3 + "',cnt='" + lblStatus.Text +"' where ID='1'", conn);
                    //cmd.ExecuteNonQuery ();
                    //conn.Close ();


                    //if (Id > 3)
                    //    Id = 1;
               

            }
            catch (Exception err)
            {
                MessageBox.Show("Error in modbus read: " + err.Message);

            }
        }
        #endregion


        private void tmrmod_Tick(object sender, EventArgs e)
        {            
            PollFunction();
            Slabel.Text = modbus.Buf;  
                    
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadListboxes();           
           
           
            //tmrmod.Enabled = true; 
        }

        #region Load Listboxes
        private void LoadListboxes()
        {
            //Three to load - ports, baudrates, datetype.  Also set default textbox values:
            //1) Available Ports:
            string[] ports = SerialPort.GetPortNames();

            foreach (string port in ports)
            {
                lstPorts.Items.Add(port);
            }

            lstPorts.SelectedIndex = 0;

            //2) Baudrates:
            string[] baudrates = { "230400", "115200", "57600", "38400", "19200", "9600" };

            foreach (string baudrate in baudrates)
            {
                lstBaudrate.Items.Add(baudrate);
            }

            lstBaudrate.SelectedIndex = 5;

            //3) Datatype:
            string[] Parity = { "Odd", "Even", "None" };

            foreach (string parity in Parity)
            {
                lstParity.Items.Add(parity);
            }

            lstParity.SelectedIndex = 0;
           
            txtSlaveID.Text = "1";
            txtStartAddr.Text = "138";
        }
        #endregion

        #region Write Function
        private void WriteFunction()
        {
            //StopPoll();

            if (txtWriteRegister.Text != "" && txtWriteValue.Text != "" && txtSlaveID.Text != "")
            {
                byte address = Convert.ToByte(txtSlaveID.Text);
                ushort start = Convert.ToUInt16(txtWriteRegister.Text);
                short[] value = new short[1];
                value[0] = Convert.ToInt16(txtWriteValue.Text);

                try
                {
                    while (!mb.SendFc16(address, start, (ushort)1, value)) ;
                }
                catch (Exception err)
                {
                    statusStrip.Text = "Error in write function: " + err.Message;
                    //DoGUIStatus("Error in write function: " + err.Message);
                }
                statusStrip.Text = mb.modbusStatus;
            }
            else
               statusStrip.Text ="Enter all fields before attempting a write";

            //StartPoll();
        }
        private void btnWrite_Click(object sender, EventArgs e)
        {
            WriteFunction();
        }
        #endregion

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       


      

       
            
    }
}