using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data.SQLite;


namespace MBKC.Repository.Repositories
{
    public class HangfireRepository
    {
        private const string PREFIX_ID = "recurring-job:";
        private const string CONNECTION_STRING = "Data Source=hangfire.db";

        #region get cron
        public string? GetCronByKey(string key)
        {
            try
            {
                string? data = null;
                using (var sqlite_conn = new SQLiteConnection(CONNECTION_STRING))
                {
                    sqlite_conn.Open();
                    SQLiteDataReader sqlite_datareader;
                    SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();
                    sqlite_cmd.CommandText = $"SELECT Value FROM Hash WHERE Key='{PREFIX_ID}{key}' AND Field='Cron'";
                    sqlite_datareader = sqlite_cmd.ExecuteReader();
                    while (sqlite_datareader.Read())
                    {
                        data = sqlite_datareader.GetString(0);
                    }

                    sqlite_conn.Close();
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region update cron
        public int UpdateCron(string key, string newCron)
        {
            try
            {
                int numberOfRowsAffected;
                using (var sqlite_conn = new SQLiteConnection(CONNECTION_STRING))
                {
                    sqlite_conn.Open();
                    SQLiteCommand sqlite_cmd = sqlite_conn.CreateCommand();
                    sqlite_cmd.CommandText = $"UPDATE Hash SET Value='{newCron}' WHERE Key='{PREFIX_ID}{key}' AND Field='Cron'";
                    numberOfRowsAffected = sqlite_cmd.ExecuteNonQuery();
                }
                return numberOfRowsAffected;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
