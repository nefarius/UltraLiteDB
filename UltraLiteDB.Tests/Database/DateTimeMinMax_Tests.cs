using System;
using System.IO;
using System.Linq;
using UltraLiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UltraLiteDB.Tests.Database
{
	#region Model

	public class DateTimeTest
	{
		public int Id { get; set; }
		public DateTime? Date { get; set; }
	}

	#endregion

	[TestClass]
	public class DateTimeMinMax_Tests
	{
		[TestMethod]
		public void DateTimeMinMax_Test()
		{
			var memory = new MemoryStream();

			using (var db = new UltraLiteDatabase(memory))
			{
				var col = db.GetCollection<DateTimeTest>();
				col.EnsureIndex("Date");

				col.Insert(new DateTimeTest() { Id = 1, Date = new DateTime(2018, 02, 22, 0, 0, 0) });
				col.Insert(new DateTimeTest() { Id = 2, Date = new DateTime(2018, 02, 22, 23, 59, 59) });

				MinMaxCommon(col);
			}

			using (var db = new UltraLiteDatabase(memory))
			{
				var col = db.GetCollection<DateTimeTest>();

				MinMaxCommon(col);

				col.Insert(new DateTimeTest() { Id = 3, Date = new DateTime(2018, 02, 21, 23, 59, 59) });
				col.Insert(new DateTimeTest() { Id = 4, Date = new DateTime(2018, 02, 23, 0, 0, 0) });
				col.Insert(new DateTimeTest() { Id = 5, Date = new DateTime(2018, 02, 22, 0, 0, 1) });
				col.Insert(new DateTimeTest() { Id = 6, Date = new DateTime(2018, 02, 22, 23, 59, 58) });

				MinMaxCommon(col);
			}

			using (var db = new UltraLiteDatabase(memory))
			{
				var col = db.GetCollection<DateTimeTest>();

				MinMaxCommon(col);
			}
		}

		private void MinMaxCommon(UltraLiteCollection<DateTimeTest> coll)
		{
			var searchdatetime = new DateTime(2018, 02, 22, 0, 0, 10);

			var min = coll.Min("Date").AsDateTime.ToUniversalTime();
			var max = coll.Max("Date").AsDateTime.ToUniversalTime();

			var smaller = coll.FindOne(Query.LT("Date", searchdatetime));
			var greater = coll.FindOne(Query.GT("Date", searchdatetime));

			var all = coll.FindAll().ToList();

			var linqmin = all.Min(x => x.Date);
			var linqmax = all.Max(x => x.Date);

			Assert.AreEqual(min, linqmin);
			Assert.AreEqual(max, linqmax);
		}
	}
}
