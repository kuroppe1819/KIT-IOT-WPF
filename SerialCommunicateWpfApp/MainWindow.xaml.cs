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
                        int temp = device.Temperature;
                        tempBox.Text = (temp == Device.EMPTY) ? "" : $"{temp.ToString()}℃";
                        break;
                    case "HumidityBox":
                        TextBox humidityBox = control.FindName(control.Name) as TextBox;
                        int humidity = device.Humidity;
                        humidityBox.Text = (humidity == Device.EMPTY) ? "" : $"{humidity.ToString()}%";
                        break;
                    case "IlluminationBox":
                        TextBox illuminationBox = control.FindName(control.Name) as TextBox;
                        int illumination = device.Illumination;
                        illuminationBox.Text = (illumination == Device.EMPTY) ? "" : $"{illumination.ToString()}lux";
                        break;
                    case "DustBox":
                        TextBox dustBox = control.FindName(control.Name) as TextBox;
                        int dust = device.Dust;
                        dustBox.Text = (dust == Device.EMPTY) ? "" : dust.ToString();
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

        private async void ClosePortBtn_Click(object sender, RoutedEventArgs e) {
            try {
                ClosePortBtn.IsEnabled = false;
                await controller.ClosePortAsync();
                OpenBtnEnabled(true);
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                MessageBox.Show(ex.Message);
            }
        }

        private async void ExportBtn_Click(object sender, RoutedEventArgs e) {
            this.IsEnabled = false;
            if (ClosePortBtn.IsEnabled) {
                //シリアルポートが開いている場合
                ClosePortBtn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent, ClosePortBtn)); //CloseButtonのクリックイベントを発生させる
            }
            var exportingWindow = new ExportingWindow();
            exportingWindow.CancelBtnClicked = CancelBtn_Click;
            exportingWindow.Show();
            await controller.ExportSerialDataAsync();
            this.IsEnabled = true;
            exportingWindow.Close();
        }

        private void CancelBtn_Click() {
            controller.CancelExport();
            this.IsEnabled = true;
        }

        protected virtual void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            controller.WindowClosing();
        }
    }
}
