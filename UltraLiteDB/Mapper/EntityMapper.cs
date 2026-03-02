using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace UltraLiteDB
{
    /// <summary>
    /// Class to map entity class to BsonDocument
    /// </summary>
    public class EntityMapper
    {
        /// <summary>
        /// List all type members that will be mapped to/from BsonDocument
        /// </summary>
        public List<MemberMapper> Members { get; set; }

        /// <summary>
        /// Indicate which member is _id
        /// </summary>
        public MemberMapper Id { get { return this.Members.SingleOrDefault(x => x.FieldName == "_id"); } }

        /// <summary>
        /// Indicate which Type this entity mapper is
        /// </summary>
        public Type ForType { get; set; }

        /// <summary>
        /// Cached lookup from BSON field name to MemberMapper for O(1) access during deserialization
        /// </summary>
        private Dictionary<string, MemberMapper> _fieldLookup;

        public Dictionary<string, MemberMapper> FieldLookup
        {
            get
            {
                if (_fieldLookup == null)
                {
                    var lookup = new Dictionary<string, MemberMapper>(StringComparer.OrdinalIgnoreCase);
                    foreach (var m in Members)
                    {
                        if (m.FieldName != null)
                            lookup[m.FieldName] = m;
                    }
                    _fieldLookup = lookup;
                }
                return _fieldLookup;
            }
        }

        /// <summary>
        /// Resolve expression to get member mapped
        /// </summary>
        public MemberMapper GetMember(Expression expr)
        {
            return this.Members.FirstOrDefault(x => x.MemberName == expr.GetPath());
        }
    }
}