class DatabaseTable {
    public const string CURRENT = "current";
    public const string ENVIRONMENT = "environment";
    public const string DUST = "dust";

    public static string[] Names {
        get { return new string[] { CURRENT, ENVIRONMENT, DUST }; }
    }

    public static class Column {
        public const string ALL = "*";
        public const string ID = "id";
        public const string AREA = "area";
        public const string DATETIME = "datetime";
        public const string CURRENT = "current";
        public const string TEMPERATURE = "temperature";
        public const string HUMIDITY = "humidity";
        public const string ILLUMINATION = "illumination";
        public const string DUST = "dust";
    }
}
