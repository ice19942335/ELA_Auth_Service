using System;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace ELA_Auth_Service.Data._MySqlDataContext
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
            var cmd = Connection.CreateCommand();
            cmd.CommandText = $@"INSERT INTO Users(userId,first_name,points,reg_date) VALUES('{guid.ToString()}','{name}',{points},'{DateTime.Now.ToString("yyyy-MM-dd H:mm:ss")}');";
            var rowsAffected = await cmd.ExecuteNonQueryAsync();

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
