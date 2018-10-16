using System;
using System.Data;
using System.IO.Ports;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using SerialCommunicateWpfApp.Entity;

namespace SerialCommunicateWpfApp.Models {
    class MainModel {
        private CustomSerialPort serialPort = new CustomSerialPort();
        private DatabaseQuery dbQuery = DatabaseQuery.GetInstance();
        private MySqlConnection connection = new MySqlConnection();
        private MySqlCommand command = new MySqlCommand();

        public MainModel() {
            connection.ConnectionString = dbQuery.Connection;
            command.Connection = connection;
        }

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

        public void OpenDatabase() {
            if (connection.State == ConnectionState.Closed) {
                connection.Open();
            }
        }

        public void CloseDatabase() {
            if (connection.State == ConnectionState.Open) {
                connection.Close();
            }
        }

        public async void InsertOf(Device device) {
            switch (device.ChildId) {
                case DeviceChildId.CURRENT:
                    command.CommandText = dbQuery.InsertToCurrentTable(device);
                    break;
                case DeviceChildId.ENVIRONMENT:
                    command.CommandText = dbQuery.InsertToEnvironmentTable(device);
                    break;
                case DeviceChildId.DUST:
                    command.CommandText = dbQuery.InsertToDustTable(device);
                    break;
                default:
                    throw new InvalidChildIdException();
            }
            try {
                //await Task.Run(() => command.ExecuteNonQuery());
            } catch (MySqlException ex) {
                throw ex;
            }
        }
    }
}
