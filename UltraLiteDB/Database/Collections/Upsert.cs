﻿using System;
using System.Collections.Generic;

namespace UltraLiteDB
{
    public partial class UltraLiteCollection<T>
    {
        /// <summary>
        /// Insert or Update a document in this collection.
        /// </summary>
        public bool Upsert(T document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));

            return _engine.Value.Upsert(_name, this.GetBsonDoc(document), _autoId) == 1;
        }

        /// <summary>
        /// Insert or Update all documents
        /// </summary>
        public int Upsert(IEnumerable<T> documents)
        {
            if (documents == null) throw new ArgumentNullException(nameof(documents));

            return _engine.Value.Upsert(_name, this.GetBsonDocs(documents), _autoId);
        }

        /// <summary>
        /// Insert or Update a document in this collection.
        /// </summary>
        public bool Upsert(BsonValue id, T document)
        {
            if (document == null) throw new ArgumentNullException(nameof(document));
            if (id == null || id.IsNull) throw new ArgumentNullException(nameof(id));

            // get BsonDocument from object
            var doc = _mapper.ToDocument(document);

            // set document _id using id parameter
            doc["_id"] = id;

            return _engine.Value.Upsert(_name, doc);
        }
    }
}