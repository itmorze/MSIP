using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using System.Text;

namespace MSIPClassLibrary
{
    public class Message
    {
        private Session currentSession;
        public delegate void DelCloseSession(string Name);
        DelCloseSession DelClosesession;

        public Message(Session session)
        {
            currentSession = session;
        }





        public bool ParseHeader(string Info)
        {
            currentSession.CSeq++;

            if (Info.Contains("BYE"))
            {
                ParseBye(Info);
                return true;
            }

            if (Info.Contains("ACK"))
            {
                Create_2XX("00", true, false);
                return true;
            }

            if (Info.Contains("CANCEL"))
            {

                return true;
            }

            if (Info.Contains("REGISTER"))
            {

                return true;
            }

            if (Info.Contains("OPTIONS"))
            {
                Create_2XX("00", true, false);
                return true;
            }

            if (Info.Contains("SIP/2.0 1"))
            {
                return true;
            }

            if (Info.Contains("SIP/2.0 2"))
            {
                Parse_2XX(Info);
                return true;
            }

            if (Info.Contains("SIP/2.0 3"))
            {
                Parse_3XX(Info);
                return true;
            }

            if (Info.Contains("SIP/2.0 4"))
            {
                this.Parse_4XX(Info);
                return true;
            }

            if (Info.Contains("SIP/2.0 5"))
            {
                this.Parse_5XX(Info);
                return true;
            }

            if (Info.Contains("SIP/2.0 6"))
            {
                this.Parse_6XX(Info);
                return true;
            }

            return false;
        }

        private void Parse_3XX(string info)
        {
            throw new NotImplementedException();
        }

        private void Parse_2XX(string info)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Функция разбора запроса категории 3XX
        /// </summary>
        /// <param name="Info">Подаваемый запрос</param>
        public void Parse_5XX(string Info)
        {
            Create_2XX("00", false, false);
        }

        public void Parse_4XX(string Info)
        {
            throw new NotImplementedException();
        }

        public void Parse_6XX(string _XX, bool SDPRequired, bool EndSession)
        {
            string Request = "";
            Request += "SIP/2.0 6";
            switch (_XX)
            {
                case "00": Request += _XX + " Busy Everywhere\n"; break;
                case "03": Request += _XX + " Decline\n"; break;
                case "04": Request += _XX + " Does Not Exist Anywhere\n"; break;
                case "06": Request += _XX + " Not Acceptable\n"; break;
                default: Request += "03 Decline\n"; break;
            }

            Request += "From: " + currentSession.MyName + " <sip:" + currentSession.MyName + "@" + currentSession.MyIP + ">" + "\n";
            Request += "To: <sip: " + currentSession.ToUser + "@" + currentSession.ToIP + ">" + "\n";
            Request += currentSession.SessionID.ToString() + "\n";
            Request += "Cseq: " + (++currentSession.CSeq).ToString();// " Decline" + "\n";
            switch (_XX)
            {
                case "00": Request += _XX + " Busy Everywhere\n"; break;
                case "03": Request += _XX + " Decline\n"; break;
                case "04": Request += _XX + " Does Not Exist Anywhere\n"; break;
                case "06": Request += _XX + " Not Acceptable\n"; break;
                default: Request += "03 Decline\n"; break;
            }

            Request += "Date: " + DateTime.Now.ToString() + "\n";

            if (SDPRequired)
                Request += "\n" + currentSession.SDP();

            SendInfo(Request);

            if (EndSession)
                this.CloseSession();

        }

        /// <summary>
        /// Функция разбора запроса категории 6XX
        /// </summary>
        /// <param name="Info">Подаваемый запрос</param>
        public void Parse_6XX(string Info)
        {
            Create_2XX("00", false, true);
            CloseSession();
        }

        void ParseBye(string Info)
        {
            Create_2XX("00", false, true);
            CloseSession();
        }

        /// <summary>
        /// Функция создания ответа категории 2XX. XX - комбинация внутри категории
        /// </summary>
        /// <param name="_XX">Комбинация внутри категории</param>
        /// <param name="SDPRequired">Флаг необходимости прикрепления SDP информации</param>
        /// <param name="EndSession">Флаг необходимости закончить сессию после отправки данного запроса</param>
        public void Create_2XX(string _XX, bool SDPRequired, bool EndSession)
        {
            string Request = "";
            Request += "SIP/2.0 2" + _XX + " OK" + "\n";
            Request += "From: " + currentSession.MyName + " <sip:" + currentSession.MyName + "@" + currentSession.MyIP.ToString() + ">" + "\n";
            Request += "To: <sip: " + currentSession.ToUser + "@" + currentSession.ToIP + ">" + "\n";
            Request += currentSession.SessionID + "\n";
            Request += "Cseq: " + (++currentSession.CSeq).ToString() + " OK" + "\n";
            Request += "Date: " + DateTime.Now.ToString() + "\n";
            if (SDPRequired)
            {
                Request += currentSession.SDPInfo;
            }
            SendInfo(Request);

            if (EndSession)
                CloseSession();
        }

