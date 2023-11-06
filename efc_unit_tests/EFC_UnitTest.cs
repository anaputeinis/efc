using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace UnitTestProjectForEFC
{
    [TestClass]
    public class EFC_UnitTest
    {

        // TESTS FOR Check_ProfileList()
        [TestMethod]
        public void Test_CheckProfileList_1()
        {
            var Profile_1 = new AllergyProfile("name1", new List<List<string>>() { new List<string>() { "ing1", "0", "0" } }, 1);
            var Profile_2 = new AllergyProfile("name2", new List<List<string>>() { new List<string>() { "ing2", "0", "0" } }, 1);
            var List_Actual = new List<AllergyProfile>() { Profile_1, Profile_2 };

            var Profile_3 = new AllergyProfile("name1", new List<List<string>>() { new List<string>() { "ing3", "0", "0" } }, 1);
            EFC_Functions.Check_ProfileList(List_Actual, Profile_3);

            var List_Expected = new List<AllergyProfile>() { new AllergyProfile("name1", new List<List<string>>() { new List<string>() { "ing1", "0", "0" }, new List<string>() { "ing3", "0", "0" } }, 1), Profile_2 };


            var ex = JsonConvert.SerializeObject(List_Expected);
            var ac = JsonConvert.SerializeObject(List_Actual);
            Assert.AreEqual(ex, ac);
        }

        [TestMethod]
        public void Test_CheckProfileList_2()
        {
            var Profile_1 = new AllergyProfile("name1", new List<List<string>>() { new List<string>() { "ing1", "0", "0" } }, 1);
            var Profile_2 = new AllergyProfile("name2", new List<List<string>>() { new List<string>() { "ing2", "0", "0" } }, 1);
            var Profile_3 = new AllergyProfile("name3", new List<List<string>>() { new List<string>() { "ing3", "0", "0" } }, 1);

            var List_Actual = new List<AllergyProfile>() { Profile_1, Profile_2 };


            EFC_Functions.Check_ProfileList(List_Actual, Profile_3);

            var List_Expected = new List<AllergyProfile>() { Profile_1, Profile_2, Profile_3 };


            var ex = JsonConvert.SerializeObject(List_Expected);
            var ac = JsonConvert.SerializeObject(List_Actual);
            Assert.AreEqual(ex, ac);
        }

        [TestMethod]
        public void Test_CheckProfileList_3()
        {
            var Profile_1 = new AllergyProfile("name1", new List<List<string>>() { new List<string>() { "ing1", "0", "0" } }, 1);
            var Profile_2 = new AllergyProfile("name2", new List<List<string>>() { new List<string>() { "ing2", "0", "0" } }, 1);
            var Profile_3 = new AllergyProfile("name1", new List<List<string>>() { new List<string>() { "ing1", "0", "0" } }, 1);

            var List_Actual = new List<AllergyProfile>() { Profile_1, Profile_2 };


            EFC_Functions.Check_ProfileList(List_Actual, Profile_3);

            var List_Expected = new List<AllergyProfile>() { Profile_1, Profile_2};


            var ex = JsonConvert.SerializeObject(List_Expected);
            var ac = JsonConvert.SerializeObject(List_Actual);
            Assert.AreEqual(ex, ac);
        }

        // TESTS FOR SeeIfContains()

        [TestMethod]
        public void Test_SeeIfContains_1()
        {
            var Item_Actual = new Item("name1", new List<List<string>>() { new List<string>() { "ing1", "1", "0" }, new List<string>() { "ing2", "0", "0" } }, 1234);

            Item_Actual.SeeIfContains_Item();

            var Item_Expected = new Item("name1", new List<List<string>>() { new List<string>() { "ing1", "1", "0" }, new List<string>() { "ing2", "0", "0" } }, 1234);
            Item_Expected.Contains_G = 1;


            var ex = JsonConvert.SerializeObject(Item_Expected);
            var ac = JsonConvert.SerializeObject(Item_Actual);
            Assert.AreEqual(ex, ac);
        }


        [TestMethod]
        public void Test_SeeIfContains_2()
        {
            var Item_Actual = new Item("name1", new List<List<string>>() { new List<string>() { "ing1", "0", "0" }, new List<string>() { "ing2", "0", "1" } }, 1234);

            Item_Actual.SeeIfContains_Item();

            var Item_Expected = new Item("name1", new List<List<string>>() { new List<string>() { "ing1", "0", "0" }, new List<string>() { "ing2", "0", "1" } }, 1234);
            Item_Expected.Contains_L = 1;


            var ex = JsonConvert.SerializeObject(Item_Expected);
            var ac = JsonConvert.SerializeObject(Item_Actual);
            Assert.AreEqual(ex, ac);
        }

        //TESTS FOR Get_IngredientsString()

        [TestMethod]

        public void Test_Get_IngredientsString_1()
        {
            var List_Test = new List<List<string>>() { new List<string>() { "ing1", "0", "0" }, new List<string>() { "ing2", "0", "1" }, new List<string>() { "ing3", "0", "0" } };
            var List_Actual = Item.Get_IngredientsString(List_Test);

            var List_Expected = new List<string>() { "ing1", "ing2", "ing3" };

            var ex = JsonConvert.SerializeObject(List_Expected);
            var ac = JsonConvert.SerializeObject(List_Actual);
            Assert.AreEqual(ex, ac);

        }

        [TestMethod]

        public void Test_Get_IngredientsString_2()
        {
            var List_Test = new List<List<string>>();
            var List_Actual = Item.Get_IngredientsString(List_Test);

            var List_Expected = new List<string>();

            var ex = JsonConvert.SerializeObject(List_Expected);
            var ac = JsonConvert.SerializeObject(List_Actual);
            Assert.AreEqual(ex, ac);
        }


        //TESTS FOR Ingredients_Normalize()

        [TestMethod]

        public void Test_Ingredients_Normalize_1()
        {
            var String_Test = " ing1,     ing2,,   ing3";
            var List_Actual = Item.Ingredients_Normalize(String_Test);
            var List_Expected = new List<string>() { "ing1", "ing2", "ing3" };
            var ex = JsonConvert.SerializeObject(List_Expected);
            var ac = JsonConvert.SerializeObject(List_Actual);
            Assert.AreEqual(ex, ac);

        }

        [TestMethod]
        public void Test_Ingredients_Normalize_2()
        {
            var String_Test = "       ";
            var List_Actual = Item.Ingredients_Normalize(String_Test);
            var List_Expected = new List<string>();
            var ex = JsonConvert.SerializeObject(List_Expected);
            var ac = JsonConvert.SerializeObject(List_Actual);
            Assert.AreEqual(ex, ac);

        }
    }
}
