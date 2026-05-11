using MongoDB.Bson.Serialization.Attributes;

namespace MembershipAPI.Models;

public class MemberSubscription
{
    [BsonId]
    public int MemberSubscriptionId { get; set; }
    public int SubscriptionId { get; set; }
    public int MemberId { get; set; }
    public decimal TotalSubscriptionPrice { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
}

public enum PaymentStatus
{
    Cancelled,
    Payed,
    NotPayed
}