using MySql.Data.MySqlClient;
using SerialCommunicateWpfApp.Entity;

public sealed class DatabaseQuery {
    private string server = "localhost";        // MySQLサーバホスト名
    private string user = "sopackuser";         // MySQLユーザ名
    private string pwd = "sopa-0001";           // MySQLパスワード
    private string database = "kobayashi_iot";  // 接続するデータベース名
    private MySqlBaseConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
    private static DatabaseQuery dbQuery = new DatabaseQuery();

    public static DatabaseQuery GetInstance() {
        return dbQuery;
    }

    private DatabaseQuery() {
        builder.Server = server;
        builder.UserID = user;
        builder.Password = pwd;
        builder.Database = database;
    }

    public string Connection {
        get { return builder.ToString(); }
    }

    public string InsertToCurrentTable(Device device) {
        return string.Format(
            "insert into current (area, datetime, current_switch) values ({0}, {1}, {2})",
            device.AreaCode, device.DateTime, device.CurrentSwitch);
    }

    public string InsertToEnvironmentTable(Device device) {
        return string.Format(
            "insert into environment (area, datetime, temperature, humidity, illumination) values ({0}, {1}, {2}, {3}, {4})",
            device.AreaCode, device.DateTime, device.Temperature, device.Humidity, device.Illumination);
    }

    public string InsertToDustTable(Device device) {
        return string.Format(
            "insert into dust (area, datetime, dust) values ({0}, {1}, {2})",
            device.AreaCode, device.DateTime, device.Dust);
    }
}
