using System.Data.SqlClient;
using Npgsql;

namespace DataAccess
{
    public class ConnectionToDB
    {
        private readonly string connectionString;

        public ConnectionToDB()
        {
            //connectionString = ConfigurationManager.ConnectionStrings["DBConnection"].ConnectionString;
            connectionString = "server=localhost\\SQLEXPRESS; database=GOKALPDB ; integrated security=true";
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }
    }
}