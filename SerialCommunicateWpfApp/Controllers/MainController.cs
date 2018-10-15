using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SerialCommunicateWpfApp.Entity;
using SerialCommunicateWpfApp.Models;

namespace SerialCommunicateWpfApp.Controller {
    class MainController {
        private MainModel model = new MainModel();
        public Action<string> RenderOfList { get; set; }

        public MainController() {
            model.SetDataReceiveHandler(DataReceivedHandler);
        }

        public List<string> GetPortNameList() {
            return model.GetSerialPortNames().ToList();
        }

        public void OpenPort(string portName, string baudRate) {
            if (portName == "" || baudRate == "") {
                throw new ArgumentException();
            } else {
                model.OpenSerialPort(portName, Convert.ToInt32(baudRate));
            }
        }

        public void ClosePort() {
            model.CloseSerialPort();
        }

        private void DataReceivedHandler(object sender, System.IO.Ports.SerialDataReceivedEventArgs e) {
            byte[] readBytes;
            try {
                readBytes = model.ReadFrames();
            } catch (Exception ex) {
                Console.WriteLine(ex);
                //親機に再送処理を通知する場合はここに処理を書く
                return;
            }
            model.InsertOf(CreateDevice(readBytes));

            if (RenderOfList != null && readBytes != null) {
                string readLine = readBytes[0].ToString();
                for (int i = 1; i < readBytes.Length; i++) {
                    readLine += " " + readBytes[i].ToString();
                }
                RenderOfList(readLine);
            }
        }

        private Device CreateDevice(byte[] buffer) {
            Device device = new Device();
            device.ChildId = buffer[0];
            device.AreaCode = (buffer[1] << 8) + buffer[2];
            device.DateTime = new DateTime(int.Parse($"20{buffer[3]}"), buffer[4], buffer[5], buffer[6], buffer[7], buffer[8]);
            switch (device.ChildId) {
                case DeviceChildId.CURRENT:
                    device.CurrentSwitch = (buffer[9] == 1) ? true : false;
                    break;
                case DeviceChildId.ENVIRONMENT:
                    device.Temperature = buffer[9];
                    device.Humidity = buffer[10];
                    device.Illumination = buffer[11];
                    break;
                case DeviceChildId.DUST:
                    device.Dust = buffer[9];
                    break;
                default:
                    break;
            }
            return device;
        }
    }
}
