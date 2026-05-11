using MembershipAPI.Models;
using MembershipAPI.Models.DTOs;

namespace MembershipAPI.Repositories.Interfaces;

public interface IMemberSubscriptionRepository
{
    Task<MemberSubscription?> GetMemberSubscriptionByMemberIdAsync(string memberId);
    Task<MemberSubscription?> GetMemberSubscriptionByIdAsync(int memberSubscriptionId);
    Task<bool> CreateMemberSubscriptionAsync(MemberSubscriptionDto memberSubscription);
    Task<bool> UpdateMemberSubscriptionAsync(MemberSubscriptionDto memberSubscription);
    Task<bool> DeleteMemberSubscriptionAsync(int memberSubscriptionId);
    Task<bool> MemberAlreadyHasSubscriptionAsync(int memberId);
    
    
    Task<bool> PaySubscriptionAsync(int memberSubscriptionId);
    Task<bool> CancelSubscriptionAsync(int memberSubscriptionId);
}