using MembershipAPI.Controllers;
using MembershipAPI.Models.DTOs;
using MembershipAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace MembershipAPI.UnitTest;

[TestClass]
public sealed class SubscriptionControllerTests
{
    Mock<ILogger<SubscriptionController>> _loggerMock;
    Mock<ISubscriptionRepository> _subscriptionRepositoryMock;
    SubscriptionController _subscriptionController;

    [TestInitialize]
    public void Initialize()
    {
        _subscriptionRepositoryMock = new Mock<ISubscriptionRepository>();
        _loggerMock = new Mock<ILogger<SubscriptionController>>();
        _subscriptionController = new (_loggerMock.Object, _subscriptionRepositoryMock.Object);
    }
    
    [TestMethod]
    public async Task CreateSubscription_Returns_CreatedResult()
    {
        // Arrange
        _subscriptionRepositoryMock.Setup(r => r.CreateNewSubscriptionAsync(It.IsAny<SubscriptionDto>()))
            .ReturnsAsync(true);
        var subscriptionDto = new SubscriptionDto("Student-Subscription", 50.99M);
        
        // Act
        var result = await _subscriptionController.CreateSubscription(subscriptionDto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(CreatedResult));
    }
    
    [TestMethod]
    public async Task CreateSubscription_Returns_BadRequestObjectResult_When_Price_Equal_Zero()
    {
        // Arrange
        var subscriptionDto = new SubscriptionDto("Student-Subscription", 0);
        
        // Act
        var result = await _subscriptionController.CreateSubscription(subscriptionDto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }
    
    [TestMethod]
    public async Task CreateSubscription_Returns_BadRequestObjectResult_When_Name_Is_Null()
    {
        // Arrange
        var subscriptionDto = new SubscriptionDto(null, 250);
        
        // Act
        var result = await _subscriptionController.CreateSubscription(subscriptionDto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }
    
    [TestMethod]
    public async Task CreateSubscription_Returns_InternalServerErrorResult_When_Repository_Fails()
    {
        // Arrange
        _subscriptionRepositoryMock.Setup(r => r.CreateNewSubscriptionAsync(It.IsAny<SubscriptionDto>()))
            .ReturnsAsync(false);
        var subscriptionDto = new SubscriptionDto("Elder-Subscription", 23.95M);
        
        // Act
        var result = await _subscriptionController.CreateSubscription(subscriptionDto);

        // Assert
        Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
        Assert.AreEqual(500, ((StatusCodeResult)result).StatusCode);
    }
    
}
