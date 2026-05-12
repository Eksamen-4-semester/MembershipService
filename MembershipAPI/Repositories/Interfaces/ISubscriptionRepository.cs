using MembershipAPI.Models;
using MembershipAPI.Models.DTOs;

namespace MembershipAPI.Repositories.Interfaces;

public interface ISubscriptionRepository
{
    Task<bool> CreateNewSubscriptionAsync(SubscriptionDto subscription);
    Task<bool> UpdateSubscriptionAsync(UpdateSubscriptionDto subscription);
    Task<bool> DeleteSubscriptionAsync(int subscriptionId);
    Task<Subscription?> GetSubscriptionByIdAsync(int subscriptionId);
    Task<List<Subscription>?> GetAllSubscriptionsAsync();
}