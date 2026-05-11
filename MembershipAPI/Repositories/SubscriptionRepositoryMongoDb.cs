using MembershipAPI.Models;
using MembershipAPI.Models.DTOs;
using MembershipAPI.Repositories.Interfaces;
using MongoDB.Driver;

namespace MembershipAPI.Repositories;

public class SubscriptionRepositoryMongoDb : ISubscriptionRepository
{
    public Task<bool> CreateNewSubscriptionAsync(SubscriptionDto subscription)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UpdateSubscriptionAsync(UpdateSubscriptionDto subscription)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteSubscriptionAsync(int subscriptionId)
    {
        throw new NotImplementedException();
    }

    public Task<Subscription?> GetSubscriptionByIdAsync(int subscriptionId)
    {
        throw new NotImplementedException();
    }

    public Task<List<Subscription>> GetAllSubscriptionsAsync()
    {
        throw new NotImplementedException();
    }
    
    private async Task<int> GetMaxId()
    {
        _logger.LogDebug("GetMaxId called from SubscriptionRepository");
        
        try
        {
            var filter = Builders<MemberSubscription>.Filter.Empty;
            var sort = Builders<MemberSubscription>.Sort.Descending("_id");
        
            var result = await _memberSubscriptionCollection.Find(filter).Sort(sort).Limit(1).FirstOrDefaultAsync();
            var maxId = result?.MemberSubscriptionId ?? 0;

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