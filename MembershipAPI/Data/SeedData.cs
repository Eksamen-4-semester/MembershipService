using MembershipAPI.Models;
using MongoDB.Driver;

namespace MembershipAPI.Data;

public class SeedData
{
    public static async Task SeedDatabase(IMongoDatabase database)
    {
        var subscriptionCollection = 
            database.GetCollection<Subscription>("Subscription");
        
        var addOnCollection = database.GetCollection<AddOn>("AddOn");
        
        var subscriptionExists = await subscriptionCollection.Find(_ => true).AnyAsync();

        if (!subscriptionExists)
        {
            var subscriptions = new List<Subscription>()
            {
                new Subscription()
                {
                    Name = "Minimum Pakke", Price = 149, SubscriptionId = 1,
                    Description = "Minimum pakken inkluderer træning i det lokale FitLife fitness center"
                },
                new Subscription()
                {
                    Name = "Mellem Pakke", Price = 189, SubscriptionId = 2,
                    Description =
                        "Mellem pakke inkluderer træning i 2 valgfrie fitness centre samt giver adgang til holdtræning"
                },
                new Subscription()
                {
                    Name = "Maximum Pakke", Price = 249, SubscriptionId = 3,
                    Description =
                        "Maximum pakke giver adgang til alle fitness centre + holdtræning + adgang til svømmehal"
                },
                new Subscription()
                {
                    Name = "Studenter Pakke", Price = 149, SubscriptionId = 4,
                    Description = "Studenter pakken. Maximum pakke gevinster til Minimum pakke priser"
                }
            };
            await subscriptionCollection.InsertManyAsync(subscriptions);

            var addOnsExists = await addOnCollection.Find(_ => true).AnyAsync();

            if (!addOnsExists)
            {
                var addOns = new List<AddOn>()
                {
                    new AddOn()
                    {
                        AddOnId = 1, Name = "Data Pakke", Price = 9,
                        Description = "Mulighed for at tracke data gennem appen"
                    },
                };
                
                await addOnCollection.InsertManyAsync(addOns);
            }
        }
    }
}