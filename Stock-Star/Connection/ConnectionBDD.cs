using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace Stock_Star
{
    internal class ConnectionBDD
    {
        string connString = "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=sudo;";
        public NpgsqlConnection GetConnection() {
            return new NpgsqlConnection(connString);
        }       
    }
}
