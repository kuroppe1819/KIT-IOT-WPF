using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

public sealed class DBConfig {
    private string server = "localhost";        // MySQLサーバホスト名
    private string user = "sopackuser";         // MySQLユーザ名
    private string pwd = "sopa-0001";           // MySQLパスワード
    private string database = "kobayashi_iot";  // 接続するデータベース名
    //public static string CONNECTION_STRING {
    //    get { return string.Format("Server={0};Database={1};Uid={2};Pwd={3}", server, database, user, pwd); }
    //}
    private static DBConfig dbConfig = new DBConfig();
    private MySqlBaseConnectionStringBuilder builder = new MySqlConnectionStringBuilder();

    public static DBConfig GetInstance() {
        return dbConfig;
    }

    private DBConfig() {
        builder.Server = server;
        builder.UserID = user;
        builder.Password = pwd;
        builder.Database = database;
    }

    public string GetConnectionString {
        get{ return builder.ToString(); }
    }
}
