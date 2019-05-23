using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bernsrite.Yahoo.Groups
{
    [TestClass]
    public class PublicGroupTests
    {
        private static string groupName = "concatenative";

        [TestMethod]
        public void GetGroup()
        {
            var api = new Api();
            var group = api.GetGroup(groupName);
            Assert.AreEqual(groupName, group.Name);
            Assert.AreEqual("Discuss the concatenative variety of computer languages: Joy, Forth, Postscript", group.Title);
            Assert.IsTrue(group.Description.StartsWith("The best introduction to this subject is at the Joy homepage."));
        }
    }
}
