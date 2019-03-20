using Microsoft.VisualStudio.TestTools.UnitTesting;
using SecretSanta.Models;
using SecretSanta.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SecretSanta.Extensions;

namespace SecretSanta.Services.Tests
{
    [TestClass()]
    public class SantaServiceTests
    {
        #region Test Constants
        private const int LARGE = 5000;
        private const int MEDIUM = 500;
        private const int SMALL = 50;
        private const int TINY = 5;
        #endregion

        /// <summary>
        /// Tests that throws ArgumentException on inputs that cannot be processed
        /// </summary>
        [TestMethod()]        
        public void ValidateAndSetupTest_InvalidInputs()
        {
            var service = SantaService.CreateService() as SantaService;

            //Null
            Assert.ThrowsException<ArgumentException>( () => service.ValidateAndSetup(null));

            //Empty
            var groups = new List<GroupOfParticipants>();
            Assert.ThrowsException<ArgumentException>(() => service.ValidateAndSetup(groups.ToArray()));

            // To Few Groups, One Name
            groups.Add(new GroupOfParticipants() { Names = new String[] { "One" } });
            Assert.ThrowsException<ArgumentException>(() => service.ValidateAndSetup(groups.ToArray()));

            // Too Few Groups, Multiple Names
            groups[0].Names = new String[] { "One", "Two", "Three" };
            Assert.ThrowsException<ArgumentException>(() => service.ValidateAndSetup(groups.ToArray()));

            // Too Many Inputs Single Group
            var names = new List<String>();
            for (int i = 0; i < SantaService.MAX_INPUT_AMOUNT + 5; i++)
                names.Add(Guid.NewGuid().ToString());
            groups[0].Names = names.ToArray();
            Assert.ThrowsException<ArgumentException>(() => service.ValidateAndSetup(groups.ToArray()));
            groups[0].Names = new String[] { "One", "Two", "Three" };

            // Dominate group more than twice input group
            groups.Add(new GroupOfParticipants() { Names = new String[] { "Four" } });
            Assert.ThrowsException<ArgumentException>(() => service.ValidateAndSetup(groups.ToArray()));

            // Repeat Name Across Group
            groups[1].Names = new String[] { "One", "Five", "Six" };
            Assert.ThrowsException<ArgumentException>(() => service.ValidateAndSetup(groups.ToArray()));

            // Repeat Name In Group
            groups[1].Names = new String[] { "Four", "Five", "Four" };
            Assert.ThrowsException<ArgumentException>(() => service.ValidateAndSetup(groups.ToArray()));

            // Too many Inputs Across Groups
            for (int i = 0; i < SantaService.MAX_INPUT_AMOUNT; i++)
                groups.Add(new GroupOfParticipants(){ Names = new String[] { Guid.NewGuid().ToString() } });
            Assert.ThrowsException<ArgumentException>(() => service.ValidateAndSetup(groups.ToArray()));

        }


        /// <summary>
        /// Tests that names are trimmed and empties/nulls are removed.
        /// </summary>
        [TestMethod()]
        public void ValidateAndSetupTest_RemoveEmpitiesAndWhiteSpace()
        {
            var service = SantaService.CreateService() as SantaService;

            var groups = new List<GroupOfParticipants>()
            {
                new GroupOfParticipants() { Names = new String[] { "One  ", "", null, "  " } },
                new GroupOfParticipants() { Names = new String[] { "", null, "   " } },
                new GroupOfParticipants() { Names = new String[] { "", null, "   ", "  Two" } }
            };

            var results = service.ValidateAndSetup(groups.ToArray());

            Assert.IsTrue(results.Count == 2);
            var One = results.Find(r => r.Name.Equals("One"));
            var Two = results.Find(r => r.Name.Equals("Two"));
            Assert.IsNotNull(One);
            Assert.IsNotNull(Two);
        }


        [TestMethod()]
        public void GenerateListTest_Minimal()
        {
            List<Participant> p = new List<Participant>()
            {
                new Participant(1, "A"),
                new Participant(2, "B"),
            };
            var service = SantaService.CreateService() as SantaService;
            var result = service.GenerateList(p);
            VerifyOutputData(p, result);
            
        }


        #region GenerateListTest - No Groups
        [TestMethod()]
        public void GenerateListTest_NoGroupsTiny()
        {
            List<Participant> p = new List<Participant>();
            for (int i = 0; i < TINY; i++)
                p.Add(new Participant(i, Guid.NewGuid().ToString()));

            var service = SantaService.CreateService() as SantaService;
            var result = service.GenerateList(p);
            VerifyOutputData(p, result);
        }


        [TestMethod()]
        public void GenerateListTest_NoGroupsSmall()
        {
            List<Participant> p = new List<Participant>();
            for (int i = 0; i < SMALL; i++)
                p.Add(new Participant(i, Guid.NewGuid().ToString()));

            var service = SantaService.CreateService() as SantaService;
            var result = service.GenerateList(p);
            VerifyOutputData(p, result);
        }


        [TestMethod()]
        public void GenerateListTest_NoGroupsMedium()
        {
            List<Participant> p = new List<Participant>();
            for (int i = 0; i < MEDIUM; i++)
                p.Add(new Participant(i, Guid.NewGuid().ToString()));

            var service = SantaService.CreateService() as SantaService;
            var result = service.GenerateList(p);
            VerifyOutputData(p, result);
        }


        [TestMethod()]
        public void GenerateListTest_NoGroupsLarge()
        {
            List<Participant> p = new List<Participant>();
            for (int i = 0; i < LARGE; i++)
                p.Add(new Participant(i, Guid.NewGuid().ToString()));

            var service = SantaService.CreateService() as SantaService;
            var result = service.GenerateList(p);
            VerifyOutputData(p, result);
        }


