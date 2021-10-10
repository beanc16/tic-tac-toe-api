using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using DotEnvHelpers;

namespace MongoDBHelpers
{
    class MongoConnection<T>
    {
        private static MongoClient _client { get; }
        private static IMongoDatabase _database { get; }

        static MongoConnection()
        {
            string uri = DotEnvHelper.GetEnvVariable("MONGO_URI");
            string dbName = DotEnvHelper.GetEnvVariable("DB_NAME");

            _client = new MongoClient(uri);
            _database = _client.GetDatabase(dbName);
        }



        /**
         * GETS
         */

        public static IMongoCollection<T> GetCollection(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        public static List<T> Find(string collectionName, FilterDefinition<T> filter)
        {
            IMongoCollection<T> collection = MongoConnection<T>.GetCollection(collectionName);
            return collection.Find(filter).ToList();
        }

        public static List<T> FindAll(string collectionName)
        {
            IMongoCollection<T> collection = MongoConnection<T>.GetCollection(collectionName);
            return collection.Find(_ => true).ToList();
        }



        /**
         * INSERTS
         */
        
        public static void InsertOne(string collectionName, T model)
        {
            IMongoCollection<T> collection = MongoConnection<T>.GetCollection(collectionName);
            collection.InsertOne(model);
        }
        
        public static void InsertMany(string collectionName, List<T> models)
        {
            IMongoCollection<T> collection = MongoConnection<T>.GetCollection(collectionName);
            collection.InsertMany(models);
        }
        
        public static void InsertMany(string collectionName, T[] models)
        {
            IMongoCollection<T> collection = MongoConnection<T>.GetCollection(collectionName);
            collection.InsertMany(models);
        }



        /**
         * UPDATES
         */
        
        public static void ReplaceOne(string collectionName, T model, FilterDefinition<T> filter)
        {
            IMongoCollection<T> collection = MongoConnection<T>.GetCollection(collectionName);
            collection.ReplaceOne(filter, model);
        }
    }
}