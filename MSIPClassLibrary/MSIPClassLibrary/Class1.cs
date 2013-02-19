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
        private string _userID;
        private string _domain;
        private string _serverport;
        private string _password;
        private string _expires;

        public string displayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        public string domain
        {
            get { return _domain; }
            set { _domain = value; }
        }

        public string serverPort
        {
            get { return _serverport; }
            set { _serverport = value; }
        }

        public string password
        {
            get { return _password; }
            set { _password = value; }
        }

        public string expires
        {
            get { return _expires; }
            set { _expires = value; }
        }

        public string userID
        {
            get { return _userID; }
            set { _userID = value; }
        }
    }
}
