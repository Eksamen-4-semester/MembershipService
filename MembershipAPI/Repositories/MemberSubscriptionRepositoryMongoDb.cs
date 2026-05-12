using MembershipAPI.Models;
using MembershipAPI.Models.DTOs;
using MembershipAPI.Repositories.Interfaces;
using MongoDB.Driver;

namespace MembershipAPI.Repositories;

public class MemberSubscriptionRepositoryMongoDb : IMemberSubscriptionRepository
{
    ILogger<MemberSubscriptionRepositoryMongoDb> _logger;
    IMongoCollection<MemberSubscription> _memberSubscriptionCollection;
    IMongoCollection<MemberAddOns> _memberAddOnCollection;

    public MemberSubscriptionRepositoryMongoDb(
        ILogger<MemberSubscriptionRepositoryMongoDb> logger,
        IMongoDatabase database)
    {
        _logger = logger;
        _memberSubscriptionCollection = database.GetCollection<MemberSubscription>("MemberSubscription");
        _memberAddOnCollection = database.GetCollection<MemberAddOns>("MemberAddOn");
    }
    
    public async Task<MemberSubscription?> GetMemberSubscriptionByMemberIdAsync(int memberId)
    {
        var filter = Builders<MemberSubscription>.Filter.Eq("MemberId", memberId);
        return await _memberSubscriptionCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<MemberSubscription?> GetMemberSubscriptionByIdAsync(int memberSubscriptionId)
    {
        var filter = Builders<MemberSubscription>.Filter.Eq("_id", memberSubscriptionId);
        return await _memberSubscriptionCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> CreateMemberSubscriptionAsync(MemberSubscriptionDto memberSubscription)
    {
        var newMax = await GetMaxId() + 1;
        var subscription = new MemberSubscription()
        {
            MemberSubscriptionId = newMax,
            MemberId = memberSubscription.MemberId,
            SubscriptionId = memberSubscription.SubscriptionId,
            PaymentStatus = PaymentStatus.NotPayed,
        };
        try
        {
            await _memberSubscriptionCollection.InsertOneAsync(subscription);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "CreateMemberSubscription failed");
            Console.WriteLine(e);
            return false;
        }
    }

    public Task<bool> UpdateMemberSubscriptionAsync(MemberSubscriptionDto memberSubscription)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteMemberSubscriptionAsync(int memberSubscriptionId)
    {
        try
        {
            var memberSubscriptionFilter = Builders<MemberSubscription>.Filter.Eq("_id", memberSubscriptionId);
            var memberAddOnFilter = Builders<MemberAddOns>.Filter.Eq("_id", memberSubscriptionId);
            await _memberAddOnCollection.DeleteOneAsync(memberAddOnFilter);
            var delRes = await _memberSubscriptionCollection.DeleteOneAsync(memberSubscriptionFilter);
            if (delRes.IsAcknowledged && delRes.DeletedCount <= 0)
                return false;
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "DeleteMemberSubscription failed");
            Console.WriteLine(e);
            return false;
        }
    }

    public Task<bool> PaySubscriptionAsync(int memberSubscriptionId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CancelSubscriptionAsync(int memberSubscriptionId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> MemberAlreadyHasSubscriptionAsync(int memberId)
    {
        var filter = Builders<MemberSubscription>.Filter.Eq("MemberId", memberId);
        var result = await _memberSubscriptionCollection.Find(filter).FirstOrDefaultAsync();
        return result != null;
    }
    
    private async Task<int> GetMaxId()
    {
        _logger.LogDebug("GetMaxId called from MemberSubscriptionRepository");
        
        try
        {
            var filter = Builders<MemberSubscription>.Filter.Empty;
            var sort = Builders<MemberSubscription>.Sort.Descending("_id");
        
            var result = await _memberSubscriptionCollection.Find(filter).Sort(sort).Limit(1).FirstOrDefaultAsync();
            var maxId = result?.MemberSubscriptionId ?? 0;

            _logger.LogDebug(
                "GetMaxId returned {MaxId} from MemberSubscription collection",
                maxId);

            return maxId;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "GetMaxId failed from MemberSubscription collection");

            return 0;
        }
    }
}