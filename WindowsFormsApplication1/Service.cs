using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    class Service : SerialPort, IContract // створюємо клас Service, успадковуємо його від класу SerialPort та реалізуємо інтерфейс IContract
    {
        static private Service serialPort; /* статичне поле типу Service */
        string portName; /* поле типу string для надання імені порта */
        public string _PortName 
        {
            get { return portName; }
            set { portName = value; }
        } /* Властивість */
        public static Service Instance()
        {
            if (serialPort == null)
            {
                serialPort = new Service();
            }
            return serialPort;
        } /* реалізація шаблону Singletone для отримання посилання на даний екземпляр класу */
        
        public void ToWrite(string message) 
        {
            PortName = "COM3";
            Open();
            Write(message);//Записуємо повідомлення у порт
            Close();
        }/* метод для посилання повідомлень на COM-порт */
        public string Say(string input)
        {
            if (!IsOpen)
            {
                portName = MainWindow.InstanceForm().ComboBox1.Text;
                PortName = portName;
                Open(); 
            }
            
            if (IsOpen)
            {
                MainWindow form1 = MainWindow.InstanceForm();
                form1.Label1 = input;
                string timePattern = @"[0-9][0-9]:[0-9][0-9]:[0-9][0-9][^time]"; /* Регулярный вираз для отримання повідомлень о запланованій операції */
                Regex regex = new Regex(timePattern);
                bool success = regex.IsMatch(input);
                
                if (success)
                {
                    string[] input2 = input.Split(' ');
                    input = input2[0];
                    switch (input2[1])
                    {
                        case "timePicker_lr": form1.LabelTime1 = input; break;
                        case "timePicker_br": form1.LabelTime2 = input; break; form1.Time2 = DateTime.Parse(input);
                        case "timePicker_cd": form1.LabelTime3 = input; break; form1.Time3 = DateTime.Parse(input);
                    }
                    
                    if (input2[1] == "timePicker_lr")
                    {
                        form1.Time = DateTime.Parse(input);
                    }
                    else if (input2[1] == "timePicker_br")
                    {
                        form1.Time2 = DateTime.Parse(input);
                    }
                    else
                    {
                        form1.Time3 = DateTime.Parse(input);
                    }
                }
                else
                {
                    Write(input);
                }
            }
            Close();
            return "OK";
        } /* метод, який реалізований з інтерфейсу. Обробляє отримані повідомлення */
    }
}