        public void Invite()
        {
            string Request = "INVITE sip: " + currentSession.ToUser + "@" + currentSession.ToIP + " SIP/2.0 \r\n";
            Request +="Via: SIP/2.0/UDP 192.168.0.101:50350;branch=z9hG4bK-d8754z-bc2fe4689063fc19-1---d8754z-;rport \r\n";
            Request+= "Record-Route: <sip:" + currentSession.ToUser + "@" + currentSession.MyIP + ";lr>" + " \r\n";
            Request += "From: " + "\"" + currentSession.MyName + "\"" + "<sip: " + currentSession.MyName + "@" + currentSession.MyIP.ToString() + "> " + " \r\n";
            Request += "To: " + "<sip: " + currentSession.ToUser + "@" + currentSession.ToIP + "> " + " \r\n";
            Request += "Call-ID: " + currentSession.SessionID + "@" + currentSession.MyIP + " \r\n";
            Request += "CSeq: " + (currentSession.CSeq).ToString() + " INVITE" + " \r\n";

            Request += "Date: " + DateTime.Now.ToString() + " \r\n";   //дата и время
            Request += "Allow: INVITE, ACK, CANCEL, BYE" + " \r\n";

            Request += currentSession.SDPInfo;

            SendInfo(Request);

          //  WaitForAnswer = new Thread(WaitForAnswerFunc);
           // WaitForAnswer.Start();

        }
        public void Register()
        {
            string Request = "REGISTER sip:test2.mangosip.ru:5060 SIP/2.0 \r\n";
            Request +=
                "Via: SIP/2.0/UDP 192.168.0.101:50580;branch=z9hG4bK-d8754z-0277bb13371c5a7d-1---d8754z-;rport \r\n";
            Request += "Max-Forwards: 70 \r\n";
            Request += "Contact: <sip:itmorze@192.168.0.101:50580;rinstance=a6e82112b3232acc> \r\n";
            Request += "To: \"itmorze\"<sip:itmorze@test2.mangosip.ru:5060> \r\n";
            Request += "From: \"itmorze\"<sip:itmorze@test2.mangosip.ru:5060>;tag=fe67df49 \r\n";
            Request += "Call-ID: ODlmNzUwOTFmMjdhYzdiMjhlYjhmMTI4ZGQ5NDQyNzY. \r\n";
            Request += "CSeq: 1 REGISTER \r\n";
            Request += "Expires: 120 \r\n";
            Request +=
                "Allow: INVITE, ACK, CANCEL, OPTIONS, BYE, REGISTER, SUBSCRIBE, NOTIFY, REFER, INFO, MESSAGE \r\n";
            Request += "Supportes: replaces \r\n";
            Request += "MSIPhone\r\n";
            Request += "Content-Length:0 \r\n\r\n";
            SendInfo(Request);


        }
        /// <summary>
        /// Функция закрытия сессии
        /// </summary>
        public void CloseSession()
        {
            DelClosesession(currentSession.MyName);
        }


        /// <summary>
        /// Функция отправки пакета данных, оформленного в виде строки
        /// </summary>
        /// <param name="Info">Отправляемая строка</param>
        /// <returns>Если true - отправка информации прошла успешно, иначе - false.</returns>
        bool SendInfo(string Info)
        {
           // string ipAddress = currentSession.CurrentIPAddress();

            try
            {
               
                NetworkInterface myPacket = new NetworkInterface();
                HostName host = new HostName("81.88.80.235");
                myPacket.Connect(host, "5060");
                myPacket.SendMessage(Info);
            }
            catch (Exception e)
            {
                return false;
            }
            
            //UdpClient udpClient = new UdpClient();

            //Byte[] sendBytes = Encoding.ASCII.GetBytes(Info);

            //if (System.Net.IPAddress.TryParse(ToIP, out ipAddress))
            //{
            //    System.Net.IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ipAddress, port);

            //    try
            //    {
            //        udpClient.Send(sendBytes, sendBytes.Length, ipEndPoint);
            //    }
            //    catch (Exception)
            //    {
            //        return false;
            //    }
            //}
            //else
            //{
            //    IPAddress[] ips;
            //    ips = Dns.GetHostAddresses(ToIP);
            //    foreach (IPAddress ip in ips)
            //    {
            //        System.Net.IPEndPoint ipEndPoint = new System.Net.IPEndPoint(ip, port);

            //        try
            //        {
            //            udpClient.Send(sendBytes, sendBytes.Length, ipEndPoint);
            //        }
            //        catch (Exception)
            //        {
            //            return false;
            //        }
            //    }
            //    if (ips.Length == 0) return false;
            //};

            return true;
        }
    }
}
