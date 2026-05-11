namespace MembershipAPI.Models;

public class MemberSubscription
{
    public int MemberSubscriptionId { get; set; }
    public int MemberId { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public int SubscriptionId { get; set; }
}

public enum PaymentStatus
{
    Cancelled,
    Payed,
    NotPayed
}