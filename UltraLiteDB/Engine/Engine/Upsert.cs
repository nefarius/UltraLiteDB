﻿using System;
using System.Collections.Generic;

namespace UltraLiteDB
{
    public partial class UltraLiteEngine
    {
        /// <summary>
        /// Implement upsert command to documents in a collection. Calls update on all documents,
        /// then any documents not updated are then attempted to insert.
        /// This will have the side effect of throwing if duplicate items are attempted to be inserted. Returns true if document is inserted
        /// </summary>
        public bool Upsert(string collection, BsonDocument doc, BsonAutoId autoId = BsonAutoId.ObjectId)
        {
            if (collection.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(collection));
            if (doc == null) throw new ArgumentNullException(nameof(doc));

            return this.Transaction<bool>(collection, true, (col) =>
            {
                var inserted = false;

                // first try update document (if exists _id)
                // if not found, insert
                if (doc["_id"] == BsonValue.Null || this.UpdateDocument(col, doc) == false)
                {
                    this.InsertDocument(col, doc, autoId);
                    inserted = true;
                }

                // returns if document was inserted
                return inserted;
            });
        }

        /// <summary>
        /// Implement upsert command to documents in a collection. Calls update on all documents,
        /// then any documents not updated are then attempted to insert.
        /// This will have the side effect of throwing if duplicate items are attempted to be inserted.
        /// </summary>
        public int Upsert(string collection, IEnumerable<BsonDocument> docs, BsonAutoId autoId = BsonAutoId.ObjectId)
        {
            if (collection.IsNullOrWhiteSpace()) throw new ArgumentNullException(nameof(collection));
            if (docs == null) throw new ArgumentNullException(nameof(docs));

            return this.Transaction<int>(collection, true, (col) =>
            {
                var count = 0;

                foreach (var doc in docs)
                {
                    // first try update document (if exists _id)
                    // if not found, insert
                    if (doc["_id"] == BsonValue.Null || this.UpdateDocument(col, doc) == false)
                    {
                        this.InsertDocument(col, doc, autoId);
                        count++;
                    }

                    _trans.CheckPoint();
                }

                // returns how many document was inserted
                return count;
            });
        }
    }
}