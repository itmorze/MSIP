using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;


namespace MSIPClassLibrary
{

    public delegate void StateNoInviteClientHandler(IStateClientNoInvite state);

    public delegate void VoidEventHandler();

    class Transactions
    {
    }


    class TransactionClientInvite
    {
        
    }


    public class TransactionClinetNoInvite
    {
        private IStateClientNoInvite stateTransaction;
        private ThreadPoolTimer timerE;
   
        private ThreadPoolTimer timerF;
        private int countE, countF;
        private const int t1 = 500, t2=4; //t1, ms; t2, s 
       
        public TransactionClinetNoInvite()
        {
            countE = t1;
            countF = 64*t1;
            
        }

        public void Start(string request, string ipAdress)
        {
             //устанавливаем статус Trying и подписываемся на событие обновления таймера E
            stateTransaction = new StateTryingNoInvite(request, ipAdress);
            stateTransaction.RefreshCountE += RefreshTimerE;

            //подписываемся на событие перехода в состояние Terminated
            stateTransaction.Terminated += Terminated;


            //Инициализируются таймеры E и F
            timerE = ThreadPoolTimer.CreateTimer((source) => Windows.System.Threading.ThreadPool.RunAsync(
                (operation) => { if (stateTransaction != null) stateTransaction.ReceivedE(ref countE); }), TimeSpan.FromMilliseconds(countE));
            
            timerF = ThreadPoolTimer.CreateTimer((source) => Windows.System.Threading.ThreadPool.RunAsync(
                (operation) => { if (stateTransaction != null) stateTransaction.ReceivedF(ref countE); }), TimeSpan.FromMilliseconds(countF));

            
           



            

        }

        internal void RefreshTimerE()
        {
            timerE = ThreadPoolTimer.CreateTimer((source) => Windows.System.Threading.ThreadPool.RunAsync(
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
        public event StateNoInviteClientHandler NextState;
        public event StateNoInviteClientHandler Terminated;
        public event VoidEventHandler RefreshCountE;

        public StateTryingNoInvite(string request, string ipAdress)
        {
            
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
