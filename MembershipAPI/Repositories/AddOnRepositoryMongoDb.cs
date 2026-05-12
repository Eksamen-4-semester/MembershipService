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
    
    public async Task<bool> CreateAddOnAsync(AddOnDto addOn)
    {
        var maxId = await GetMaxAddOnId() + 1;
        var add = new AddOn()
        {
            AddOnId = maxId,
            Name = addOn.Name,
            Price = addOn.Price
        };
        
        try
        {
            await _addOnCollection.InsertOneAsync(add);
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<bool> DeleteAddOnAsync(int addOnId)
    {
        var filter = Builders<AddOn>.Filter.Eq("_id", addOnId);
        var res = await _addOnCollection.DeleteOneAsync(filter);
        return res.DeletedCount > 0;
    }

    public async Task<AddOn?> GetAddOnByIdAsync(int addOnId)
    {
        var filter = Builders<AddOn>.Filter.Eq("_id", addOnId);
        return await _addOnCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<MemberAddOns?> GetSubscriptionAddOnsAsync(int memberSubscriptionId)
    {
        var filter = Builders<MemberAddOns>.Filter.Eq("MemberSubscriptionId", memberSubscriptionId);
        return await _memberAddOnCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<AddOn>?> GetAllAddOnsAsync()
    {
        var filter = Builders<AddOn>.Filter.Empty; 
        return await _addOnCollection.Find(filter).ToListAsync();
    }

    public async Task<bool> AssignAddOnsToMemberSubscriptionAsync(int memberSubscriptionId, List<int> addOnId)
    {
        var newMax = await GetMaxMemberAddOnId() + 1;
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

    public async Task<bool> AddOnsExistsAsync(List<int> addOnIds)
    {
        foreach (var id in addOnIds)
        {
            var filter = Builders<AddOn>.Filter.Eq("_id", id);
            var result = await _addOnCollection.Find(filter).FirstOrDefaultAsync();
            if (result == null)
                return false;
        }
        return true;
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