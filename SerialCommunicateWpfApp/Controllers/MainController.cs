﻿using SerialCommunicateWpfApp.Entity;
using SerialCommunicateWpfApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SerialCommunicateWpfApp.Controller {
    class MainController {
        private MainModel model = new MainModel();
        public Action<Device> RenderOfSerialData { get; set; }

        public MainController() {
            model.SetDataReceivedHandler(DataReceivedHandler);
        }

        public string ExcludeEndNumber(string str) {
            if (str.Length == 0) {
                return str;
            }

            Int32 number;
            int endIndex = str.Length - 1;
            if (Int32.TryParse(str.Substring(endIndex), out number)) { //末尾の文字が数値に変換できるかチェックする
                string subString = str.Substring(0, endIndex); //コントロール名から番号を削除する
                return ExcludeEndNumber(subString);
            } else {
                return str;
            }
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

        public async Task ClosePortAsync() {
            await Task.Run(() => {
                model.CloseSerialPort();
                model.CloseDatabase();
            });
        }

        private void DataReceivedHandler(object sender, System.IO.Ports.SerialDataReceivedEventArgs e) {
            byte[] readBytes = null;
            try {
                readBytes = model.ReadFrames();
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                //親機に再送処理を通知する場合はここに処理を書く
                return;
            }

            if (RenderOfSerialData != null && readBytes != null) {
                Device device = CreateDevice(readBytes);
                try {
                    model.InsertOf(device);
                    RenderOfSerialData(device);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        private Device CreateDevice(byte[] buffer) {
            Device device = new Device();
            device.ChildId = buffer[0];
            device.AreaCode = (buffer[1] << 8) + buffer[2]; //上位桁と下位桁を結合
            device.DateTime = DateTime.Now;
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

        public async Task ExportSerialDataAsync() {
            await Task.Run(async () => { //SQL実行中にクローズしないようにするため遅延処理を行う
                await Task.Delay(1000); //1000ms待機
                try {
                    model.ExportToWorksheet();
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            });
        }

        public void CancelExport() {
            model.Canceling = true;
        }

        public void WindowClosing() {
            Task.Run(async () => { //SQL実行中にクローズしないようにするため遅延処理を行う
                model.CloseSerialPort();
                model.DisposeReceivedHandler();
                await Task.Delay(3000); //3000ms待機
                model.CloseDatabase();
            });
        }
    }
}
