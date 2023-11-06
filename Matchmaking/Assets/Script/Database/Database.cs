using MySql.Data.MySqlClient;
using Riptide;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class Database  
{
    private const string _mysqlServer = "localhost";
    private const string _mysqlUsername = "root";
    private const string _mysqlPassword = ""; // Kassker98
    private const string _mysqlDatabase = "hexchess";
    private const UInt16 _mysqlPort = 3306;

    public static MySqlConnection GetMysqlConnection()
    {
        MySqlConnectionStringBuilder connString = new MySqlConnectionStringBuilder();
        connString.Server = _mysqlServer;
        connString.UserID = _mysqlUsername;
        connString.Password = _mysqlPassword;
        connString.Port = _mysqlPort;
        connString.Database = _mysqlDatabase;
        connString.CharacterSet = "utf8mb4"; // utf8mb4
        MySqlConnection _mysqlConnection = new MySqlConnection(connString.ToString());
        try
        {
            _mysqlConnection.Open();
        }
        catch (Exception e)
        {
            Console.WriteLine("MESSAGE: " + e.Message);
            Console.WriteLine("STACK TRACE: " + e.StackTrace);
            Console.WriteLine("SOURCE: " + e.Source);
            Console.WriteLine("DICTIONARY DATA: " + e.Data);
            Console.WriteLine("HELP LINK: " + e.HelpLink);
        }
        return _mysqlConnection;
    }

    public async static Task<PlayerData> AuthenticatePlayerAsync(string deviceID)
    {
        Task<PlayerData> task = Task.Run(() => {

            PlayerData data = null;

            using (MySqlConnection connection = GetMysqlConnection())
            {
                string query = String.Format("SELECT acc_id, nickname, `rank`, selected_class FROM player WHERE device_id = '{0}'", deviceID);
                bool account_found = false;
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                data = new PlayerData
                                {
                                    account_id = int.Parse(reader["acc_id"].ToString()),
                                    nickname = reader["nickname"].ToString(),
                                    rank = int.Parse(reader["rank"].ToString()),
                                    class_type = (ClassType)int.Parse(reader["selected_class"].ToString())

                                };
                                account_found = true;

                            }
                        }
                    }
                }

                if(!account_found)
                {
                    string start_nickname = "New Player - " + new Random().Next(1000, 10001).ToString();
                    query = String.Format("INSERT INTO player (device_id, nickname, selected_class) VALUES('{0}', '{1}', {2})", deviceID, start_nickname, (int)ClassType.None);
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        data = new PlayerData
                        {
                            account_id = command.LastInsertedId,
                            nickname = start_nickname,
                            class_type = ClassType.None,
                            rank = 500
                        };
                    }
                    
                        UnityEngine.Debug.Log("CREATED ACC !!!");
                }
                connection.Close();
            }

            return data;
        });
        return await task;
    }

    public async static Task<int> CheckIsThereAGameThatIsNotFinished(Player player)
    {
        Task<int> task = Task.Run(() => {

            int match_id = 0;
            using (MySqlConnection connection = GetMysqlConnection())
            {
                string query = String.Format("SELECT match_id FROM game WHERE (player1_acc = {0} OR player2_acc = {0}) AND match_state = {1}", player.data.account_id,(int)MatchState.READY);

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                match_id = int.Parse(reader["match_id"].ToString());
                            }
                        }
                    }
                }
                connection.Close();
            }

            return match_id;
        });
        return await task;
    }

    public async static Task UpdatePlayerNickname(string nickname, Player player)
    {
        Task task = Task.Run(() =>
        {
            using (MySqlConnection connection = GetMysqlConnection())
            {
                string query = String.Format("UPDATE player SET nickname = '{0}' WHERE acc_id = {1};", nickname, player.data.account_id);
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        });
        await task;
    }

    public async static Task<int> CreateMatchAsync(Player player1, Player player2)
    {
        Task<int> task = Task.Run(() =>
        {
            long LastInsertedId = -1;
            using (MySqlConnection connection = GetMysqlConnection())
            {
                string query = String.Format("INSERT INTO game (player1_acc, player1_class, player2_acc, player2_class) VALUES({0}, {1}, {2}, {3})", player1.data.account_id, (int)player1.data.class_type, player2.data.account_id, (int)player2.data.class_type);
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                    LastInsertedId = command.LastInsertedId;
                }
                connection.Close();
            }
            return (int)LastInsertedId;
        });
        return await task;
    }

    public async static Task<int> UpdatePlayerClassType(Player player)
    {
        Task<int> task = Task.Run(() =>
        {
            using (MySqlConnection connection = GetMysqlConnection())
            {
                string query = String.Format("UPDATE player SET selected_class = {0} " +
                    "WHERE acc_id = {1};"
                    , (int)player.data.class_type, player.data.account_id);
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            return 1;
        });
        return await task;
    }
}
