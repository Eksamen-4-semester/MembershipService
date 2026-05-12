using MembershipAPI.Models;
using MembershipAPI.Models.DTOs;
using MembershipAPI.Repositories.Interfaces;
using MongoDB.Driver;

namespace MembershipAPI.Repositories;

public class SubscriptionRepositoryMongoDb : ISubscriptionRepository
{
    IMongoCollection<Subscription> _subcriptionCollection;
    ILogger<SubscriptionRepositoryMongoDb> _logger;
    public SubscriptionRepositoryMongoDb(
        ILogger<SubscriptionRepositoryMongoDb> logger,
        IMongoDatabase database)
    {
        _logger = logger;
        _subcriptionCollection = database.GetCollection<Subscription>("Subscription");
    }
    
    public async Task<bool> CreateNewSubscriptionAsync(SubscriptionDto subscription)
    {
        try
        {
            var newMax = await GetMaxId() + 1;
            var newSub = new Subscription()
            {
                SubscriptionId = newMax,
                Name = subscription.Name,
                Price = subscription.Price,
            };
            await _subcriptionCollection.InsertOneAsync(newSub);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public Task<bool> UpdateSubscriptionAsync(UpdateSubscriptionDto subscription)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteSubscriptionAsync(int subscriptionId)
    {
        throw new NotImplementedException();
    }

    public async Task<Subscription?> GetSubscriptionByIdAsync(int subscriptionId)
    {
        var filter = Builders<Subscription>.Filter.Eq("_id", subscriptionId);
        return await _subcriptionCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<Subscription>?> GetAllSubscriptionsAsync()
    {
        var filter = Builders<Subscription>.Filter.Empty;
        return await _subcriptionCollection.Find(filter).ToListAsync();
    }
    
    private async Task<int> GetMaxId()
    {
        _logger.LogDebug("GetMaxId called from SubscriptionRepository");
        
        try
        {
            var filter = Builders<Subscription>.Filter.Empty;
            var sort = Builders<Subscription>.Sort.Descending("_id");
        
            var result = await _subcriptionCollection.Find(filter).Sort(sort).Limit(1).FirstOrDefaultAsync();
            var maxId = result?.SubscriptionId ?? 0;

            _logger.LogDebug(
                "GetMaxId returned {MaxId} from Subscription collection",
                maxId);

            return maxId;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "GetMaxId failed from Subscription collection");

            return 0;
        }
    }
}