using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bernsrite.Yahoo.Groups
{
    [TestClass]
    public class AuthenticationTests
    {
        static readonly string userName = "...";
        static readonly string password = "...";
        static readonly string groupName = "...";

        [TestMethod]
        public void Login()
        {
            var api = new Api();
            api.Login(userName, password);
            var numMessages = 10;
            var messages = api.GetMessages(groupName, numMessages);
            Assert.AreEqual(numMessages, messages.Length);
        }

        static Exception GetInnermostException(Exception ex)
        {
            return ex.InnerException == null
                ? ex
                : GetInnermostException(ex.InnerException);
        }

        [TestMethod]
        public void BadUserName()
        {
            var api = new Api();
            try
            {
                api.Login("incorrect", password);
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid user name", GetInnermostException(ex).Message);
            }
        }

        [TestMethod]
        public void BadPassword()
        {
            var api = new Api();
            try
            {
                api.Login(userName, "incorrect");
                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.AreEqual("Invalid password", GetInnermostException(ex).Message);
            }
        }
    }
}
