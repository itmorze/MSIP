using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;


namespace MSIPClassLibrary
{

    public delegate void StateNoInviteClientHandler(IStateClientNoInvite state);

    public delegate void VoidEventHandler();

    public delegate void MessageEventHandler(string mes);

    public class Transactions
    {
        protected string _branch;
        protected event MessageEventHandler receivedMessageEvent;
        protected string RandomBranch(int size)
        {
            Random random = new Random((int)DateTime.Now.Ticks);
            StringBuilder builder = new StringBuilder();
            builder.Append("z9hG4bK-");
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        protected async Task SendInfo(string info, string ipAdress, string ipPort, string myIpPort)
        {
            // string ipAddress = currentSession.CurrentIPAddress();

            try
            {
             
                DatagramSocket udpSocket=new DatagramSocket();
                udpSocket.MessageReceived += MessageReceived;
                await udpSocket.BindServiceNameAsync(myIpPort);

              //  NetworkInterface myPacket = new NetworkInterface();
                
                HostName host = new HostName(ipAdress);
                
                //await udpSocket.GetOutputStreamAsync(host, ipPort);
                
                IAsyncAction connectAction=udpSocket.ConnectAsync(host, ipPort);
                connectAction.AsTask().Wait();

                DataWriter udpWriter=new DataWriter(udpSocket.OutputStream);
                udpWriter.WriteString(info);
                await udpWriter.StoreAsync();

                udpSocket.Dispose();

            }
            catch (Exception e)
            {

            }
        }
        void MessageReceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs eventArgs)
        {
            try
            {
                 uint stringLenght = eventArgs.GetDataReader().UnconsumedBufferLength;
                string response=eventArgs.GetDataReader().ReadString(stringLenght);
                receivedMessageEvent(response);

            }
            catch (Exception exception)
            {

                throw;
            }
           

           
        }

    }


    class TransactionClientInvite
    {
        
    }


    public class TransactionClinetNoInvite:Transactions
    {
        private IStateClientNoInvite stateTransaction;
        private ThreadPoolTimer timerE;

       // private string _branch;
        private ThreadPoolTimer timerF;
        private int countE, countF;
        private const int t1 = 500, t2=4; //t1, ms; t2, s 

        private string _request, _ipAdress, _port;
        private string _myPort;

        public TransactionClinetNoInvite()
        {
            countE = t1;
            countF = 64*t1;
            _branch = RandomBranch(15);
        }

        public string Branch 
        {
            get { return _branch; }
        }

        public void Start(string request, string toIp,string serverPort,string myPort)//string ipAdress,string port)
        {
            _request = request;
            _ipAdress = toIp;
            _port = serverPort;
            _myPort = myPort;
             //устанавливаем статус Trying и подписываемся на событие обновления таймера E
            stateTransaction = new StateTryingNoInvite(request, toIp);
            stateTransaction.RefreshCountE += RefreshTimerE;

            //подписываемся на событие перехода в состояние Terminated
            stateTransaction.Terminated += Terminated;
             receivedMessageEvent += stateTransaction.ReceivedMessage;

            var tskSend=SendInfo(_request, _ipAdress, _port, _myPort);
            tskSend.Wait();

           
            //Инициализируются таймеры E и F
            timerE = ThreadPoolTimer.CreateTimer( (source) =>  Windows.System.Threading.ThreadPool.RunAsync(
                (operation) => { if (stateTransaction != null) stateTransaction.ReceivedE(ref countE); }), TimeSpan.FromMilliseconds(countE));
            
            timerF = ThreadPoolTimer.CreateTimer( (source) =>  Windows.System.Threading.ThreadPool.RunAsync(
                (operation) => { if (stateTransaction != null) stateTransaction.ReceivedF(ref countE); }), TimeSpan.FromMilliseconds(countF));

            
        }

        internal void RefreshTimerE()
        {
      
            var tskSend=SendInfo(_request, _ipAdress, _port,_myPort);
            tskSend.Wait();

            timerE = ThreadPoolTimer.CreateTimer((source) =>  Windows.System.Threading.ThreadPool.RunAsync(
                (operation) => { if (stateTransaction != null) stateTransaction.ReceivedE(ref countE); }), TimeSpan.FromMilliseconds(countE));
        
        }


        private void Terminated(IStateClientNoInvite _state)
        {
            //проинформировать TU, чтобы тот сбросил транзакцию
            stateTransaction = _state;


        }

       
    }

    public interface IStateClientNoInvite
    {
        event StateNoInviteClientHandler NextState;
        event StateNoInviteClientHandler Terminated;
        event VoidEventHandler RefreshCountE;
    //    event StateNoInviteClientHandler ChangeToTerminated();
        void ReceivedE(ref int countE);
        void ReceivedF(ref int countF);
        void ReceivedMessage(string mes);


    }

    public class StateIdleNoInvite:IStateClientNoInvite
    {
        public event StateNoInviteClientHandler NextState;
        public event StateNoInviteClientHandler Terminated;
        public event VoidEventHandler RefreshCountE;

        public void ReceivedE(ref int countE)
        {
            // хз, нужен ли это состояние вообще

        }
        public void ReceivedF(ref int countF)
        {
            // хз, нужен ли это состояние вообще
        }
        public void ReceivedMessage(string mes)
        {

        }
    
    }

    class StateTryingNoInvite:IStateClientNoInvite
    {
        const int t2=4000;
        private string _request;
        private string _ipAddress;
        public event StateNoInviteClientHandler NextState;
        public event StateNoInviteClientHandler Terminated;
        public event VoidEventHandler RefreshCountE;

        public StateTryingNoInvite(string request, string ipAddress)
        {
            _request = request;
            _ipAddress = ipAddress;
        }
        
        public void ReceivedE(ref int countE)
        {
            //отослать запрос повторно


            
            //refresh countE
            if (2*countE < t2)
                countE *= 2;
            else
            {
                countE = t2;
            }
            RefreshCountE();
        }

        public void ReceivedF(ref int countF)
        {
            
            //перейти в состояние Terminated
            Terminated(new StateTerminatedNoInvite());
        }

        public void ReceivedMessage(string mes)
        {
            string olol = mes;
        }
    
    }

    class StateProceedingNoInvite:IStateClientNoInvite
    {
        const int t2 = 4000;

        public event StateNoInviteClientHandler NextState;
        public event StateNoInviteClientHandler Terminated;
        public event VoidEventHandler RefreshCountE;

        public void ReceivedE(ref int countE)
        {
            countE = t2;
        }
        public void ReceivedF(ref int countF)
        {
            //проинформировать TU
            Terminated(new StateTerminatedNoInvite());
        }

        public void ReceivedMessage(string mes)
        {

        }
    }

    class StateCompleetedNoInvite:IStateClientNoInvite
    {
        public event StateNoInviteClientHandler NextState;
        public event StateNoInviteClientHandler Terminated;
        public event VoidEventHandler RefreshCountE;

        public void ReceivedF(ref int countF)
        {
            //ничего не предпринимать

        }
        public void ReceivedE(ref int countE)
        {
            //ничего не предпринимать

        }
        public void ReceivedMessage(string mes)
        {

        }
    }

    public class StateTerminatedNoInvite:IStateClientNoInvite
    {
        public event StateNoInviteClientHandler NextState;
        public event StateNoInviteClientHandler Terminated;
        public event VoidEventHandler RefreshCountE;

        public void ReceivedF(ref int countF)
        {
            //ничего не предпринимать

        }
        public void ReceivedE(ref int countE)
        {
            //ничего не предпринимать

        }
        public void ReceivedMessage(string mes)
        {

        }
    }



}
