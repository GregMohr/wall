using System.Collections.Generic;
using Dapper;
using System.Data;
using MySql.Data.MySqlClient;
using wall.Models;
using Microsoft.Extensions.Options;
using System.Linq;

namespace wall.Factory
{
    public class MessageFactory
    {
        private readonly IOptions<MySqlOptions> mysqlConfig;
        public MessageFactory(IOptions<MySqlOptions> conf)
        {
            mysqlConfig = conf;
        }
        internal IDbConnection Connection
        {
            get {
                return new MySqlConnection(mysqlConfig.Value.ConnectionString);
            }
        }
        public void Add(Message item)
        {
            using (IDbConnection dbConnection = Connection) {
                string query = "INSERT INTO messages (message, created_at, updated_at, userId) VALUES (@message, NOW(), NOW(), @userid)";
                dbConnection.Open();
                dbConnection.Execute(query, item);
            }
        }
        public IEnumerable<Message> FindAll()
        {
            using (IDbConnection dbConnection = Connection)
            {
                var messagesQuery = @"SELECT * FROM messages;";
                var commentsQuery = @"SELECT * FROM comments;";

                dbConnection.Open();

                IEnumerable<Message> messages = dbConnection.Query<Message>(messagesQuery);
                IEnumerable<Comment> comments = dbConnection.Query<Comment>(commentsQuery);
                List<Message> combo = messages.GroupJoin(comments, message => message.id, comment => comment.messageid,
                                        (message, matchedComments) =>
                                        {
                                            message.comments = matchedComments.ToList();
                                            return message;
                                        }).ToList();               
                return combo;
            }
        }

        public void Delete(int id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                dbConnection.Execute($"DELETE FROM messages WHERE id = {@id}");
            }
        }

        internal void AddComment(Comment comment)
        {
            using (IDbConnection dbConnection = Connection) {
                string query = "INSERT INTO comments (comment, created_at, updated_at, userid, messageid) VALUES (@comment, NOW(), NOW(), @userid, @messageid)";
                dbConnection.Open();
                dbConnection.Execute(query, comment);
            }
        }
    }
}