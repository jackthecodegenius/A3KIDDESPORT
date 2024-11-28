using System.Configuration;
using System.Data.SqlClient;

namespace DataManagement
{
    public static class Helper
    {
        /// <summary>
        /// Reads the App.config file and returns the details of the connection string matching the
        /// provided name.
        /// </summary>
        /// <param name="name">The name of the desired connection string</param>
        /// <returns>A string containing all the connection string details.</returns>
        private static string GetConnectionString(string teamName)
        {
            return ConfigurationManager.ConnectionStrings[teamName].ConnectionString;
        }

        /// <summary>
        /// Creates an SqlConnection object which is used for connecting to an SQL Server database.
        /// As part of this it retrieves the required connection string via the GetConnectionString
        /// method.
        /// </summary>
        /// <param name="name">The name of the desired connection string.</param>
        /// <returns>A configured Sql Server connection object.</returns>
        public static SqlConnection GetSQLServerConnection(string teamName)
        {
            return new SqlConnection(GetConnectionString(teamName));
        }

    }
}
