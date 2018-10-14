using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

static class DBConfig {
    private static string server = "localhost";        // MySQLサーバホスト名
    private static string user = "sopackuser";         // MySQLユーザ名
    private static string pwd = "sopa-0001";           // MySQLパスワード
    private static string database = "kobayashi_iot";  // 接続するデータベース名

    public static string CONNECTION_STRING {
        get { return string.Format("Server={0};Database={1};Uid={2};Pwd={3}", server, database, user, pwd); }
    }
}
