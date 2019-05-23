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

        [TestMethod]
        public void GetMessages()
        {
            var api = new Api();
            var numMessages = 10;
            var messages = api.GetMessages(groupName, numMessages);
            Assert.AreEqual(numMessages, messages.Length);

            var message = messages[0];
            Assert.AreEqual(5026, message.Id);
            Assert.AreEqual("Re: Where's formal proof of Haskell's claim to...?", message.Subject);
            Assert.AreEqual("chris glur", message.Author);
            Assert.AreEqual("", message.YahooAlias);
            Assert.IsTrue(message.Email.StartsWith("crglur@"));
            Assert.AreEqual("]> Where's formal proof of Haskell's claim to be able to avoid the need to ]> tell the machine what to do step-by-step, and just provide it with a ]>", message.Summary);
        }
    }
}
