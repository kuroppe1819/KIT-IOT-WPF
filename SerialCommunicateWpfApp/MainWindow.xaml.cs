using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace SerialCommunicateWpfApp
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        SerialPort serialPort = null;
        public MainWindow()
        {
            InitializeComponent();
            PortNameBox.ItemsSource = SerialPort.GetPortNames().ToList();
            OpenPortBtn.IsEnabled = true;
            ClosePortBtn.IsEnabled = false;

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += new EventHandler(SerialReadLine);
            timer.Start();
        }

        private SerialPort CreateSerialPort()
        {
            SerialPort serialPort = new SerialPort();
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.Encoding = Encoding.UTF8;
            serialPort.ReadTimeout = 100000;
            return serialPort;
        }

        void SerialReadLine(object sender, EventArgs e)
        {
            if (serialPort == null) {
                return;
            }

            if (serialPort.IsOpen == true)
            {
                ReadLineList.Items.Add(serialPort.ReadLine());
                ReadLineList.Items.Refresh();
            }
        }

        private void OpenPortBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PortNameBox.Text == "" || BaundRateBox.Text == "")
            {
                MessageBox.Show("使用するポートと通信速度を設定してください");
                return;
            }
            serialPort = CreateSerialPort();
            serialPort.PortName = PortNameBox.Text;
            serialPort.BaudRate = Convert.ToInt32(BaundRateBox.Text);

            try
            {
                serialPort.Open();
                OpenPortBtn.IsEnabled = false;
                ClosePortBtn.IsEnabled = true;
                ProgressStatus.Value = 100;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ClosePortBtn_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort == null)
            {
                return;
            }

            try
            {
                serialPort.Close();
                OpenPortBtn.IsEnabled = true;
                ClosePortBtn.IsEnabled = false;
                ProgressStatus.Value = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