        [TestMethod()]
        public void GenerateListTest_NoGroupsMAX()
        {
            List<Participant> p = new List<Participant>();
            for (int i = 0; i < SantaService.MAX_INPUT_AMOUNT; i++)
                p.Add(new Participant(i, Guid.NewGuid().ToString()));

            var service = SantaService.CreateService() as SantaService;
            var result = service.GenerateList(p);
            VerifyOutputData(p, result);
        }
        #endregion


        #region GenerateListTest - Groups
        [TestMethod()]
        public void GenerateListTest_LargeGroupsOf200()
        {
            List<Participant> p = new List<Participant>();
            for (int i = 0; i < LARGE; i++)
                p.Add(new Participant(i%(LARGE/200), Guid.NewGuid().ToString()));
            
            var service = SantaService.CreateService() as SantaService;
            var result = service.GenerateList(p);
            VerifyOutputData(p, result);
        }


        [TestMethod()]
        public void GenerateListTest_LargeGroupsOf5()
        {
            List<Participant> p = new List<Participant>();
            for (int i = 0; i < LARGE; i++)
                p.Add(new Participant (i % (LARGE / 5), Guid.NewGuid().ToString()));

            var service = SantaService.CreateService() as SantaService;
            var result = service.GenerateList(p);
            VerifyOutputData(p, result);
        }


        [TestMethod()]
        public void GenerateListTest_SmallGroupsOf4()
        {
            List<Participant> p = new List<Participant>();
            for (int i = 0; i < SMALL; i++)
                p.Add(new Participant (i % (SMALL / 4), Guid.NewGuid().ToString() ));

            var service = SantaService.CreateService() as SantaService;
            var result = service.GenerateList(p);
            VerifyOutputData(p, result);
        }


        [TestMethod()]
        public void GenerateListTest_UnevenGroups()
        {
            List<Participant> p = new List<Participant>();
            for (int i = 0; i < 200; i++)
                p.Add(new Participant (i%10, Guid.NewGuid().ToString()));
            for (int i = 200; i < 400; i++)
                p.Add(new Participant(200+ (i%35), Guid.NewGuid().ToString() ));
            for (int i = 400; i < 600; i++)
                p.Add(new Participant(400 + (i % 50), Guid.NewGuid().ToString() ));
            for (int i = 600; i < 800; i++)
                p.Add(new Participant(600+ (i % 100), Guid.NewGuid().ToString() ));
            for (int i = 800; i < 1000; i++)
                p.Add(new Participant(i, Guid.NewGuid().ToString() ));

            var service = SantaService.CreateService() as SantaService;
            var result = service.GenerateList(p);
            VerifyOutputData(p, result);
        }


        [TestMethod()]
        public void GenerateListTest_MAXGroupsOf4()
        {
            List<Participant> p = new List<Participant>();
            for (int i = 0; i < SantaService.MAX_INPUT_AMOUNT; i++)
                p.Add(new Participant ( i % (SantaService.MAX_INPUT_AMOUNT / 4), Guid.NewGuid().ToString() ));

            var service = SantaService.CreateService() as SantaService;
            var result = service.GenerateList(p);
            VerifyOutputData(p, result);
        }


        [TestMethod()]
        public void GenerateListTest_MediumGroupsOf4()
        {
            List<Participant> p = new List<Participant>();
            for (int i = 0; i < MEDIUM; i++)
                p.Add(new Participant ( i % (MEDIUM/4), Guid.NewGuid().ToString()));

            var service = SantaService.CreateService() as SantaService;
            var result = service.GenerateList(p);
            VerifyOutputData(p, result);
        }


        [TestMethod()]
        public void GenerateListTest_MediumGroupsOf3()
        {
            List<Participant> p = new List<Participant>();
            for (int i = 0; i < MEDIUM; i++)
                p.Add(new Participant(i % (MEDIUM / 3), Guid.NewGuid().ToString()));

            var service = SantaService.CreateService() as SantaService;
            var result = service.GenerateList(p);
            VerifyOutputData(p, result);
        }


        [TestMethod()]
        public void GenerateListTest_TinyGroupsOf2()
        {
            List<Participant> p = new List<Participant>();
            for (int i = 0; i < TINY; i++)
                p.Add(new Participant(i / 2, Guid.NewGuid().ToString()));

            var service = SantaService.CreateService() as SantaService;
            var result = service.GenerateList(p);
            VerifyOutputData(p, result);
        }
        #endregion


        #region Helpers
        /// <summary>
        /// Helper method to validate the final result in GenerateListTests
        /// </summary>
        private void VerifyOutputData(List<Participant> originalList, List<SantaResult> list)
        {
            // Giver and Recipient exist in original list and are not in the same group
            foreach (SantaResult pair in list)
            {
                var Giver = originalList.Find(p => String.Equals(p.Name, pair.Giver));
                var Recipient = originalList.Find(p => String.Equals(p.Name, pair.Recipient));
                Assert.IsNotNull(Giver);
                Assert.IsNotNull(Recipient);
                Assert.AreNotEqual(Giver.GroupID, Recipient.GroupID);
            }

            // The list counts are equal
            Assert.AreEqual(list.Count, originalList.Count);

            Assert.IsTrue(list.Select(pair => pair.Recipient).AreAllUnique().AreUnique);
            Assert.IsTrue(list.Select(pair => pair.Giver).AreAllUnique().AreUnique);
        }
        #endregion

    }
}