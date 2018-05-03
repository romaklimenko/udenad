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
        public long Due { get; set; }
        public long Learned { get; set; }
        public long Seen { get; set; }
    }
}