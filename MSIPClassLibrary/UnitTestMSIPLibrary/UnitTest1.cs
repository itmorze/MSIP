using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MSIPClassLibrary;

namespace UnitTestMSIPLibrary
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Session mySession=new Session();
            Assert.AreEqual("46.249.24.190",mySession.CurrentIPAddress());
        }
    }
}
