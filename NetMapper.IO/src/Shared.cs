using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace NetMapper.IO
{
    public class SharedFunctions
    {
        public static String sqlString()
        {
            Boolean debug = false; // Flag to true (I use this for local DB testing)

            SqlConnectionStringBuilder sqlConnectionStringBuilder = new SqlConnectionStringBuilder();
            if (!debug)
            {
                sqlConnectionStringBuilder.UserID = ""; // Live Database User ID
                sqlConnectionStringBuilder.DataSource = ""; // Live Database Source
                sqlConnectionStringBuilder.Password = ""; // Live Database Password
                sqlConnectionStringBuilder.InitialCatalog = ""; // Live Database DB
                sqlConnectionStringBuilder.IntegratedSecurity = false;
            }
            else
            {
                sqlConnectionStringBuilder.UserID = "";
                sqlConnectionStringBuilder.DataSource = "";
                sqlConnectionStringBuilder.Password = "";
                sqlConnectionStringBuilder.InitialCatalog = "";
                sqlConnectionStringBuilder.IntegratedSecurity = false;
            }
            return sqlConnectionStringBuilder.ConnectionString;
        }
        public static string sha256(string randomString)
        {
            var crypt = new SHA256Managed();
            string hash = String.Empty;
            byte[] crypto = crypt.ComputeHash(Encoding.ASCII.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash += theByte.ToString("x2");
            }
            return hash;
        }
    }
    public struct NetIconObject
    {
        public Int32 NetIconId { get; set; }
        public String IconPath { get; set; }
        public String IconName { get; set; }
    }
}