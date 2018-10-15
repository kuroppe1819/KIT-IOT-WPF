using System;
using System.Windows;
using SerialCommunicateWpfApp.Controller;

namespace SerialCommunicateWpfApp {
    public partial class MainWindow : System.Windows.Window {
        MainController controller = new MainController();
        public MainWindow() {
            InitializeComponent();
            InitView();
            controller.RenderOfList = RenderOfSerialData;
        }

        private void InitView() {
            PortNameBox.ItemsSource = controller.GetPortNameList();
            OpenBtnEnabled(true);
        }

        private void OpenBtnEnabled(Boolean enabled) {
            OpenPortBtn.IsEnabled = enabled;
            ClosePortBtn.IsEnabled = !enabled;
        }

        private void RenderOfSerialData(string readLine) {
            ReadLineList.Dispatcher.Invoke(new Action(() => {
                ReadLineList.Items.Add(readLine);
                ReadLineList.Items.Refresh();
            }));
        }

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
