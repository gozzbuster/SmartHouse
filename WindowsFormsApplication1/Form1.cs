using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Networking;

namespace WindowsFormsApplication1
{
    public partial class MainWindow : Form
    {
        ServiceHost host; //Поле host типу ServiceHost
        string hostName;
        SerialPort serPort;
        private static MainWindow form1;
        DateTime time, time2, time3;
        private int inst;

        public DateTime Time
        {
            get { return time; }
            set
            {
                time = value;
            }
        }
        public DateTime Time2
        {
            get
            {

                return time2;
            }
            set
            {
                time2 = value;
            }
        }
        public DateTime Time3
        {
            get
            {

                return time3;
            }
            set {
                time3 = value;
            }
        }
        public System.Windows.Forms.Timer _Timer
        {
            get { return timer1; }
            set { timer1 = value; }
        }
        public string _TimerLabel
        {
            get { return TimeLabel.Text; }
            set { TimeLabel.Text = value;}
        }
        public ComboBox ComboBox1
        {
            get
            {
                return comboBox1;
            }
        }
        public string Label1
        {
            set
            {
                string pattern = @"^(25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}|[0-9]{2}|[0-9])(\.(25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}|[0-9]{2}|[0-9])){3}$";
                Regex regex = new Regex(pattern);
                bool success = regex.IsMatch(value);
                if (success)
                {
                    label1.Text = "Подключено: ";
                    label1.Text += value;
                }
            }
            get
            {
                return label1.Text;
            }
            
        }
        public string LabelTime1
        {
            get
            {
                return label6.Text;
            }
            set
            {
                label6.Text = "Автоматическое изменение в ";
                label6.Text += value;
            }
        }
        public string LabelTime2
        {
            get
            {
                return label7.Text;
            }
            set
            {
                label7.Text = "Автоматическое изменение в ";
                label7.Text += value;
            }
        }
        public string LabelTime3
        {
            get
            {
                return label8.Text;
            }
            set
            {
                label8.Text = "Автоматическое изменение в ";
                label8.Text += value;
            }
        }
        public MainWindow()
        {

            InitializeComponent();
            StartServerButton.BackColor = Color.BlueViolet;
            hostName = Dns.GetHostName();
            try
            {
                label2.Text += HostName.GetInstanceHostName().GetLocalIP().ToString();
            }
            catch
            {
                Exception ex = new Exception("Проверьте соедиение!");
                MessageBox.Show(ex.Message);
            }
            timer1.Start();
        }
        
        public static MainWindow InstanceForm() /*Метод-одинак*/
        {
            if (form1 == null)
            {
                form1 = new MainWindow();
            }
            return form1;
        }
        private void Start_button_Click(object sender, EventArgs e) //метод-обробник натискання по кнопці
        {
            if (StartServerButton.Text == "Запустить сервер")
            {
                try
                {
                    Uri address = new Uri("net.tcp://" + HostName.GetInstanceHostName().GetLocalIP().ToString() + ":4000/IContract");/* Отримуємо IP-адресу локальної машини, та записуємо її у змінну address типу Uri */
                    NetTcpBinding binding = new NetTcpBinding(); /* Отримуємо надійну прив'язку для обміну даними між сервер-провайдером та конс'юмером */
                    binding.Security.Mode = SecurityMode.None; /*тип безпеки: безпека заблокована*/
                    Type contract = typeof(IContract);
                    host = new ServiceHost(typeof(Service)); // об'єднаємо в змінній host всі складові сервіса
                    host.AddServiceEndpoint(contract, binding, address);
                    host.Open();
                    StartServerButton.Text = "Остановить сервер";
                    StartServerButton.BackColor = Color.Green;
                    this.Text = "Сервер \"Умный Дом\"  [Работает]";
                }
                catch
                {
                    Exception ex = new Exception("Отсутствует соединение");
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                host.Close();
                StartServerButton.Text = "Запустить сервер";
                StartServerButton.BackColor = Color.BlueViolet;
                this.Text = "Сервер \"Умный Дом\"  [Отключён]";
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            
            string POT = serPort.ReadLine();
            this.BeginInvoke(new LineReceivedEvent(LineReceived), POT);
        }
        private delegate void LineReceivedEvent(string POT);

        private void LineReceived(string POT)
        {
            label1.Text = POT;


        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            TimeLabel.Text = DateTime.Now.Hour.ToString("00") + ":" + DateTime.Now.Minute.ToString("00") + ":" + DateTime.Now.Second.ToString("00");

            if (time.Hour == DateTime.Now.Hour && time.Minute == DateTime.Now.Minute && time.Second == DateTime.Now.Second)
            {
                Service.Instance().ToWrite("A");
                label6.Text = "Автоматическое изменение в ";
            }
            else if (time2.Hour == DateTime.Now.Hour && time2.Minute == DateTime.Now.Minute && time2.Second == DateTime.Now.Second)
            {
                Service.Instance().ToWrite("C");
                label7.Text = "Автоматическое изменение в ";
            }
            else if (time2.Hour == DateTime.Now.Hour && time2.Minute == DateTime.Now.Minute && time2.Second == DateTime.Now.Second)
            {
                Service.Instance().ToWrite("E");
                label8.Text = "Автоматическое изменение в ";
            }
        }
    }  
}
