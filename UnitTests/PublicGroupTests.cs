using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bernsrite.Yahoo.Groups
{
    [TestClass]
    public class PublicGroupTests
    {
        private const string groupName = "concatenative";
        private static readonly GroupApi api = new GroupApi(new SessionApi(), groupName);

        [TestMethod]
        public void GetGroup()
        {
            var group = api.GetGroup();
            Assert.AreEqual(groupName, group.Name);
            Assert.AreEqual("Discuss the concatenative variety of computer languages: Joy, Forth, Postscript", group.Title);
            Assert.IsTrue(group.Description.StartsWith("The best introduction to this subject is at the Joy homepage."));
        }

        [TestMethod]
        public void GetMessages()
        {
            var numMessages = 10;
            var messages = api.GetMessages(numMessages);
            Assert.AreEqual(numMessages, messages.Length);

            var message = messages[0];
            Assert.AreEqual(5026, message.Id);
            Assert.AreEqual("Re: Where's formal proof of Haskell's claim to...?", message.Subject);
            Assert.AreEqual("chris glur", message.Author);
            Assert.AreEqual("", message.YahooAlias);
            Assert.IsTrue(message.Email.StartsWith("crglur@"));
            Assert.AreEqual("]> Where's formal proof of Haskell's claim to be able to avoid the need to ]> tell the machine what to do step-by-step, and just provide it with a ]>", message.Summary);
            Assert.AreEqual(new DateTime(2015, 3, 9), message.DateTime.Date);
        }
    }
}
