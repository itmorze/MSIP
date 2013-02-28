using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSIPClassLibrary
{
    class Message
    {
        private Session currentSession;
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

        void ParseBye(string Info)
        {
            Create_2XX("00", false, true);
            this.CloseSession();
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
                this.CloseSession();
        }
    }
}
