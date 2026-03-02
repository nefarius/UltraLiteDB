using System;

namespace UltraLiteDB
{
    public partial class BsonMapper
    {
        /// <summary>
        /// Deserialize BSON bytes directly to an entity, bypassing intermediate BsonDocument creation
        /// </summary>
        public virtual T DeserializeFromBytes<T>(byte[] bson, int offset = 0)
        {
            return (T)this.DeserializeFromBytes(typeof(T), bson, offset);
        }

        /// <summary>
        /// Deserialize BSON bytes directly to an entity, bypassing intermediate BsonDocument creation
        /// </summary>
        public virtual object DeserializeFromBytes(Type type, byte[] bson, int offset = 0)
        {
            if (bson == null) throw new ArgumentNullException(nameof(bson));

            // If target is BsonDocument, use existing path
            if (type == typeof(BsonDocument))
            {
                return BsonReader.Deserialize(bson, offset);
            }

            var reader = new ByteReader(bson);
            if (offset > 0) reader.Skip(offset);

            return DirectBsonReader.ReadObjectDirect(reader, this, type);
        }
    }
}
