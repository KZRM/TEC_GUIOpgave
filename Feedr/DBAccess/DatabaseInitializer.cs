using Feedr.Data;
using Microsoft.EntityFrameworkCore;

namespace Feedr.DBAccess;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(FeedrDBContext db)
    {
        // Projektet bruger en scaffoldet model uden migrations.
        await db.Database.EnsureCreatedAsync();

        // Seed kun en tom database, så egne data og ændringer ikke overskrives.
        if (await db.Addresses.AnyAsync()
            || await db.AppUsers.AnyAsync()
            || await db.OrderStatusTypes.AnyAsync()
            || await db.Restaurants.AnyAsync()
            || await db.Products.AnyAsync()
            || await db.RestaurantOrders.AnyAsync()
            || await db.OrderItems.AnyAsync()
            || await db.OrderItemModifications.AnyAsync()
            || await db.OrderStatusHistories.AnyAsync()
            || await db.Reviews.AnyAsync())
        {
            return;
        }

        // Alt gemmes samlet, så en fejl ikke efterlader delvise eksempeldata.
        await using var transaction = await db.Database.BeginTransactionAsync();

        // HasTrigger i modellen opretter ikke selve SQL-triggeren.
        await db.Database.ExecuteSqlRawAsync("""
            IF OBJECT_ID(N'dbo.TR_RestaurantOrder_StatusHistory', N'TR') IS NULL
            EXEC(N'CREATE TRIGGER dbo.TR_RestaurantOrder_StatusHistory
                ON dbo.RestaurantOrder
                AFTER UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;
                    INSERT INTO dbo.OrderStatusHistory (OrderID, CustomerID, OldStatusID, NewStatusID)
                    SELECT i.OrderID, i.OrderingCustomerID, d.OrderStatusID, i.OrderStatusID
                    FROM inserted AS i
                    INNER JOIN deleted AS d ON d.OrderID = i.OrderID
                    WHERE i.OrderStatusID <> d.OrderStatusID;
                END;');
            """);

        string[] statusNames = ["Pending", "Accepted", "Preparing", "Ready", "Delivering", "Delivered", "Cancelled"];
        db.OrderStatusTypes.AddRange(statusNames.Select((name, index) => new OrderStatusType
        {
            OrderStatusId = index + 1,
            StatusName = name
        }));

        var owner = new AppUser
        {
            Name = "Demo Restaurant Owner",
            Username = "demo.owner",
            Password = "DemoOnly123!",
            Email = "owner@example.com",
            IsAdmin = true,
            Address = new Address { Street = "Torvegade", HouseNumber = "1", PostalCode = "8000", City = "Aarhus" }
        };
        var customer = new AppUser
        {
            Name = "Anna Jensen",
            Username = "demo.anna",
            Password = "DemoOnly123!",
            Email = "anna@example.com",
            IsPremium = true,
            Address = new Address { Street = "Vestergade", HouseNumber = "12", PostalCode = "8000", City = "Aarhus" }
        };
        var otherCustomer = new AppUser
        {
            Name = "Mikkel Nielsen",
            Username = "demo.mikkel",
            Password = "DemoOnly123!",
            Email = "mikkel@example.com",
            Address = new Address { Street = "Skolegade", HouseNumber = "8", PostalCode = "8000", City = "Aarhus" }
        };

        var pizzaRestaurant = new Restaurant
        {
            Name = "Demo Pizzeria",
            Owner = owner,
            Address = new Address { Street = "Pizza Torv", HouseNumber = "2", PostalCode = "8000", City = "Aarhus" }
        };
        var burgerRestaurant = new Restaurant
        {
            Name = "Demo Burger",
            Owner = owner,
            Address = new Address { Street = "Burgergade", HouseNumber = "5", PostalCode = "8000", City = "Aarhus" }
        };

        var pizza = new Product { Name = "Margherita", Price = 79m, Restaurant = pizzaRestaurant };
        var pepperoni = new Product { Name = "Pepperoni", Price = 89m, Restaurant = pizzaRestaurant };
        var burger = new Product { Name = "Cheeseburger", Price = 85m, Restaurant = burgerRestaurant };
        var fries = new Product { Name = "Fries", Price = 30m, Restaurant = burgerRestaurant };
        db.Products.AddRange(pizza, pepperoni, burger, fries);

        var deliveredOrder = new RestaurantOrder
        {
            OrderingCustomer = customer,
            Restaurant = pizzaRestaurant,
            OrderStatusId = 6,
            DistanceToDelivery = 3,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
            OrderItems =
            [
                new OrderItem
                {
                    Product = pizza,
                    Quantity = 1,
                    BasePriceAtOrder = 79m,
                    FinalPriceAtOrder = 89m,
                    OrderItemModifications =
                    [
                        new OrderItemModification { Modification = "Extra cheese", PriceChange = 10m }
                    ]
                },
                new OrderItem { Product = pepperoni, Quantity = 1, BasePriceAtOrder = 89m, FinalPriceAtOrder = 89m }
            ]
        };
        var pendingOrder = new RestaurantOrder
        {
            OrderingCustomer = otherCustomer,
            Restaurant = burgerRestaurant,
            OrderStatusId = 1,
            DistanceToDelivery = 2,
            CreatedAtUtc = DateTime.UtcNow,
            OrderItems =
            [
                new OrderItem
                {
                    Product = burger,
                    Quantity = 2,
                    BasePriceAtOrder = 85m,
                    FinalPriceAtOrder = 85m,
                    OrderItemModifications =
                    [
                        new OrderItemModification { Modification = "No onions", PriceChange = 0m }
                    ]
                },
                new OrderItem { Product = fries, Quantity = 1, BasePriceAtOrder = 30m, FinalPriceAtOrder = 30m }
            ]
        };
        db.RestaurantOrders.AddRange(deliveredOrder, pendingOrder);
        db.Reviews.Add(new Review
        {
            User = customer,
            Restaurant = pizzaRestaurant,
            Rating = 5,
            ReviewDescription = "Great pizza and quick delivery!"
        });

        // EF indsætter relationerne i korrekt rækkefølge og tildeler ID'er.
        await db.SaveChangesAsync();
        db.OrderStatusHistories.Add(new OrderStatusHistory
        {
            OrderId = deliveredOrder.OrderId,
            CustomerId = customer.UserId,
            OldStatusId = 5,
            NewStatusId = 6,
            ChangedAtUtc = deliveredOrder.CreatedAtUtc!.Value.AddMinutes(30)
        });
        await db.SaveChangesAsync();
        await transaction.CommitAsync();
    }
}
