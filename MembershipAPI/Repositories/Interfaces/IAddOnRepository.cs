using MembershipAPI.Models;
using MembershipAPI.Models.DTOs;

namespace MembershipAPI.Repositories.Interfaces;

public interface IAddOnRepository
{
    Task<bool> CreateAddOnAsync(AddOnDto addOn);
    Task<bool> DeleteAddOnAsync(int addOnId);
    Task<AddOn?> GetAddOnByIdAsync(int addOnId);
    Task<MemberAddOns?> GetSubscriptionAddOnsAsync(int memberSubscriptionId);
    
    Task<bool> AssignAddOnsToMemberSubscriptionAsync(int memberSubscriptionId, List<int> addOnId);
    Task<bool> RemoveAddOnsFromMemberSubscriptionAsync(int memberSubscriptionId, List<int> addOnId);
}