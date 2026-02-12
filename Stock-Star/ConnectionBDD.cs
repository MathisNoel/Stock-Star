using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace Stock_Star
{
    internal class ConnectionBDD
    {
        string connString = "Server=localhost;Port=5432;Database=Stock-Star;User Id=user;Password=user;";
        public NpgsqlConnection GetConnection() {
            return new NpgsqlConnection(connString);
        }
        /*
        NpgsqlConnection myConnection = new NpgsqlConnection(connString);
        myConnection.Open();
        */  
    }
}
