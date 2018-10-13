using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SerialCommunicateWpfApp.Models;

namespace SerialCommunicateWpfApp.Controller {
    class MainController {
        private MainModel model = new MainModel();
        private Action<string> renderOfList;
        public Action<string> RenderOfList {
            set { renderOfList = value; }
            get { return renderOfList; }
        }

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

            if (renderOfList != null && readBytes != null) {
                string readLine = readBytes[0].ToString();
                for (int i = 1; i < readBytes.Length; i++) {
                    readLine += " " + readBytes[i].ToString();
                }
                renderOfList(readLine);
            }
        }
    }
}
