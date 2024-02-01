using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace Products
{
    
    public class DataBase
    {
       NpgsqlConnection npgsqlConnection = new NpgsqlConnection("Server = localhost; port = 5432; Database = Products_bd; User Id = postgres; Password = 123");

        public void OpenConnecting()
        {
            if(npgsqlConnection.State == System.Data.ConnectionState.Closed)
            {
                npgsqlConnection.Open();
            }
        }

        public void CloseConnecting()
        {
            if(npgsqlConnection.State == System.Data.ConnectionState.Open)
            {
                npgsqlConnection.Close();
            }
        }

        public NpgsqlConnection GetConnecting()
        {
            return npgsqlConnection;
        }

    }


}
