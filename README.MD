# MembershipAPI

A microservice for managing FitLife memberships, subscriptions, and add-ons. The service allows members to create subscriptions, view available subscription plans, attach add-ons to member subscriptions, and allows admins or trainers to manage subscription offerings.

## Endpoints

### Add-On

- `POST /api/AddOn/create` - Create a new add-on (Admin)
- `GET /api/AddOn` - Get all available add-ons
- `POST /api/AddOn/{memberSubscriptionId}` - Register one or more add-ons to a member subscription

### Member Subscription

- `GET /api/MemberSubscription/{memberId}` - Get the subscription for a specific member
- `POST /api/MemberSubscription/create` - Create a subscription for the authenticated member

### Subscription

- `GET /api/Subscription` - Get all available subscriptions
- `GET /api/Subscription/{subscriptionId}` - Get a specific subscription by ID
- `POST /api/Subscription/create` - Create a new subscription plan (Admin, Trainer)

## Technologies

- **C#**
- **ASP.NET Core**
- **JWT Authentication**
- **Role-Based Authorization**
- **Microsoft ILogger**
- **Repository Pattern**
- **VaultSharp**
