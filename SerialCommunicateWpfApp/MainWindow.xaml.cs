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
                foreach (Control control in LayoutRoot.Children) {
                    if (control is GroupBox && controller.ExcludeEndNumber(control.Name) == "GroupArea") {
                        GroupBox groupBox = control.FindName("GroupArea" + device.AreaCode.ToString()) as GroupBox;
                        RenderIntoGroupBox(groupBox, device);
                    }
                }
            }));
        }

        private void RenderIntoGroupBox(GroupBox groupBox, Device device) {
            foreach (Control control in ((Canvas)groupBox.Content).Children) {   
                switch (controller.ExcludeEndNumber(control.Name)) {
                    case "DateTimeBox":
                        TextBox datetimeBox = control.FindName(control.Name) as TextBox;
                        datetimeBox.Text = device.DateTime.ToLongTimeString();
                        break;
                    case "TempBox":
                        TextBox tempBox = control.FindName(control.Name) as TextBox;
                        tempBox.Text = device.Temperature.ToString() + "℃";
                        break;
                    case "HumidityBox":
                        TextBox humidityBox = control.FindName(control.Name) as TextBox;
                        humidityBox.Text = device.Humidity.ToString() + "%";
                        break;
                    case "IlluminationBox":
                        TextBox illuminationBox = control.FindName(control.Name) as TextBox;
                        illuminationBox.Text = device.Illumination.ToString() + "lux";
                        break;
                    case "Power":
                        ProgressBar powerProgress = control.FindName(control.Name) as ProgressBar;
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
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show("使用するポートと通信速度を設定してください");
            }
        }

        private void ClosePortBtn_Click(object sender, RoutedEventArgs e) {
            try {
                controller.ClosePort();
                OpenBtnEnabled(true);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show(ex.Message);
            }
        }

        private async void ExportBtn_Click(object sender, RoutedEventArgs e) {
            if (ClosePortBtn.IsEnabled) {
                //シリアルポートが開いている場合
                ClosePortBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, ClosePortBtn)); //CloseButtonのクリックイベントを発生させる
            }
            await controller.ExportSerialDataAsync();
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

        protected virtual void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            controller.WindowClosing();
        }
    }
}
