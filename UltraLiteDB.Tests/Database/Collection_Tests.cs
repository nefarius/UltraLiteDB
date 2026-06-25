using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UltraLiteDB.Tests.Database
{
	[TestClass]
	public class Collection_Tests
	{
		[TestMethod]
		public void Reuse_Deleted_Collection()
		{
			using (var f = new TempFile())
			using (var db = new UltraLiteDatabase(f.Filename))
			{
				var col = db.GetCollection("col1");

				var id1 = col.Insert(new BsonDocument { { "a", 1 } });
				var id2 = col.Insert(new BsonDocument { { "a", 2 } });

				col.Delete(id2);

				Assert.IsNotNull(col.FindById(id1));
				Assert.IsNull(col.FindById(id2));

				db.DropCollection("col1");

				var newCol = db.GetCollection("col1");
				Assert.IsNull(col.FindById(id1));
				Assert.IsNull(newCol.FindById(id1));
			}
		}
	}
}
