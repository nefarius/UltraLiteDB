using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace UltraLiteDB.Tests.Engine
{
	[TestClass]
	public class Performance_Tests
	{
		const int N1 = 10000;
		const int N2 = 1000;

		[TestMethod]
		public void Simple_Performance_Runner()
		{
			// just a simple example to test performance speed
			using (var file = new TempFile())
			using (var db = new UltraLiteEngine(file.Filename))
			{
				var ti = new Stopwatch();
				var tx = new Stopwatch();
				var tu = new Stopwatch();
				var td = new Stopwatch();

				ti.Start();
				db.Insert("col", GetDocs(N1));
				ti.Stop();

				tx.Start();
				db.EnsureIndex("col", "name");
				tx.Stop();

				tu.Start();
				db.Update("col", GetDocs(N1));
				tu.Stop();

				db.EnsureIndex("col", "name");

				td.Start();
				db.Delete("col", Query.All());
				td.Stop();

				Debug.WriteLine("Insert time: " + ti.ElapsedMilliseconds);
				Debug.WriteLine("EnsureIndex time: " + tx.ElapsedMilliseconds);
				Debug.WriteLine("Update time: " + tu.ElapsedMilliseconds);
				Debug.WriteLine("Delete time: " + td.ElapsedMilliseconds);
			}
		}

		[TestMethod]
		public void RepeatedUpdateSuite()
		{
			// Test higher volume operation performance
			using (var file = new TempFile())
			using (var db = new UltraLiteDatabase(file.Filename))
			{
				UltraLiteCollection<BsonDocument> collection = db.GetCollection<BsonDocument>("col");
				const int runs = 1000;

				BsonDocument bigDoc = new BsonDocument();
				bigDoc["_id"] = 1000;
				bigDoc["name"] = "Doccy";
				bigDoc["thing"] = 1.0f;
				bigDoc["data"] = new byte[3000];
				bigDoc["list"] = new BsonArray(new int[3000]);

				BsonDocument smallDoc = new BsonDocument();
				smallDoc["_id"] = 2000;
				smallDoc["name"] = "Doccy";

				BsonDocument cDoc1 = new BsonDocument();
				cDoc1["_id"] = 3000;
				cDoc1["name"] = "Doccy";

				BsonDocument cDoc2 = new BsonDocument();
				cDoc2["_id"] = 3001;
				cDoc2["thing"] = 1.0f;

				BsonDocument cDoc3 = new BsonDocument();
				cDoc3["_id"] = 3002;
				cDoc3["data"] = new byte[3000];

				BsonDocument cDoc4 = new BsonDocument();
				cDoc4["_id"] = 3003;
				cDoc4["list"] = new BsonArray(new int[3000]);

				collection.Insert(bigDoc);
				collection.Insert(smallDoc);

				collection.Insert(cDoc1);
				collection.Insert(cDoc2);
				collection.Insert(cDoc3);
				collection.Insert(cDoc4);

				var bigUpdateSw = new Stopwatch();

				bigUpdateSw.Start();
				for (int i = 0; i < runs; i++)
				{
					collection.Update(bigDoc);
				}
				bigUpdateSw.Stop();

				var smallUpdateSw = new Stopwatch();

				smallUpdateSw.Start();
				for (int i = 0; i < runs; i++)
				{
					collection.Update(smallDoc);
				}
				smallUpdateSw.Stop();

				var compoundUpdateSw = new Stopwatch();

				compoundUpdateSw.Start();
				for (int i = 0; i < runs; i++)
				{
					collection.Update(cDoc1);
					collection.Update(cDoc2);
					collection.Update(cDoc3);
					collection.Update(cDoc4);
				}
				compoundUpdateSw.Stop();

				Debug.WriteLine("Bigdoc update time/1000: " + bigUpdateSw.ElapsedMilliseconds);
				Debug.WriteLine("Smalldoc update time/1000: " + smallUpdateSw.ElapsedMilliseconds);
				Debug.WriteLine("Compounddoc update time/1000: " + compoundUpdateSw.ElapsedMilliseconds);
			}
		}

		private IEnumerable<BsonDocument> GetDocs(int count)
		{
			var rnd = new Random();

			for (var i = 0; i < count; i++)
			{
				yield return new BsonDocument
				{
					{ "_id", i },
					{ "name", Guid.NewGuid().ToString() },
					{ "type", rnd.Next(1, 100) },
					{ "lorem", TempFile.LoremIpsum(3, 5, 2, 3, 3) }
				};
			}
		}
	}
}
