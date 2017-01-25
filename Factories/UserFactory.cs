using System.Collections.Generic;
using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using wall.Models;
using Microsoft.Extensions.Options;

namespace wall.Factory
{
    public class UserFactory
    {
        private readonly IOptions<MySqlOptions> mysqlConfig;
        public UserFactory(IOptions<MySqlOptions> conf)
        {
            mysqlConfig = conf;
        }
        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(mysqlConfig.Value.ConnectionString);
            }
        }
        public void Add(User user)
        {
            using (IDbConnection dbConnection = Connection) {
                string query =  "INSERT INTO users (first, last, email, password, created_at, updated_at) VALUES (@first, @last, @email, @password, NOW(), NOW())";
                dbConnection.Open();
                dbConnection.Execute(query, user);
            }
        }
        public IEnumerable<User> Find(string email)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                
                return dbConnection.Query<User>("SELECT * FROM users WHERE email = '" + email + "'");
            }
        }
    }
}