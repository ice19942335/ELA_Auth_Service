using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;

namespace ELA_Auth_Service._Data._MySqlDataContext
{
    public class MySqlDataContext : IDisposable
    {
        public MySqlDataContext(string connectionString)
        {
            Connection = new MySqlConnection(connectionString);
            Connection.Open();
        }

        public MySqlConnection Connection { get; set; }

        public async Task<bool> CreateUser(Guid guid, string name, int points)
        {
            int? rowsAffected;

            var cmd = Connection.CreateCommand();
            cmd.CommandText = $@"INSERT INTO Users(userId,first_name,points,reg_date) VALUES('{guid.ToString()}','{name}',{points},'{DateTime.Now:yyyy-MM-dd H:mm:ss}');";
            try
            {
                rowsAffected = await cmd.ExecuteNonQueryAsync();
            }
            catch
            {
                return false;
            }

            if (rowsAffected < 1)
                return false;

            return true;
        }

        public void Dispose()
        {
            Connection.Close();
        }
    }
}
