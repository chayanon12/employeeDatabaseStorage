using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace employeeDatabaseStorage.Data
{
    internal class Connection
    {
        public static MySqlConnection connMaster = new MySqlConnection();

        static string server = "127.0.0.1;";
        static string database = "test;";
        static string Uid = "root;";
        static string password = ";";

        public static MySqlConnection dataSource()
        {
            connMaster = new MySqlConnection($"server={server} database={database} Uid={Uid} password = {password}");
            return connMaster;
        }

        public void connOpen()
        {
            dataSource();
            connMaster.Open();
        }
        public void connClose()
        {
            dataSource();
            connMaster.Close();
        }
    }
}
