using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDbBookstoreApi.Models
{
    public class MediaFile
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonIgnoreIfDefault] // This allows auto-generation when Id is null
        public string? Id { get; set; }
        public string FileName { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public byte[] Data { get; set; } = null!; // Binary data < 16MB
    }
}
