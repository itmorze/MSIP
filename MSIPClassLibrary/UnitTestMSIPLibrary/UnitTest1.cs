﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MSIPClassLibrary;
using Windows.Networking;

namespace UnitTestMSIPLibrary
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
         //   Session mySession=new Session();
            //Assert.AreEqual("46.249.24.190",mySession.CurrentIPAddress());
            DelCloseSession del;
           // del= fortest("blabla");
           // Encoding.ASCII.GetString(receiveBytes);            
            Parameters myParam=new Parameters("itmorze","itmorze","test2.mangosip.ru","5060","Itqq2808690","3600");
            Session ses=new Session(5060,"test", "123456Ggg","anySDP", myParam);
            Message mes=new Message(ses);
            mes.Register();

            
           // Assert.AreEqual(true, myHost.IsConnected);
        }

       
    }
}
