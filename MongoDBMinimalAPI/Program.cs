using MongoDB.Bson;
using MongoDB.Driver;
using MongoDBMinimalAPI;

var builder = WebApplication.CreateBuilder(args);

// Get the MongoDB connection string from configuration
string connectionString = builder.Configuration.GetConnectionString("MongoDB");

var mongoClient = new MongoClient(connectionString);
var database = mongoClient.GetDatabase("MongoDBMinimalAPI");

builder.Services.AddSingleton<IMongoCollection<User>>(database.GetCollection<User>("Users"));
builder.Services.AddSingleton<IMongoCollection<Product>>(database.GetCollection<Product>("Products"));

var app = builder.Build();

// Get all users
app.MapGet("/users", async (IMongoCollection<User> usersCollection) =>
{
    var users = await usersCollection.Find(_ => true).ToListAsync();
    return Results.Ok(users);
});

// Get a user by ID
app.MapGet("/users/{id}", async (string id, IMongoCollection<User> usersCollection) =>
{
    var user = await usersCollection.Find(u => u.Id == id).FirstOrDefaultAsync();
    return user is not null ? Results.Ok(user) : Results.NotFound();
});

app.MapPost("/users/dummy-insert", async (IMongoCollection<User> usersCollection) =>
{
    var dummyUser = new User
    {
        Name = "Dummy User",
        Email = "dummy.user@example.com",
        Age = 25
    };

    await usersCollection.InsertOneAsync(dummyUser);
    return Results.Ok(new { Message = "Dummy user inserted successfully!", User = dummyUser });
});

// Create a new user
app.MapPost("/users", async (User newUser, IMongoCollection<User> usersCollection) =>
{
    await usersCollection.InsertOneAsync(newUser);
    return Results.Created($"/users/{newUser.Id}", newUser);
});

// Update a user
app.MapPut("/users/{id}", async (string id, User updatedUser, IMongoCollection<User> usersCollection) =>
{
    var result = await usersCollection.ReplaceOneAsync(u => u.Id == id, updatedUser);
    return result.MatchedCount > 0 ? Results.Ok(updatedUser) : Results.NotFound();
});

// Delete a user
app.MapDelete("/users/{id}", async (string id, IMongoCollection<User> usersCollection) =>
{
    var result = await usersCollection.DeleteOneAsync(u => u.Id == id);
    return result.DeletedCount > 0 ? Results.Ok() : Results.NotFound();
});

// Get all products
app.MapGet("/products", async (IMongoCollection<Product> productsCollection) =>
{
    var products = await productsCollection.Find(_ => true).ToListAsync();
    return Results.Ok(products);
});

// Get a product by ID
app.MapGet("/products/{id}", async (string id, IMongoCollection<Product> productsCollection) =>
{
    var product = await productsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
    return product is not null ? Results.Ok(product) : Results.NotFound();
});

// Create a new product
app.MapPost("/products", async (Product newProduct, IMongoCollection<Product> productsCollection) =>
{
    await productsCollection.InsertOneAsync(newProduct);
    return Results.Created($"/products/{newProduct.Id}", newProduct);
});

// Update a product
app.MapPut("/products/{id}", async (string id, Product updatedProduct, IMongoCollection<Product> productsCollection) =>
{
    var result = await productsCollection.ReplaceOneAsync(p => p.Id == id, updatedProduct);
    return result.MatchedCount > 0 ? Results.Ok(updatedProduct) : Results.NotFound();
});

// Delete a product
app.MapDelete("/products/{id}", async (string id, IMongoCollection<Product> productsCollection) =>
{
    var result = await productsCollection.DeleteOneAsync(p => p.Id == id);
    return result.DeletedCount > 0 ? Results.Ok() : Results.NotFound();
});


app.Run();
