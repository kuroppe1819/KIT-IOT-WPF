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
using System.Threading;

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
            timer.Interval = TimeSpan.FromMilliseconds(1000); //ある程度秒数に余裕を待たせないとシリアルポートから読み込みができない
            timer.Tick += new EventHandler(TickShowReception);
            timer.Start();
        }

        private int CalcChecksum(byte[] buffer)
        {
            int sum = 0;
            for (int i = 2; i < buffer.Length - 1; i++)
            {
                sum += buffer[i];
            }
            return 0xFF - (sum & 0xFF);
        }

        private byte[] SerialReadBuffer()
        {
            var buffer = new byte[16];

            serialPort.Read(buffer, 0, buffer.Length);
            if (buffer.First() == 0xFF && CalcChecksum(buffer) == buffer[buffer.Length - 1]) {
                return buffer;
            }
            else
            {
                serialPort.DiscardInBuffer();
                return new byte[0];
            }
        }

        private async void TickShowReception(object sender, EventArgs e)
        {
            if (serialPort != null)
            {
                if (serialPort.IsOpen == true)
                {
                    byte[] buffer = await Task.Run(() => SerialReadBuffer());
                    string readLine = "";

                    for (int i = 2; i < buffer.Length - 1; i++) {
                        readLine += buffer[i] + " ";
                    }

                    if (readLine != "")
                    {
                        ReadLineList.Items.Add(readLine);
                        ReadLineList.Items.Refresh();
                    }
                    else
                    {
                        //TODO: テストが終わったらさくじょする
                        ReadLineList.Items.Add("読み取りに失敗しました");
                        ReadLineList.Items.Refresh();
                    }
                }
            }
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
