using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace UltraLiteDB.Tests.Database
{
	#region Model

	public class EntityMinMax
	{
		public int Id { get; set; }
		public byte ByteValue { get; set; }
		public int IntValue { get; set; }
		public uint UintValue { get; set; }
		public long LongValue { get; set; }
	}

	#endregion

	[TestClass]
	public class Query_Min_Max_Tests
	{
		[TestMethod]
		public void Query_Min_Max()
		{
			using (var f = new TempFile())
			using (var db = new UltraLiteDatabase(f.Filename))
			{
				var c = db.GetCollection<EntityMinMax>("col");

				c.Insert(new EntityMinMax { });
				c.Insert(new EntityMinMax
				{
					ByteValue = 200,
					IntValue = 443500,
					LongValue = 443500,
					UintValue = 443500
				});

				c.EnsureIndex("ByteValue");
				c.EnsureIndex("IntValue");
				c.EnsureIndex("LongValue");
				c.EnsureIndex("UintValue");

				Assert.AreEqual(200, c.Max("ByteValue").AsInt32);
				Assert.AreEqual(443500, c.Max("IntValue").AsInt32);
				Assert.AreEqual(443500, c.Max("LongValue").AsInt64);
				Assert.AreEqual(443500, c.Max("UintValue").AsInt32);

			}
		}
	}
}
