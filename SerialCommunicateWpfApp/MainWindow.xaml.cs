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
                        if (device.ChildId == DeviceChildId.ENVIRONMENT) {
                            TextBox tempBox = control.FindName(control.Name) as TextBox;
                            tempBox.Text = (device.Temperature == Device.EMPTY) ? "" : $"{device.Temperature.ToString()}℃";
                        }
                        break;
                    case "HumidityBox":
                        if (device.ChildId == DeviceChildId.ENVIRONMENT) {
                            TextBox humidityBox = control.FindName(control.Name) as TextBox;
                            humidityBox.Text = (device.Humidity == Device.EMPTY) ? "" : $"{device.Humidity.ToString()}%";
                        }
                        break;
                    case "IlluminationBox":
                        if (device.ChildId == DeviceChildId.ENVIRONMENT) {
                            TextBox illuminationBox = control.FindName(control.Name) as TextBox;
                            illuminationBox.Text = (device.Illumination == Device.EMPTY) ? "" : $"{device.Illumination.ToString()}lux";
                        }
                        break;
                    case "DustBox":
                        if (device.ChildId == DeviceChildId.DUST) {
                            TextBox dustBox = control.FindName(control.Name) as TextBox;
                            dustBox.Text = (device.Dust == Device.EMPTY) ? "" : device.Dust.ToString();
                        }
                        break;
                    case "Power":
                        if (device.ChildId == DeviceChildId.CURRENT) {
                            ProgressBar powerProgress = control.FindName(control.Name) as ProgressBar;
                            powerProgress.Value = (device.CurrentSwitch) ? 100 : 0;
                        }
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
