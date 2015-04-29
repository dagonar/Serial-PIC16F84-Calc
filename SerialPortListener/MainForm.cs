using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SerialPortListener.Serial;
using System.IO;

namespace SerialPortListener
{
    public partial class MainForm : Form
    {
        SerialPortManager _spManager;
        private string firstNumber;
        private string secondNumber;
        private string Result;
        private string Function;
        private bool firstNumberIsReady;
        private bool secondNumberIsReady;
        private bool functionWasSet;
        public MainForm()
        {
            InitializeComponent();
            UserInitialization();

            Console.WriteLine("CONSTRUCTOR");
        }
        private void AuxVariablesInit() 
        {
            this.firstNumber = "";
            this.secondNumber = "";
            this.Result = "";
            this.Function = "";
            this.firstNumberIsReady = false;
            this.secondNumberIsReady = false;
            this.functionWasSet = false;
        }

      
        private void UserInitialization()
        {
            _spManager = new SerialPortManager();
            SerialSettings mySerialSettings = _spManager.CurrentSerialSettings;
            serialSettingsBindingSource.DataSource = mySerialSettings;
            portNameComboBox.DataSource = mySerialSettings.PortNameCollection;
            baudRateComboBox.DataSource = mySerialSettings.BaudRateCollection;
            dataBitsComboBox.DataSource = mySerialSettings.DataBitsCollection;
            parityComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.Parity));
            stopBitsComboBox.DataSource = Enum.GetValues(typeof(System.IO.Ports.StopBits));

            _spManager.NewSerialDataRecieved += new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved);
            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);
        }

        
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _spManager.Dispose();   
        }

        void _spManager_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            

            if (this.InvokeRequired)
            {
                // Using this.Invoke causes deadlock when closing serial port, and BeginInvoke is good practice anyway.
                this.BeginInvoke(new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved), new object[] { sender, e });
                return;
            }

            int maxTextLength = 1000; // maximum text length in text box
            if (tbData.TextLength > maxTextLength)
                tbData.Text = tbData.Text.Remove(0, tbData.TextLength - maxTextLength);

            // Obtenemos el arreglo de Bytes del botón que se pulso
            Byte[] arrayByte = e.Data.ToArray<Byte>();
            string Hex = "";
            for (int i = 0; i < arrayByte.Length; i++)
                Hex += arrayByte[i].ToString();

            //Console.WriteLine("Escribiste : " + Hex);
            //tbData.Text = "Número : " + this.getFunction(Hex);
            if (this.isNumber(Hex) && !this.firstNumberIsReady)
            {
                this.firstNumber += this.getNumber(Hex);
                this.tbData.Text = this.firstNumber;
            }
            else if(this.isFunction(Hex) && !this.isEqual(Hex) &&!this.isClear(Hex)){
                this.Function = this.getFunction(Hex);
                this.firstNumberIsReady = true;
                this.tbData.Text += this.Function;
            }
            else if (this.isNumber(Hex) && !this.secondNumberIsReady)
            {
                this.secondNumber += this.getNumber(Hex);
                this.tbData.Text += this.getNumber(Hex);
            }
            else if (this.isEqual(Hex))
            {
                this.secondNumberIsReady = true;
                this.tbData.Text += this.getFunction(Hex);
                if (this.Function == "+")
                    this.Result = (Convert.ToInt32(this.firstNumber) + Convert.ToInt32(this.secondNumber)).ToString();
                else if(this.Function == "-")
                    this.Result = (Convert.ToInt32(this.firstNumber) - Convert.ToInt32(this.secondNumber)).ToString();
                else if (this.Function == "x")
                    this.Result = (Convert.ToInt32(this.firstNumber) * Convert.ToInt32(this.secondNumber)).ToString();
                else if (this.Function == "/")
                    this.Result = (Convert.ToInt32(this.firstNumber) / Convert.ToInt32(this.secondNumber)).ToString();
                
                tbData.Text += Result;
                
            }
            else if (this.isClear(Hex))
            {
                this.reset();
            }
                
        }

        // Handles the "Start Listening"-buttom click event
        private void btnStart_Click(object sender, EventArgs e)
        {
            _spManager.StartListening();
            Console.WriteLine("INICIO LECTURA");
           
        }

        // Handles the "Stop Listening"-buttom click event
        private void btnStop_Click(object sender, EventArgs e)
        {
            _spManager.StopListening();
            Console.WriteLine("CANCELAR LECTURA");
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            tbData.Text = "";
        }

        private string getNumber(string _hex)
        {
            string result = "";
            if (_hex == "12") result = "1";
            else if (_hex == "28") result = "2";
            else if (_hex == "62") result = "3";
            else if (_hex == "16") result = "4";
            else if (_hex == "108") result = "5";
            else if (_hex == "0") result = "6";
            else if (_hex == "14") result = "7";
            else if (_hex == "2") result = "8";
            else if (_hex == "114") result = "9";
            else if (_hex == "126") result = "0";

            return result;
        }
        private bool isNumber(string _hex)
        {
            bool result = false;
            if (_hex == "12") result = true;
            else if (_hex == "28") result = true;
            else if (_hex == "62") result = true;
            else if (_hex == "16") result = true;
            else if (_hex == "108") result = true;
            else if (_hex == "0") result = true;
            else if (_hex == "14") result = true;
            else if (_hex == "2") result = true;
            else if (_hex == "114") result = true;
            else if (_hex == "126") result = true;

            return result;
        }
        private string getFunction(string _hex)
        {
            string func = "";
            if (_hex.Contains("30")) func = "+";
            else if (_hex.Contains("242") || _hex.Contains("156")) func = "-";
            else if (_hex.Contains("96")) func = "x";
            else if (_hex.Contains("60")) func = "/";
            else if (_hex.Contains("144") || _hex.Contains("131")) func = "=";
            else if (_hex.Contains("254")) func = "C";
            return func;
        }

        private bool isFunction(string _hex)
        {
            bool result = false;

            if (_hex.Contains("30")) result = true;
            else if (_hex.Contains("242") || _hex.Contains("156")) result = true;
            else if (_hex.Contains("96")) result = true;
            else if (_hex.Contains("60")) result = true;
            else if (_hex.Contains("144") || _hex.Contains("131")) result = true;
            else if (_hex.Contains("254")) result = true;

            return result;
        }

        private bool isEqual(string _hex)
        {
            bool result = false;
            if (_hex.Contains("144") || _hex.Contains("131"))
                result = true;
            return result;
        }
        private bool isClear(string _hex)
        {
            bool result = false;
            if (_hex.Contains("254"))
                result = true;
            return result;
        }

        private void reset()
        {
            this.firstNumber = "";
            this.secondNumber = "";
            this.Result = "";
            this.Function = "";
            this.firstNumberIsReady = false;
            this.secondNumberIsReady = false;
            this.functionWasSet = false;
            this.tbData.Text = "";
        }
    }
}
