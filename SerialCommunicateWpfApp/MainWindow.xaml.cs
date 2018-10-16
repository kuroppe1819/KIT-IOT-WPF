using System;
using System.Windows;
using System.Windows.Controls;
using SerialCommunicateWpfApp.Controller;
using SerialCommunicateWpfApp.Entity;

namespace SerialCommunicateWpfApp {
    public partial class MainWindow : System.Windows.Window {
        MainController controller = new MainController();
        public MainWindow() {
            InitializeComponent();
            InitView();
            controller.RenderOfSerialData = RenderOfSerialData;
        }

        private void InitView() {
            PortNameBox.ItemsSource = controller.GetPortNameList();
            OpenBtnEnabled(true);
        }

        private void OpenBtnEnabled(Boolean enabled) {
            OpenPortBtn.IsEnabled = enabled;
            ClosePortBtn.IsEnabled = !enabled;
        }

        private void RenderOfSerialData(Device device) {
            this.Dispatcher.Invoke(new Action(() => {
                switch (device.AreaCode) {
                    case 0:
                        RenderIntoGroupBox(GroupArea0, device);
                        break;
                    case 1:
                        RenderIntoGroupBox(GroupArea1, device);
                        break;
                    case 2:
                        RenderIntoGroupBox(GroupArea2, device);
                        break;
                    default:
                        break;
                }
            }));
        }

        private void RenderIntoGroupBox(GroupBox groupBox, Device device) {
            foreach (Control content in ((Canvas)groupBox.Content).Children) {
                if (content.Name.Length == 0) {
                    continue;
                }
                string contentName = content.Name.Substring(0, content.Name.Length - 1); //コントロール名から番号を削除する
                switch (contentName) {
                    case "DateTimeBox":
                        TextBox datetimeBox = content.FindName(content.Name) as TextBox;
                        datetimeBox.Text = device.DateTime.ToLongTimeString();
                        break;
                    case "TempBox":
                        TextBox tempBox = content.FindName(content.Name) as TextBox;
                        tempBox.Text = device.Temperature.ToString() + "℃";
                        break;
                    case "HumidityBox":
                        TextBox humidityBox = content.FindName(content.Name) as TextBox;
                        humidityBox.Text = device.Humidity.ToString() + "%";
                        break;
                    case "IlluminationBox":
                        TextBox illuminationBox = content.FindName(content.Name) as TextBox;
                        illuminationBox.Text = device.Illumination.ToString() + "lux";
                        break;
                    case "Power":
                        ProgressBar powerProgress = content.FindName(content.Name) as ProgressBar;
                        powerProgress.Value = (device.CurrentSwitch) ? 100 : 0;
                        break;
                    default:
                        break;
                }
            };
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
