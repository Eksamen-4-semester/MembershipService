using MembershipAPI.Models;
using MembershipAPI.Models.DTOs;
using MembershipAPI.Repositories.Interfaces;
using MongoDB.Driver;

namespace MembershipAPI.Repositories;

public class AddOnRepositoryMongoDb : IAddOnRepository
{
    ILogger<AddOnRepositoryMongoDb> _logger;
    IMongoCollection<AddOn> _addOnCollection;
    IMongoCollection<MemberAddOns> _memberAddOnCollection;
    IMongoCollection<MemberSubscription> _memberSubscriptionCollection;

    public AddOnRepositoryMongoDb(
        ILogger<AddOnRepositoryMongoDb> logger,
        IMongoDatabase database)
    {
        _logger = logger;
        _memberAddOnCollection = database.GetCollection<MemberAddOns>("MemberAddOn");
        _addOnCollection = database.GetCollection<AddOn>("AddOn");
        _memberSubscriptionCollection = database.GetCollection<MemberSubscription>("MemberSubscription");
    }
    
    public Task<bool> CreateAddOnAsync(AddOnDto addOn)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteAddOnAsync(int addOnId)
    {
        throw new NotImplementedException();
    }

    public Task<AddOn?> GetAddOnByIdAsync(int addOnId)
    {
        throw new NotImplementedException();
    }

    public async Task<MemberAddOns?> GetSubscriptionAddOnsAsync(int memberSubscriptionId)
    {
        var filter = Builders<MemberAddOns>.Filter.Eq("MemberSubscriptionId", memberSubscriptionId);
        return await _memberAddOnCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> AssignAddOnsToMemberSubscriptionAsync(int memberSubscriptionId, List<int> addOnId)
    {
        var totalAddOnPrice = 0;
        foreach (var id in addOnId)
        {
            var exists = await AddOnExistsAsync(id);
            if (!exists)
                return false;
        }
        
        var newMax = await GetMaxMemberAddOnId();
        var newMemberAddOn = new MemberAddOns()
        {
            MemberAddOnId = newMax,
            MemberSubscriptionId = memberSubscriptionId,
            AddOnIds = addOnId
        };

        try
        {
            await _memberAddOnCollection.InsertOneAsync(newMemberAddOn);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> RemoveAddOnsFromMemberSubscriptionAsync(int memberSubscriptionId, List<int> addOnId)
    {
        foreach (var id in addOnId)
        {
            var exists = await AddOnExistsAsync(id);
            if (!exists)
                return false;
        }
        
        var currentMemberSubscriptionAddOns = await GetSubscriptionAddOnsAsync(memberSubscriptionId);
        if (currentMemberSubscriptionAddOns == null)
            return false;

        foreach (var id in addOnId)
        {
            currentMemberSubscriptionAddOns.AddOnIds.Remove(id);
        }

        try
        {
            var filter = Builders<MemberAddOns>.Filter.Eq("MemberSubscriptionId", memberSubscriptionId);
            await _memberAddOnCollection.FindOneAndReplaceAsync(filter, currentMemberSubscriptionAddOns);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private async Task<bool> AddOnExistsAsync(int addOnId)
    {
        var filter = Builders<AddOn>.Filter.Eq("_id", addOnId);
        var result = await _addOnCollection.Find(filter).FirstOrDefaultAsync();
        return result != null;
    }
    
    private async Task<int> GetMaxAddOnId()
    {
        _logger.LogDebug("GetMaxId called from AddOnRepository");
        
        try
        {
            var filter = Builders<AddOn>.Filter.Empty;
            var sort = Builders<AddOn>.Sort.Descending("_id");
        
            var result = await _addOnCollection.Find(filter).Sort(sort).Limit(1).FirstOrDefaultAsync();
            var maxId = result?.AddOnId ?? 0;

            _logger.LogDebug(
                "GetMaxId returned {MaxId} from AddOn collection",
                maxId);

            return maxId;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "GetMaxId failed from AddOn collection");

            return 0;
        }
    }
    
    private async Task<int> GetMaxMemberAddOnId()
    {
        _logger.LogDebug("GetMaxId called from AddOnRepository");
        
        try
        {
            var filter = Builders<MemberAddOns>.Filter.Empty;
            var sort = Builders<MemberAddOns>.Sort.Descending("_id");
        
            var result = await _memberAddOnCollection.Find(filter).Sort(sort).Limit(1).FirstOrDefaultAsync();
            var maxId = result?.MemberAddOnId ?? 0;

            _logger.LogDebug(
                "GetMaxId returned {MaxId} from MemberAddOn collection",
                maxId);

            return maxId;
        }
        catch (Exception e)
        {
            _logger.LogError(
                e,
                "GetMaxId failed from MemberAddOn collection");

            return 0;
        }
    }
}