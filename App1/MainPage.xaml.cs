using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net;
using System.Xml;
using System.IO.IsolatedStorage;
using Windows.Storage;
using System.Text.RegularExpressions;
using Windows.UI;
using System.Threading;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Networking;
using Windows.Networking.Connectivity;
using System.Threading.Tasks;

// Документацию по шаблону элемента "Пустая страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace App1
{

    [ServiceContract]
    interface IContract
    {
        [OperationContract]
        string Say(string input);
    }
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    /// 
    public sealed partial class MainPage : Page
    {
        NetTcpBinding binding = new NetTcpBinding();
        EndpointAddress endpoint;
        ChannelFactory<IContract> factory;
        IContract channel;
        string IP;
        int flag = 0;
        bool isStarted;
        XmlDocument ipaddresses = new XmlDocument();
        bool[] toggle_flags = new bool[6];
        public MainPage()
        {
            this.InitializeComponent();
            textBox.IsEnabled = false;
            
            IP = textBox.Text;
            CreateFile();
            textBox.Text = IP;
        }
        private void Refresh_toggle()
        {
            for (int i = 0; i < 6; i++)
            {
                toggle_flags[i] = false;
            }
        }
        private void TimeToggle()
        {
        }
        private async void CreateFile()
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile sampleFile = await storageFolder.GetFileAsync("sample.txt");
                IP = await FileIO.ReadTextAsync(sampleFile);
            }
            catch
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile sampleFile = await storageFolder.CreateFileAsync("sample.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sampleFile, IP);
            }
            
            
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            
            
        }
        private void button_Holding(object sender, HoldingRoutedEventArgs e)
        {
            textBox.IsEnabled = true;
            textBox.Text = "";
            textBox.Focus(FocusState.Programmatic);
            textBox.LostFocus += TextBox_LostFocus;
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            textBox.IsEnabled = true;
            textBox.Focus(FocusState.Programmatic);
            textBox.LostFocus += TextBox_LostFocus;
        }

        private async void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string pattern = @"^(25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}|[0-9]{2}|[0-9])(\.(25[0-5]|2[0-4][0-9]|[0-1][0-9]{2}|[0-9]{2}|[0-9])){3}$";
            Regex regex = new Regex(pattern);
            bool success = regex.IsMatch(textBox.Text);
            if (success)
            {
                textBox.IsEnabled = false;

                if (textBox.Text == "")
                {
                    textBox.Text = IP;
                }
                IP = textBox.Text;
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile sampleFile = await storageFolder.CreateFileAsync("sample.txt", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(sampleFile, IP);
                button1.IsEnabled = true;
                button1.Content = "Соединить";
            }
            else
            {
                Color color = new Color();
                color = Colors.Red;
                SolidColorBrush scb = new SolidColorBrush(color);
                textBox.BorderBrush = scb;
            }
        }
        // Function to get the IP Address from Windows Phone 8
        public IPAddress GetIPAddress()
        {
            List<string> IpAddress = new List<string>();
            var Hosts = Windows.Networking.Connectivity.NetworkInformation.GetHostNames().ToList();
            foreach (var Host in Hosts)
            {
                string IP = Host.DisplayName;
                IpAddress.Add(IP);
            }
            IPAddress my_address = IPAddress.Parse(IpAddress.Last());
            return my_address;
        }
        public void StartConnect()
        {
            IPAddress myIP = GetIPAddress();
            Uri address = new Uri("net.tcp://" + IP + ":4000/IContract");
            binding.Security.Mode = SecurityMode.None;
            endpoint = new EndpointAddress(address);
            factory = new ChannelFactory<IContract>(binding, endpoint);
            channel = factory.CreateChannel();
            string answer = channel.Say(myIP.ToString());
            button1.Content = "Соединение установленно";
            button1.IsEnabled = false;
            isStarted = true;
        }
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            StartConnect();
        }
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            StorageFile storageFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(
                new Uri("ms-appx:///VoiceCommands.xml", UriKind.Absolute));
            await VoiceCommandDefinitionManager.InstallCommandDefinitionsFromStorageFileAsync(storageFile);
        }
        public async void toggleSwitch_switch(string command, bool isOn, string time)
        {
            if (!isStarted)
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
                StartConnect();
                
            }
            if (time == ":00")
            {
                switch (command)
                {
                    case "Living room": toggleSwitch.IsOn = isOn; break;
                    case "Bedroom": toggleSwitch_Copy.IsOn = isOn; break;
                    case "Corridor": toggleSwitch_Copy1.IsOn = isOn; break;
                }
            }
            else
            {

                TimeSpan timeSpan = TimeSpan.Parse(time);
                switch (command)
                {
                    case "Living room": timePicker_lr.Time = timeSpan; break;
                    case "Bedroom": timePicker_br.Time = timeSpan; break;
                    case "Corridor": timePicker_cd.Time = timeSpan; break;
                }
            }
        }
        private void toggleSwitch_Holding(object sender, HoldingRoutedEventArgs e)
        {
            flag++;
            if (flag%2 == 1)
            {
                ToggleSwitch tg = sender as ToggleSwitch;
                if (tg == null)
                {
                    TextBlock tb = sender as TextBlock;
                    switch (tb.Text)
                    {
                        case "Гостиная": tb.Text = "Детская";  break;
                        case "Детская": tb.Text = "Гостиная"; break;
                        case "Спальня": tb.Text = "Кухня"; break;
                        case "Кухня": tb.Text = "Спальня"; break;
                        case "Коридор": tb.Text = "Прихожая"; break;
                        case "Прихожая": tb.Text = "Коридор"; break;
                    }
                }
                else
                {
                    switch ((string)tg.Header)
                    {
                        case "Гостиная": tg.Header = "Детская"; break;
                        case "Детская": tg.Header = "Гостиная"; break;
                        case "Спальня": tg.Header = "Кухня"; break;
                        case "Кухня": tg.Header = "Спальня"; break;
                        case "Коридор": tg.Header = "Прихожая"; break;
                        case "Прихожая": tg.Header = "Коридор"; break;
                    }
                }
            }
        }

        private void toggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch ts = sender as ToggleSwitch;
            if (ts.IsOn)
            {
                switch ((string)ts.Header)
                {
                    case "Гостиная": channel.Say("A"); break;
                    case "Спальня": channel.Say("C"); break;
                    case "Коридор": channel.Say("E"); break;
                }
            }
            else
            {
                switch ((string)ts.Header)
                {
                    case "Гостиная": channel.Say("B"); break;
                    case "Спальня": channel.Say("D"); break;
                    case "Коридор": channel.Say("F"); break;
                }
            }
        }

        private void timePicker_lr_TimeChanged(object sender, TimePickerValueChangedEventArgs e)
        {
            TimePicker tp = sender as TimePicker;
            TimeSpan ts = tp.Time;
            string str = (ts.ToString() + " " + tp.Name.ToString());
            channel.Say(str);
        }
    }
}
