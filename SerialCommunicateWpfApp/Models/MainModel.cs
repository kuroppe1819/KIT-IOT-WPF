using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace SerialCommunicateWpfApp.Models {
    class MainModel {
        private CustomSerialPort serialPort = new CustomSerialPort();

        public void SetDataReceiveHandler(Action<object, SerialDataReceivedEventArgs> handler) {
            serialPort.SetDataReceiveHandler(handler);
        }

        public string[] GetSerialPortNames() {
            return CustomSerialPort.GetPortNames();
        }

        public void OpenSerialPort(string portName, int baudRate) {
            serialPort.Open(portName, baudRate);
        }

        public void CloseSerialPort() {
            if (serialPort.IsOpen) {
                serialPort.Close();
            }
        }

        public byte[] ReadFrames() {
            return serialPort.ReadFrames();
        }
    }
}
