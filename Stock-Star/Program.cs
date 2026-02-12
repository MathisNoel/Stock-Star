using System;
using System.Collections.Generic;

//On inclue la librairie
using Npgsql;
using NpgsqlTypes;
//Fin
using System.Linq;
using System.Data;
using System.Text;


namespace Stock_Star
{
    public class produits
    {
            //Connection a Npgsql
            NpgsqlConnection myConnection = null;
            string connString = "Server=localhost;Port=5432;Database=produits;User Id=user;Password=user;";
            myConnection = new NpgsqlConnection(connString);
            myConnection.open();
    }

        internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }
}