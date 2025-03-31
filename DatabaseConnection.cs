using MySql.Data.MySqlClient;

partial class Program
{
    static class DatabaseConnection
    {
        public static MySqlConnection GetDatabaseConnection()
        {
            return new MySqlConnection(
                "Server=127.0.0.1;Port=3306;user=user;password=mark;" +
                "database=gameHubDb;Allow User Variables=true;");
        }
    }
}