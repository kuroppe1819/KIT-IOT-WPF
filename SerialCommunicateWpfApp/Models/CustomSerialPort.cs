using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace SerialCommunicateWpfApp.Models {
    class CustomSerialPort : SerialPort {
        public CustomSerialPort() {
            DataBits = 8;
            Parity = Parity.None;
            StopBits = StopBits.One;
            Encoding = Encoding.UTF8;
            ReadTimeout = 100000; //ミリ秒単位
        }

        public void SetDataReceiveHandler(Action<object, SerialDataReceivedEventArgs> handler) {
            DataReceived += new SerialDataReceivedEventHandler(handler);
        }

        public void Open(string portName, int baudRate) {
            PortName = portName;
            BaudRate = baudRate;
            Open();
        }

        public byte[] ReadFrames() {
            while (BytesToRead < 16) { } //バッファに16バイト以上のシリアルデータが蓄積されるまで待機する
            while (ReadByte() != 0xFF) { } //スタートビットを受信するまで調整する
            int frameLength = ReadByte(); //フレーム長を読み取る
            var buffer = new byte[frameLength]; 
            Read(buffer, 0, buffer.Length); //フレーム長とチェックサムに挟まれた分だけ読み取る
            int checksum = ReadByte(); //チェックサムを読み取る
            int stopbit = ReadByte(); //ストップビットを読み取る

            if (checksum == CustomSerialPort.CalcChecksum(buffer)) {
                return buffer;
            } else {
                throw new InvalidReadByteException();
            }
        }

        public static int CalcChecksum(byte[] buffer) {
            int sum = 0;
            for (int i = 0; i < buffer.Length; i++) {
                sum += buffer[i];
            }
            return 0xFF - (sum & 0xFF);
        }
    }
}
