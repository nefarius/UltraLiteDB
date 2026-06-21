using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace UltraLiteDB.Tests.Engine
{
    [TestClass]
    public class JsonMembers_Tests
    {
        private static List<JsonMember> Members(string json)
        {
            return new JsonReader(new StringReader(json)).DeserializeMembers().ToList();
        }

        [TestMethod]
        public void DeserializeMembers_YieldsMembersInSourceOrder()
        {
            var members = Members("{ \"a\": 1, \"b\": \"two\", \"c\": true }");

            CollectionAssert.AreEqual(
                new[] { "a", "b", "c" },
                members.Select(m => m.Key).ToArray());

            Assert.AreEqual(1, members[0].Value.AsInt32);
            Assert.AreEqual("two", members[1].Value.AsString);
            Assert.AreEqual(true, members[2].Value.AsBoolean);
        }

        [TestMethod]
        public void DeserializeMembers_DoesNotMergeDuplicateKeys()
        {
            var members = Members("{ \"x\": 1, \"x\": 2 }");

            Assert.AreEqual(2, members.Count);
            Assert.AreEqual(1, members[0].Value.AsInt32);
            Assert.AreEqual(2, members[1].Value.AsInt32);
        }

        [TestMethod]
        public void DeserializeMembers_TracksKeyLineAndColumn()
        {
            // line 1: {
            // line 2:   "a": 1,
            // line 3:   "b": 2
            // line 4: }
            var members = Members("{\n  \"a\": 1,\n  \"b\": 2\n}");

            Assert.AreEqual(2, members[0].Line);
            Assert.AreEqual(3, members[0].Column);
            Assert.AreEqual(3, members[1].Line);
            Assert.AreEqual(3, members[1].Column);
        }

        [TestMethod]
        public void DeserializeMembers_EmptyInputYieldsNothing()
        {
            Assert.AreEqual(0, Members("").Count);
        }

        [TestMethod]
        public void DeserializeMembers_StopsAtSyntaxErrorButYieldsPriorMembers()
        {
            var reader = new JsonReader(new StringReader("{ \"a\": 1, \"b\": }"));
            var collected = new List<JsonMember>();
            UltraLiteException? caught = null;

            try
            {
                foreach (var m in reader.DeserializeMembers())
                    collected.Add(m);
            }
            catch (UltraLiteException ex)
            {
                caught = ex;
            }

            Assert.AreEqual(1, collected.Count);
            Assert.AreEqual("a", collected[0].Key);
            Assert.IsNotNull(caught);
            Assert.AreEqual(1, caught.SourceLine);
        }
    }
}
