using System;
using System.Configuration;
using System.Data.SqlClient;
namespace Hastane_Proje
{
    public class sqlbaglantisi
    {
        public static string ConnectionString
        {
            get
            {
                var local = Environment.GetEnvironmentVariable("HASTANE_CONNECTION_STRING");
                return string.IsNullOrWhiteSpace(local)
                    ? ConfigurationManager.ConnectionStrings["Hastane_Proje.Properties.Settings.HastaneProjeConnectionString"].ConnectionString
                    : local;
            }
        }
        public SqlConnection baglanti()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }
}
