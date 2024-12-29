namespace MongoDBMinimalAPI
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } // Let MongoDB auto-generate this if null

        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int Age { get; set; }
    }
}
