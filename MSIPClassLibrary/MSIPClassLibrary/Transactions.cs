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

    public class Transactions
    {
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

        protected async Task SendInfo(string Info, string ipAdress, string ipPort)
        {
            // string ipAddress = currentSession.CurrentIPAddress();

            try
            {
             
                DatagramSocket udpSocket=new DatagramSocket();
                await udpSocket.BindServiceNameAsync("5060");

              //  NetworkInterface myPacket = new NetworkInterface();
                HostName host = new HostName(ipAdress);
                    await udpSocket.ConnectAsync(host,ipPort);


                DataWriter udpWriter=new DataWriter(udpSocket.OutputStream);
                udpWriter.WriteString(Info);
                await udpWriter.StoreAsync();
                //var tsk = myPacket.Connect(host, ipPort);
                //tsk.Wait();

                //if (myPacket.IsConnected)
                //{
                //    myPacket.SendMessage(Inf);
                //}
                udpSocket.Dispose();

            }
            catch (Exception e)
            {

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

        private string _branch;
        private ThreadPoolTimer timerF;
        private int countE, countF;
        private const int t1 = 500, t2=4; //t1, ms; t2, s 

        private string _request, _ipAdress, _port;
       
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

        public void Start(string request, string toIp,string serverPort)//string ipAdress,string port)
        {
            _request = request;
            _ipAdress = toIp;
            _port = serverPort;
             //устанавливаем статус Trying и подписываемся на событие обновления таймера E
            stateTransaction = new StateTryingNoInvite(request, toIp);
            stateTransaction.RefreshCountE += RefreshTimerE;

            //подписываемся на событие перехода в состояние Terminated
            stateTransaction.Terminated += Terminated;

            var tskSend=SendInfo(_request, _ipAdress, _port);
            tskSend.Wait();
            //Инициализируются таймеры E и F
            timerE = ThreadPoolTimer.CreateTimer( (source) =>  Windows.System.Threading.ThreadPool.RunAsync(
                (operation) => { if (stateTransaction != null) stateTransaction.ReceivedE(ref countE); }), TimeSpan.FromMilliseconds(countE));
            
            timerF = ThreadPoolTimer.CreateTimer( (source) =>  Windows.System.Threading.ThreadPool.RunAsync(
                (operation) => { if (stateTransaction != null) stateTransaction.ReceivedF(ref countE); }), TimeSpan.FromMilliseconds(countF));

            
        }

        internal void RefreshTimerE()
        {
      
            var tskSend=SendInfo(_request, _ipAdress, _port);
            tskSend.Wait();

            timerE = ThreadPoolTimer.CreateTimer((source) =>  Windows.System.Threading.ThreadPool.RunAsync(
                (operation) => { if (stateTransaction != null) stateTransaction.ReceivedE(ref countE); }), TimeSpan.FromMilliseconds(countE));
        
        }


        private void Terminated(IStateClientNoInvite _state)
        {
            //проинформировать TU, чтобы тот сбросил транзакцию
            stateTransaction = _state;


            //тест
            //Parameters myParam = new Parameters(stateTransaction.GetType().ToString(), "itmorze", "test2.mangosip.ru", "5060", "Itqq2808690", "3600");
            //Session ses = new Session(5060, "test", "123456Ggg", "anySDP", myParam);
            //Message mes = new Message(ses);
            //mes.Register();
            //тест

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

        public async void SendRequest()
        {
            
        }
       
    
    }

    class StateProceedingNoInvite:IStateClientNoInvite
    {
        public event StateNoInviteClientHandler NextState;
        public event StateNoInviteClientHandler Terminated;
        public event VoidEventHandler RefreshCountE;

        public void ReceivedE(ref int countE)
        {
            //отослать запрос повторно

        }
        public void ReceivedF(ref int countF)
        {
            //проинформировать TU
            Terminated(new StateTerminatedNoInvite());
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
    }



}
