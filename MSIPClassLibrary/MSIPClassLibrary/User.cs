using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSIPClassLibrary
{
    public class User
    {
        private string _displayName;
        private string _userId;
        private string _domain;
        private string _serverport;
        private string _password;
        private string _expires;

        public string DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        public string Domain
        {
            get { return _domain; }
            set { _domain = value; }
        }

        public string ServerPort
        {
            get { return _serverport; }
            set { _serverport = value; }
        }

        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        public string Expires
        {
            get { return _expires; }
            set { _expires = value; }
        }

        public string UserId
        {
            get { return _userId; }
            set { _userId = value; }
        }

        public User(string dName,string uID, string dom,string sport,string pass,string exp)
        {
            DisplayName = dName;
            UserId = uID;
            Domain = dom;
            ServerPort = sport;
            Password = pass;
            Expires = exp;
        }
    }
}
