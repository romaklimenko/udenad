using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Udenad.Core
{
    public class Count
    {
        [BsonId]
        [BsonDateTimeOptions(DateOnly = true, Kind = DateTimeKind.Local)]
        public DateTime Date;
        public long All { get; set; }
        public long Bad { get; set; }
        public long Due { get; set; }
        public long Mature { get; set; }
        public long Unseen { get; set; }
    }
}