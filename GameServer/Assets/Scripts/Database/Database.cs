using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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

                            }
                        }
                    }
                }

                connection.Close();
            }

            return data;
        });
        return await task;
    }

    public async static Task<List<int>> UpdateMatchesState()
    {
        Task<List<int>> task = Task.Run(() => {

            List<int> matches_ids = null;
            using (MySqlConnection connection = GetMysqlConnection())
            {
                string query = String.Format("SELECT match_id FROM game WHERE match_state = '{0}'", (int)MatchState.NOT_READY);
                
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                if(matches_ids == null)
                                    matches_ids = new List<int>();

                                int.TryParse(reader["match_id"].ToString(), out int match_id);
                                matches_ids.Add(match_id);
                            }
                        }
                    }
                }
            
             /*   if(matches_ids != null)
                {
                    foreach (int match_id in matches_ids)
                    {
                        string qry = String.Format("UPDATE game SET match_state = {0} " +
                            "WHERE match_id = {1};", (int)MatchState.READY, match_id);
                        using (MySqlCommand command = new MySqlCommand(qry, connection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }
                }*/
                connection.Close();
            }
            return matches_ids;
        });
        return await task;
    }
}
