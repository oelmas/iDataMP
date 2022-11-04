using Npgsql;
using System;
using System.Data;
using System.Data.SqlClient;
using Common.Cache;


namespace DataAccess
{
    public class UserDao : ConnectionToDB
    {
        public bool Login(string username, string password)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var command = new SqlCommand())
                {
                    command.Connection = connection;
                    command.CommandText = "SELECT * FROM users WHERE loginname = @username AND password = @password";
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    command.CommandType = CommandType.Text;
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                UserLoginCache.UserId = reader.GetInt32(0);
                                UserLoginCache.UserName = reader.GetString(3);
                                UserLoginCache.UserLastName = reader.GetString(4);
                                UserLoginCache.UserPosition = reader.GetString(5);
                                UserLoginCache.UserTypeId = reader.GetInt32(6);
                                UserLoginCache.UserEmail = reader.GetString(7);
                            }
                            return true;
                        }
                            
                        return false;
                    }
                }
            }
        }
    }
}