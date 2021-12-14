using System;

namespace Auth_Service.Data
{
    public static class ConnectionStringUtil
    {
        public static string GetConnectionString()
        {
            string host = Environment.GetEnvironmentVariable("DB_HOST");
            string port = Environment.GetEnvironmentVariable("DB_PORT");
            string name = Environment.GetEnvironmentVariable("DB_NAME");
            string username = Environment.GetEnvironmentVariable("DB_USERNAME");
            string password = Environment.GetEnvironmentVariable("DB_PASSWORD");

            return $"Host={host};Username={username};Password={password};Database={name}";
            //return $"Data Source={host};Initial Catalog={name};User Id={username};Password={password}";
        }
    }
}