using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bernsrite.Yahoo.Groups
{
    [TestClass]
    public class AuthenticationTests
    {
        /* Put credentials for your private group here */
        static readonly string userName = "...";
        static readonly string password = "...";
        static readonly string groupName = "...";

        [TestMethod]
        public void Login()
        {
            var api = new SessionApi();
            api.Login(userName, password);

            var groupApi = new GroupApi(api, groupName);
            var numSummaries = 10;
            var summaries = groupApi.GetMessageSummaries(numSummaries);
            Assert.AreEqual(numSummaries, summaries.Length);
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
            var api = new SessionApi();
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
            var api = new SessionApi();
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
