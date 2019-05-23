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
            var numSummaries = 10;
            var summaries = api.GetMessageSummaries(numSummaries);
            Assert.AreEqual(numSummaries, summaries.Length);

            var summary = summaries[0];
            Assert.AreEqual(5026, summary.MessageId);
            Assert.AreEqual("Re: Where's formal proof of Haskell's claim to...?", summary.Subject);   // why does Yahoo eat the substring "[stack]"
            Assert.AreEqual("chris glur", summary.Author);
            Assert.AreEqual("", summary.YahooAlias);
            Assert.IsTrue(summary.Email.StartsWith("crglur@"));
            Assert.AreEqual("]> Where's formal proof of Haskell's claim to be able to avoid the need to ]> tell the machine what to do step-by-step, and just provide it with a ]>", summary.Summary);
            Assert.AreEqual(new DateTime(2015, 3, 9), summary.ReceivedTime.Date);

            var message = api.GetMessage(summary.MessageId);
            Assert.AreEqual(summary.MessageId, message.MessageId);
            Assert.AreEqual(summary.Author, message.Author);
            Assert.AreEqual("Re: [stack] Where's formal proof of Haskell's claim to...?", message.Subject);
            Assert.AreEqual(new DateTime(2015, 3, 1), message.SentTime.Date);   // why does Yahoo UI show 2/28 instead?
            Assert.IsTrue(
                message.Body
                    .Replace('\n', ' ')   // why doesn't Yahoo treat whitespace consistently?
                    .StartsWith(summary.Summary));
        }
    }
}
