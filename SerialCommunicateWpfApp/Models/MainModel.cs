﻿using System;
using System.Data;
using System.IO.Ports;
using MySql.Data.MySqlClient;
using SerialCommunicateWpfApp.Entity;
using Microsoft.Office.Interop.Excel;
using System.Linq;

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

        public void InsertOf(Device device) {
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
                connection.Open();
                command.ExecuteNonQuery();
            } catch (MySqlException ex) {
                throw ex;
            } finally {
                connection.Close();
            }
        }

        public MySqlDataReader SelectFrom(string tableName, string column) {
            command.CommandText = dbQuery.SelectFrom(tableName, "all");
            try {
                connection.Open();
                return command.ExecuteReader();
            } catch (MySqlException ex) {
                throw ex;
            }
        }

        public void ExportToWorksheet() {
            Application excelApp = new Application();
            excelApp.Visible = true; //書き出すときにExcelを表示しない
            excelApp.WindowState = XlWindowState.xlMaximized; //windowを最大化する
            Workbook workbook = excelApp.Workbooks.Add(XlWBATemplate.xlWBATWorksheet); //ワークシートが追加されているworkbookを作成
            foreach (var table in DatabaseTable.Names.Select((name, index) => new { name, index })) {
                Worksheet worksheet = workbook.Worksheets[table.index + 1]; //シートの番号
                try {
                    MySqlDataReader reader = SelectFrom(table.name, DatabaseTable.Column.ALL);
                    var columns = Enumerable.Range('A', reader.FieldCount)
                        .Select(alphabet => (char)alphabet)
                        .Select((alphabet, index) => new { alphabet, index })
                        .ToArray();
                    //カラム名を取得してExcelの1行目に追加する
                    foreach (var column in columns) {
                        worksheet.Range[$"{column.alphabet}1"].Value = reader.GetName(column.index);
                    }
                    //テーブルを出力してExcelに書き込む
                    int row = 2;
                    while (reader.Read()) {
                        foreach (var colum in columns) {
                            worksheet.Range[$"{colum.alphabet}{row}"].Value = reader.GetString(colum.index);
                        }
                        row++;
                    }
                    reader.Close();
                    //TODO: 保存先をAppData以下にする？
                    workbook.SaveAs(@"C:\Users\atsusuke\WorkSpace\source\repos\SerialCommunicateWpfApp\SerialCommunicateWpfApp\SensorData.xlsx");
                } catch (Exception ex) {
                    throw ex;
                } finally {
                    connection.Close();
                }
            }
        }
    }
}
