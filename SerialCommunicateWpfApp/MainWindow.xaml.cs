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
using Microsoft.Office.Interop.Excel;
using SerialCommunicateWpfApp.Controller;

namespace SerialCommunicateWpfApp {
    public partial class MainWindow : System.Windows.Window {
        MainController controller = new MainController();
        public MainWindow() {
            InitializeComponent();
            InitView();

        }

        private void InitView() {
            PortNameBox.ItemsSource = controller.GetPortNameList();
            OpenBtnEnabled(true);
        }

        private void OpenBtnEnabled(Boolean enabled) {
            OpenPortBtn.IsEnabled = enabled;
            ClosePortBtn.IsEnabled = !enabled;
        }

        //    private void InsertOf(byte[] buffer)
        //    {
        //        if (sqlConnection.State == ConnectionState.Closed)
        //        {
        //            sqlConnection.Open();
        //        }

        //        try
        //        {
        //            var sqlCount = new SqlCommand("SELECT COUNT(*) FROM SENSORS", sqlConnection);
        //            int count = (int)sqlCount.ExecuteScalar();
        //            var sqlInsert = new SqlCommand("INSERT INTO SENSORS (Id, [Area], [DateTime], [Current], [Temperature], [Humidity], [Illumination], [Dust]) VALUES (@Id, @Area, @DateTime, @Current, @Temperature, @Humidity, @Illumination, @Dust)", sqlConnection);
        //            sqlInsert.Parameters.AddWithValue("@Id", count);
        //            sqlInsert.Parameters.AddWithValue("@Area", (buffer[2] << 8) + buffer[3]);
        //            sqlInsert.Parameters.AddWithValue("@DateTime", new DateTime(int.Parse($"20{buffer[4]}"), buffer[5], buffer[6], buffer[7], buffer[8], buffer[9]));
        //            sqlInsert.Parameters.AddWithValue("@Current", (buffer[10] == 0xFE) ? SqlByte.Null : buffer[10]);
        //            sqlInsert.Parameters.AddWithValue("@Temperature", (buffer[11] == 0xFE) ? SqlByte.Null : buffer[11]);
        //            sqlInsert.Parameters.AddWithValue("@Humidity", (buffer[12] == 0xFE) ? SqlByte.Null : buffer[12]);
        //            sqlInsert.Parameters.AddWithValue("@Illumination", (buffer[13] == 0xFE) ? SqlByte.Null : buffer[13]);
        //            sqlInsert.Parameters.AddWithValue("@Dust", (buffer[14] == 0xFE) ? SqlByte.Null : buffer[14]);
        //            sqlInsert.ExecuteNonQuery();
        //        }
        //        finally
        //        {
        //            sqlConnection.Close();
        //        }
        //    }

        //    private async void TickShowReception(object sender, EventArgs e)
        //    {
        //        if (serialPort != null)
        //        {
        //            if (serialPort.IsOpen == true)
        //            {
        //                byte[] buffer = await Task.Run(new Func<byte[]>(() => SerialReadBuffer()));
        //                string readLine = "";

        //                for (int i = 2; i < buffer.Length - 1; i++)
        //                {
        //                    readLine += buffer[i] + " ";
        //                }

        //                if (buffer.Length != 0)
        //                {
        //                    ReadLineList.Items.Add(readLine);
        //                    ReadLineList.Items.Refresh();
        //                    InsertOf(buffer);
        //                }
        //            }
        //        }
        //    }

        private void OpenPortBtn_Click(object sender, RoutedEventArgs e) {
            try {
                controller.OpenPort(PortNameBox.Text, BaundRateBox.Text);
                OpenBtnEnabled(false);
            } catch (Exception ex) {
                MessageBox.Show("使用するポートと通信速度を設定してください");
            }
        }

        private void ClosePortBtn_Click(object sender, RoutedEventArgs e) {
            try {
                controller.ClosePort();
                OpenBtnEnabled(true);
            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
        }

        private void ExportBtn_Click(object sender, RoutedEventArgs e) {
            //        var sqlConnection = new SqlConnection(localDbPath);
            //        try
            //        {
            //            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            //            app.Visible = true;
            //            app.WindowState = XlWindowState.xlMaximized;
            //            Workbook workbook = app.Workbooks.Add(XlWBATemplate.xlWBATWorksheet);
            //            Worksheet worksheet = workbook.Worksheets[1];
            //            worksheet.Range["A1"].Value = "Id";
            //            worksheet.Range["B1"].Value = "Area";
            //            worksheet.Range["C1"].Value = "DateTime";
            //            worksheet.Range["D1"].Value = "Current";
            //            worksheet.Range["E1"].Value = "Temperature";
            //            worksheet.Range["F1"].Value = "Humidity";
            //            worksheet.Range["G1"].Value = "Illumination";
            //            worksheet.Range["H1"].Value = "Dust";

            //            sqlConnection.Open();
            //            int count = 2;
            //            var sqlSelect = new SqlCommand("SELECT * FROM SENSORS", sqlConnection);
            //            SqlDataReader reader = sqlSelect.ExecuteReader();
            //            while (reader.Read() == true)
            //            {
            //                worksheet.Range[$"A{count}"].Value = reader["Id"];
            //                worksheet.Range[$"B{count}"].Value = reader["Area"];
            //                worksheet.Range[$"C{count}"].Value = reader["DateTime"];
            //                worksheet.Range[$"D{count}"].Value = reader["Current"];
            //                worksheet.Range[$"E{count}"].Value = reader["Temperature"];
            //                worksheet.Range[$"F{count}"].Value = reader["Humidity"];
            //                worksheet.Range[$"G{count}"].Value = reader["Illumination"];
            //                worksheet.Range[$"H{count}"].Value = reader["Dust"];
            //                count++;
            //            }
            //            reader.Close();
            //            workbook.SaveAs(@"C:\Users\atsusuke\WorkSpace\source\repos\SerialCommunicateWpfApp\SerialCommunicateWpfApp\SensorData.xlsx");
            //        }
            //        finally
            //        {
            //            sqlConnection.Close();
            //        }
        }
    }
}
