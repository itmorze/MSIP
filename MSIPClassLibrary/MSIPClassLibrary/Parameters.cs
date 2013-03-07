using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Connectivity;

namespace MSIPClassLibrary
{
    public delegate void DelCloseSession(string Name);
    public class Parameters
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

        public Parameters(string dName,string uID, string dom,string sport,string pass,string exp)
        {
            DisplayName = dName;
            UserId = uID;
            Domain = dom;
            ServerPort = sport;
            Password = pass;
            Expires = exp;
        }
    }
    
    
    public class Session
    {
        DelCloseSession DelClosesession;
        string _toIP;
        string _toUser;
        string _myName;
        string _myIp;
        int _cSeq = 0;       //
        int _port, _myaudioport, _toaudioport;
        bool _sessionConfirmed = false;
        string _sessionID;
        string _SDP;
        private Parameters _usParam;
        
        //Thread WaitForAnswer;       
       

        public Session(int myPort,  string toUser, /*DelCloseSession d1,*/ string ID, string SDPfunc, Parameters usParam)
        {
            _toIP = usParam.Domain;
            _toUser = toUser;
            _myName = usParam.UserId;
            _myIp = CurrentIPAddress();
            _port = myPort;
            _cSeq = 0;
            _myaudioport = 11010;
            _sessionID = ID;
           // DelClosesession = d1;
            _cSeq++;
            _usParam = usParam;

            if (SDPfunc.Length != 0)
            {
                _SDP = SDPcombine(SDPfunc);
            }
            else
            {
                _SDP = SDP();
            }
        }

        public string _ToUser
        {
            get
            {
                return _toUser;
            }
        }
        public int CSeq
        {
            set { _cSeq = value; }
            get { return _cSeq; }
        }
        public string MyName
        {
            get { return _myName; }
        }
        public string MyIP
        {
            get { return _myIp; }
        }
        public string ToUser
        {
            set { _toUser = value; }
            get { return _toUser; }
        }
        public string ToIP
        {
            set { _toIP = value; }
            get { return _toIP; }
        }
        public string SDPInfo
        {
            get { return _SDP; }
        }
        public string SessionID
        {
            get { return _sessionID; }
        }
        public string Domain
        {
            get { return _usParam.Domain; }
        }
        public string ServerPort
        {
            get { return _usParam.ServerPort; }
        }
        /// <summary>
        /// Функция закрытия сессии
        /// </summary>
        public void CloseSession()
        {
            DelClosesession(_myName);
        }
        /// <summary>
        /// Интерфейс подтверждённости сессии (сессия была принята или на неё как-либо иначе отреагировали)
        /// </summary>
        public bool SessionConfirmed
        {
            get
            {
                return _sessionConfirmed;
            }
        }
        /// <summary>
        /// Интерфейс получения ID сессии
        /// </summary>
        public string _SessionID
        {
            get
            {
                return this._sessionID;
            }
        }



        public string CurrentIPAddress()
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            if (icp != null && icp.NetworkAdapter != null)
            {
                var hostname =
                    NetworkInformation.GetHostNames()
                        .SingleOrDefault(
                            hn =>
                            hn.IPInformation != null && hn.IPInformation.NetworkAdapter != null
                            && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                            == icp.NetworkAdapter.NetworkAdapterId);

                if (hostname != null)
                {
                    // the ip address
                    return hostname.CanonicalName;
                }
            }

            return string.Empty;
        }

        string SDPcombine(string str)
        {
            string CodecInfo = "", tmp = "", tmp1 = "";
            CodecInfo += "Content-Type: application/sdp \r\n";
            tmp += "v=0 \r\n";
            tmp += "o=" + _cSeq.ToString() + "m" + "a" + _sessionID.ToString() + "IN IP4 " + _myIp + "\r\n";
            tmp += "c=IN IP4 " + _myIp + " \r\n";

            string[] ms = str.Split('\n');
            foreach (string str1 in ms)
            {
                if (str1.Contains("m=audio"))
                {
                    tmp += str1 + "\r\n";
                    tmp1 = str1.Remove(0, str1.IndexOf("audio ") + "audio ".Length);
                    tmp1 = tmp1.Remove(tmp1.IndexOf(" RTP"));
                    this._toaudioport = Convert.ToInt32(tmp1);
                }
                if (str1.Contains("PCMA/8000")) tmp += str1 + "r\n";
            }

            CodecInfo += "Content-Length: " + tmp.Length + "\r\n\n" + tmp;
            return CodecInfo;
        }

        public string SDP()
        {
            string CodecInfo = "", tmp = "";
            CodecInfo += "Content-Type: application/sdp \r\n";

            tmp += "v=0\r\n";
            tmp += "o=" + _myName + _cSeq.ToString() + "m" + "a" + _sessionID.ToString() + "IN IP4" + _myIp + "\r\n";
            tmp += "c=IN IP4 " + _myIp + "\r\n";
            tmp += "m=audio " + this._myaudioport.ToString() + " RTP/AVP 0\r\n";
            tmp += "a=rtpmap:0 PCMA/8000\r\n";

            CodecInfo += "Content-Length: " + tmp.Length + "\r\n\n" + tmp;

            return CodecInfo;
        }
    }


}
