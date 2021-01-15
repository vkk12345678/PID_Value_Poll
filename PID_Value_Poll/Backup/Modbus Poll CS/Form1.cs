using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Timers;
using System.IO.Ports;

namespace Modbus_Poll_CS
{
    public partial class Form1 : Form
    {
        modbus mb = new modbus();
        SerialPort sp = new SerialPort();        
        System.Timers.Timer timer = new System.Timers.Timer(); 
        byte Id = 1;
        #region GUI Delegate Declarations
        public delegate void GUIDelegate(string paramString);
        public delegate void GUIClear();
        public delegate void GUIStatus(string paramString);
        #endregion

        public Form1()
        {
            InitializeComponent();           
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }


        public void DoGUIUpdate(string paramString)
        {
            try
            {

                if (this.InvokeRequired)
                {
                    GUIDelegate delegateMethod = new GUIDelegate(this.DoGUIUpdate);
                    this.Invoke(delegateMethod, new object[] { paramString });
                }
                else
                    switch (Id)
                    {
                        case 1:
                            Id = 2;
                            //double l;
                            //l = Double.Parse(paramString)/10;
                            this.label10.Text = paramString;
                            StartPoll(); 
                            break;
                        case 2:
                            Id = 1;
                            this.label11.Text = paramString;
                            StartPoll(); 
                            break;
                    }                    
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error in DoGUIUpdate: " + ex.Message);
            }
        }
      

        #region Timer Elapsed Event Handler
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PollFunction();
        }
        #endregion

        #region Start and Stop Procedures
        private void StartPoll()
        {

            
            if (mb.Open("COM1", 9600, 8, Parity.Odd, StopBits.One))
            {
                timer.AutoReset = true;
                timer.Interval = 100;               
                timer.Start();                
            }
        }
        private void StopPoll()
        {           
            timer.Stop();
            mb.Close();            
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            StartPoll();
            
        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            StopPoll();
        }
        #endregion

        #region Poll Function
        
        private void PollFunction()
        {          
            short[] values = new short[Convert.ToInt32(1)];
            ushort pollStart = 138;
            string itemString;            
            
            try
            {
                switch (Id)
                {
                    case 1:                        
                        while (!mb.SendFc3(1, pollStart, 1, ref values)) ;
                        itemString = values[0].ToString();                        
                        DoGUIUpdate(itemString);
                        break;
                    case 2:                        
                        while (!mb.SendFc3(2, pollStart, 1, ref values)) ;
                        itemString = values[0].ToString();                         
                        DoGUIUpdate(itemString);                        
                        break;
                }  
            }
            catch (Exception err)
            {
                MessageBox.Show ("Error in modbus read: " + err.Message);
            }
        }
        #endregion

        
        #region Write Function
        private void WriteFunction()
        {
            ////StopPoll();

            //if (txtWriteRegister.Text != "" && txtWriteValue.Text != "" && txtSlaveID.Text != "")
            //{
            //    byte address = Convert.ToByte(txtSlaveID.Text);
            //    ushort start = Convert.ToUInt16(txtWriteRegister.Text);
            //    short[] value = new short[1];
            //    value[0] = Convert.ToInt16(txtWriteValue.Text);

            //    try
            //    {
            //        while (!mb.SendFc16(address, start, (ushort)1, value)) ;
            //    }
            //    catch (Exception err)
            //    {
            //        DoGUIStatus("Error in write function: " + err.Message);
            //    }
            //    DoGUIStatus(mb.modbusStatus);
            //}
            //else
            //    DoGUIStatus("Enter all fields before attempting a write");

            ////StartPoll();
        }
        private void btnWrite_Click(object sender, EventArgs e)
        {
            WriteFunction();
        }
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}