using MongoDB.Driver;
using MongoDB.Driver.Core.Connections;

namespace BannerlordSimulator
{
    public class Mongo
    {
        private static readonly string MongoConnectionString = "mongodb://root:root@localhost:27017";
        private static readonly string DatabaseName = "mnbsimulations";

        private static IMongoDatabase _database;

        public static IMongoDatabase Database
        {
            get
            {
                if (_database == null)
                {
                    _database = Connect();
                }

                return _database;
            }
        }

        private static IMongoDatabase Connect()
        {
            MongoClient dbClient = new MongoClient(MongoConnectionString);
            return dbClient.GetDatabase(DatabaseName);
        }
    }
}