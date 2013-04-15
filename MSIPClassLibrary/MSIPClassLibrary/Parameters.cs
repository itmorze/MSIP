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
       //Объекты данного класса содержат настройки аккаунта
       

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
        //Объект данного класса описывает параметры сессии соединения с сервером
        //А также объекты описывают сессию каждого вызова
        string _toIP;
        string _toUser;
        string _myName;
        string _myIp;
        int _cSeq;
        private string _portRegister;
        private string _myaudioport;
        string _toaudioport;
        string _sessionID;
        private string _tag;
        string _SDP;
        internal Parameters _usParam;
        

        public Session(string toUser, string SDPfunc, Parameters usParam)
        {
            _myaudioport = "110110";
            _portRegister = usParam.ServerPort;

            _toIP = usParam.Domain;
            _toUser = toUser;
            _myName = usParam.UserId;
            _myIp = CurrentIPAddress();
            _cSeq = 0;
            _cSeq++;
            _usParam = usParam;
            _sessionID = RandomForCallid(8) + '-' + RandomForCallid(6) + '-' + RandomForCallid(3) + '-' +
                         RandomForCallid(5) + '@' + _myIp;
            _tag = RandomForCallid(9);

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
        public string Tag
        {
            get { return _tag; }
        }
        public string Domain
        {
            get { return _usParam.Domain; }
        }
        public string ServerPort
        {
            get { return _usParam.ServerPort; }
        }
        public string PortRegister
        {
            set { _portRegister = value; }
            get { return _portRegister.ToString(); }
        }
        public string PortAudio
        {
            set { _myaudioport = value; }
            get { return _myaudioport; }
        }
        public string SessionId
        {
            get
            {
                return this._sessionID;
            }
        }


        private string CurrentIPAddress()
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

        /// <summary>
        /// Перекомбинация SDP
        /// </summary>
        /// <param name="str">полученный от Абонента Б SDP</param>
        /// <returns>Перекомбинированный SDP</returns>
        private string SDPcombine(string str)
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
                    this._toaudioport = tmp1;
                }
                if (str1.Contains("PCMA/8000")) tmp += str1 + "r\n";
            }

            CodecInfo += "Content-Length: " + tmp.Length + "\r\n\n" + tmp;
            return CodecInfo;
        }


        internal string SDP()
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

        private static string RandomForCallid(int size)
        {
            var random = new Random((int)DateTime.Now.Ticks);
            var builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }


}
