﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MSIPClassLibrary;
using Windows.Networking;
using Windows.UI.Xaml;

namespace UnitTestMSIPLibrary
{
    [TestClass]
    public class UnitTest1
    {
        //DispatcherTimer timerE=new DispatcherTimer();
        // DispatcherTimer   timerF = new DispatcherTimer();
       
        public void TestMethodSendInfo()
        {
         //   Session mySession=new Session();
            //Assert.AreEqual("46.249.24.190",mySession.CurrentIPAddress());
           // DelCloseSession del;
           //// del= fortest("blabla");
           //// Encoding.ASCII.GetString(receiveBytes);            
           // Parameters myParam=new Parameters("itmorze","itmorze","test2.mangosip.ru","5060","Itqq2808690","3600");
           // Session ses=new Session(5060,"test", "123456Ggg","anySDP", myParam);
           // Message mes=new Message(ses);
           // mes.Register();


        }

         [TestMethod]
        public void TestRegistrationMethod()
         {
             Parameters myParam = new Parameters("itmorze", "itmorze", "test2.mangosip.ru", "5060", "Itqq2808690", "3600");
             Session registerSes=new Session("itmorze","",myParam);
             Message registerMes=new Message(registerSes);
             TransactionClinetNoInvite newTrans=new TransactionClinetNoInvite();

            // newTrans.Start(registerMes.Register(newTrans.Branch),registerSes);

         }

       
    }
}
