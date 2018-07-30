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
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;

namespace SerialCommunicateWpfApp
{
    public partial class MainWindow : Window
    {
        SerialPort serialPort = null;
        SqlConnection sqlConnection = null;
        string localDbPath = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\atsusuke\WorkSpace\source\repos\SerialCommunicateWpfApp\SerialCommunicateWpfApp\SensorDB.mdf;Integrated Security=True";

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

            sqlConnection = new SqlConnection(localDbPath);
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
            if (buffer.First() == 0xFF && CalcChecksum(buffer) == buffer[buffer.Length - 1])
            {
                return buffer;
            }
            else
            {
                serialPort.DiscardInBuffer();
                return new byte[0];
            }
        }

        private void InsertOf(byte[] buffer)
        {
            if (sqlConnection.State == ConnectionState.Closed)
            {
                sqlConnection.Open();
            }

            try
            {
                var sqlCount = new SqlCommand("SELECT COUNT(*) FROM SENSORS", sqlConnection);
                int count = (int)sqlCount.ExecuteScalar();
                var sqlInsert = new SqlCommand("INSERT INTO SENSORS (Id, [DateTime], [Current], [Temperature], [Humidity], [Illumination], [Dust]) VALUES (@Id, @DateTime, @Current, @Temperature, @Humidity, @Illumination, @Dust)", sqlConnection);
                sqlInsert.Parameters.AddWithValue("@Id", count);
                sqlInsert.Parameters.AddWithValue("@DateTime", new DateTime(int.Parse($"20{buffer[4]}"), buffer[5], buffer[6], buffer[7], buffer[8], buffer[9]));
                sqlInsert.Parameters.AddWithValue("@Current", (buffer[10] == 0xFE) ? SqlByte.Null : buffer[10]);
                sqlInsert.Parameters.AddWithValue("@Temperature", (buffer[11] == 0xFE) ? SqlByte.Null : buffer[11]);
                sqlInsert.Parameters.AddWithValue("@Humidity", (buffer[12] == 0xFE) ? SqlByte.Null : buffer[12]);
                sqlInsert.Parameters.AddWithValue("@Illumination", (buffer[13] == 0xFE) ? SqlByte.Null : buffer[13]);
                sqlInsert.Parameters.AddWithValue("@Dust", (buffer[14] == 0xFE) ? SqlByte.Null : buffer[14]);
                sqlInsert.ExecuteNonQuery();
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private void TickShowReception(object sender, EventArgs e)
        {
            if (serialPort != null)
            {
                if (serialPort.IsOpen == true)
                {
                    byte[] buffer = SerialReadBuffer();
                    string readLine = "";

                    for (int i = 2; i < buffer.Length - 1; i++)
                    {
                        readLine += buffer[i] + " ";
                    }

                    if (buffer.Length != 0)
                    {
                        ReadLineList.Items.Add(readLine);
                        ReadLineList.Items.Refresh();
                        InsertOf(buffer);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var sqlConnection = new SqlConnection(localDbPath);
            try
            {
                sqlConnection.Open();
                var sqlSelect = new SqlCommand("SELECT * FROM SENSORS", sqlConnection);
                SqlDataReader reader = sqlSelect.ExecuteReader();
                while (reader.Read() == true)
                {
                    Console.WriteLine((int)reader["Id"]);
                    Console.WriteLine((DateTime)reader["DateTime"]);
                    Console.WriteLine((Boolean)reader["Current"]);
                    Console.WriteLine((int)reader["Temperature"]);
                }
                reader.Close();
            }
            finally
            {
                sqlConnection.Close();
            }
        }
    }
}
