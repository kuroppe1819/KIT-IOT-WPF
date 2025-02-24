﻿using System;
using System.Text;
using System.IO.Ports;

class CustomSerialPort : SerialPort {
    private SerialDataReceivedEventHandler DataReceivedHandler { get; set; }
    private const int BYTES_THRESHOLD = 14;

    public CustomSerialPort() {
        DataBits = 8;
        Parity = Parity.None;
        StopBits = StopBits.One;
        Encoding = Encoding.UTF8;
        ReadTimeout = 100000; //100秒
        ReceivedBytesThreshold = BYTES_THRESHOLD; //DataReceivedイベントが発生するバッファのバイト数を設定する。
    }

    public void SetDataReceivedHandler(SerialDataReceivedEventHandler handler) {
        DataReceivedHandler = handler;
        DataReceived += handler;
    }

    public void DisposeDataReceivedHandler() {
        DataReceived += DataReceivedHandler;
    }

    public void Open(string portName, int baudRate) {
        PortName = portName;
        BaudRate = baudRate;
        Open();
    }

    public byte[] ReadFrames() {
        int readCount = 0;
        while (ReadByte() != 0xFF) { //スタートビットを受信するまで調整する
            readCount++;
            if (readCount == BYTES_THRESHOLD) {
                throw new TimeoutException();
            }
        }
        int frameLength = ReadByte(); //フレーム長を読み取る
        var buffer = new byte[frameLength];
        Read(buffer, 0, buffer.Length); //フレーム長とチェックサムに挟まれた分だけ読み取る
        int checksum = ReadByte(); //チェックサムを読み取る
        int stopbit = ReadByte(); //ストップビットを読み取る

        if (checksum == CalcChecksum(buffer)) {
            return buffer;
        } else {
            throw new InvalidReadByteException();
        }
    }

    public int CalcChecksum(byte[] buffer) {
        int sum = 0;
        for (int i = 0; i < buffer.Length; i++) {
            sum += buffer[i];
        }
        return 0xFF - (sum & 0xFF);
    }
}
