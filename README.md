# KIT-IoT-WPF
- デバイスの情報を監視してデータベースに記録するWindowsのアプリケーション

## Requirement
- MySQL Server 8.0
- Excel

## Development
- VisualStudio 2017
- MySQL Server 8.0
- MySQL Connector/NET 8.0
- Python 3.6
- Excel

## Install
1. MySQL Serverをインストールする。
1. rootでログインしたあとsourceコマンドを使ってCreateKobayashiIoT.sqlファイルを読みこむ。
1. 親機をPCに接続する。
1. SerialCommunicateWpfApp.exeを実行する。

## Usage
1. `PortName`をクリックして親機からシリアルデータを受け取るポートを選択する。
1. 子機とのシリアル通信速度は9600なので`BaundRate`をクリックして`9600`を選択する。
1. `OpenPort`をクリックすると親機からのデータを読みこんでデータベースに値を保存する処理が実行される。
1. `Export`を押すとPublicフォルダ下にデータベースの情報をエクセルに出力した`KobayashiIotSensors.xlsx`が生成される。
