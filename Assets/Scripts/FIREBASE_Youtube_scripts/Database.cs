using MySql.Data.MySqlClient;
using System;
public class Database 
{
    public static readonly string CONNECTION_STRING = "Server=185.87.194.44;Port=3333;Database=fluffyunicorn;Uid=fluffyunicorn;Pwd=CO0I0hL7qHgj76cWWXWcKVfF5iuIod1i;";
    public static int UserId;
    public static void SendUserToDB(string nikname, string login)
    {
        MySqlConnection dbConnection = new MySqlConnection(CONNECTION_STRING);
        dbConnection.Open();

        MySqlCommand dbCommand = dbConnection.CreateCommand();
        string sqlQuery = $"INSERT INTO Users (name, role, login, coins) VALUES ('{nikname}','user','{login}',0); SELECT LAST_INSERT_ID();";

        dbCommand.CommandText = sqlQuery;
        UserId = Convert.ToInt32(dbCommand.ExecuteScalar());

        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;

        //запись в таблице Killings
        dbConnection = new MySqlConnection(CONNECTION_STRING);
        dbConnection.Open();

        dbCommand = dbConnection.CreateCommand();
        sqlQuery = $"INSERT INTO Killings (id_user, id_location, boss_is_killed) VALUES ('{UserId}', 1, 0);";

        dbCommand.CommandText = sqlQuery;
        dbCommand.ExecuteNonQuery();

        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;
    }

    public static bool FindUserByDB(string nikname, string login)    
    {
        MySqlConnection dbConnection = new MySqlConnection(CONNECTION_STRING);
        dbConnection.Open();

        MySqlCommand dbCommand = dbConnection.CreateCommand();
        string sqlQuery = $"SELECT COUNT(*) FROM Users WHERE name = '{nikname}' AND login = '{login}'";

        dbCommand.CommandText = sqlQuery;
        int count = Convert.ToInt32(dbCommand.ExecuteScalar());

        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;

        return count > 0;
    }

    public static bool FindUserNimnameByDB(string nikname)
    {
        MySqlConnection dbConnection = new MySqlConnection(CONNECTION_STRING);
        dbConnection.Open();

        MySqlCommand dbCommand = dbConnection.CreateCommand();
        string sqlQuery = $"SELECT COUNT(*) FROM Users WHERE name = '{nikname}'";

        dbCommand.CommandText = sqlQuery;
        int count = Convert.ToInt32(dbCommand.ExecuteScalar());

        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;

        return count == 0;
    }

    public static void FindIdUserOnAutorization(string nikname, string login)
    {
        MySqlConnection dbConnection = new MySqlConnection(CONNECTION_STRING);
        dbConnection.Open();

        MySqlCommand dbCommand = dbConnection.CreateCommand();
        string sqlQuery = $"SELECT id_user FROM Users WHERE name = '{nikname}' AND login = '{login}'";

        dbCommand.CommandText = sqlQuery;
        UserId = Convert.ToInt32(dbCommand.ExecuteScalar());

        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;
    }
}
