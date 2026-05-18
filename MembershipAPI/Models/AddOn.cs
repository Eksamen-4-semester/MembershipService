using MongoDB.Bson.Serialization.Attributes;

namespace MembershipAPI.Models;

public class AddOn
{
    [BsonId]
    public int AddOnId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; }
}