using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSanta.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SecretSanta.Extensions.Tests
{
    [TestClass()]
    public class ExtensionsTests
    {
        [TestMethod()]
        public void TrimAndRemoveEmpitiesTest()
        {

            new String[0].TrimAndRemoveEmpities();

            var array = new String[] { " ", "One", "  Two  ", "Three  ", "  Four", null, "", "   ", "  Five", "Six", "Seven", "", null, " Ei  ght ", null, "    "};
            var newArray = array.TrimAndRemoveEmpities().ToArray();

            Assert.IsTrue(newArray.Length == 8);
            Assert.IsTrue(newArray.Contains("One"));
            Assert.IsTrue(newArray.Contains("Two"));
            Assert.IsTrue(newArray.Contains("Three"));
            Assert.IsTrue(newArray.Contains("Four"));
            Assert.IsTrue(newArray.Contains("Five"));
            Assert.IsTrue(newArray.Contains("Six"));
            Assert.IsTrue(newArray.Contains("Seven"));
            Assert.IsTrue(newArray.Contains("Ei  ght"));
        }


        [TestMethod()]
        public void GetRandomTest()
        {
            var list = new List<String>()
            {
                "One", "Two", "Three", "Four"
            };

            Assert.IsNull(new List<String>().GetRandom());
            Assert.IsTrue(new List<int>().GetRandom() == 0);
            for (int i = 0; i < 10; i++)
            {
                var value = list.GetRandom();
                Assert.IsTrue(list.Contains(value));
            }
        }

        [TestMethod()]
        public void AreAllUniqueTest()
        {
            var list = new List<String>()
            {
                "One", "Two", "Three", "Four"
            };

            var (AreUnique, Duplicant) = list.AreAllUnique();
            Assert.IsTrue(AreUnique);
            Assert.IsNull(Duplicant);

            list = new List<String>()
            {
                "One", "Two", "One", "Four"
            };
            (AreUnique, Duplicant) = list.AreAllUnique();
            Assert.IsFalse(AreUnique);
            Assert.AreEqual(Duplicant, "One");
        }

    }
}