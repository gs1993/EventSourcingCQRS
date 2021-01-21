using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ReadModel.Persistence
{
    public class MongoDBRepository<T> : IRepository<T> where T : IReadEntity
    {
        private readonly IMongoDatabase _mongoDatabase;
        private string CollectionName => typeof(T).Name;

        public MongoDBRepository(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }


        public async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> predicate)
        {
            var cursor = await _mongoDatabase.GetCollection<T>(CollectionName)
                .FindAsync(predicate);

            return cursor.ToEnumerable();
        }

        public Task<T> Get(string id)
        {
            return _mongoDatabase.GetCollection<T>(CollectionName)
                .Find(x => x.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task Insert(T entity)
        {
            try
            {
                await _mongoDatabase.GetCollection<T>(CollectionName)
                    .InsertOneAsync(entity);
            }
            catch (MongoWriteException ex)
            {
                throw new RepositoryException($"Error inserting entity {entity.Id}", ex);
            }
        }

        public async Task Update(T entity)
        {
            try
            {
                var result = await _mongoDatabase.GetCollection<T>(CollectionName)
                    .ReplaceOneAsync(x => x.Id == entity.Id, entity);

                if (result.MatchedCount != 1)
                {
                    throw new RepositoryException($"Missing entoty {entity.Id}");
                }
            }
            catch (MongoWriteException ex)
            {
                throw new RepositoryException($"Error updating entity {entity.Id}", ex);
            }
        }
    }
}
