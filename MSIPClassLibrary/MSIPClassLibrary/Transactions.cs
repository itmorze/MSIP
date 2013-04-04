using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;


namespace MSIPClassLibrary
{
    public delegate void StateNoInviteClientHandler(IStateClientNoInvite state);

    class Transactions
    {
    }


    class TransactionClientInvite
    {
        
    }


    class TransactionClinetNoInvite
    {
        private IStateClientNoInvite stateTransaction;
        private DispatcherTimer timerE;// = new DispatcherTimer();
        private DispatcherTimer timerF;
        private int countE, countF;
       
        public TransactionClinetNoInvite()
        {
            countE = 500;
            countF = 64;
        }

        public void Start(string request, string ipAdress)
        {

            stateTransaction = new StateTryingNoInvite(request, ipAdress);
            stateTransaction.Terminated += Terminated;
            timerE.Interval=new TimeSpan(0,0,0,0,countE);
            timerF.Interval = new TimeSpan(0, 0, 0, 64);

        }

        private void Terminated(IStateClientNoInvite _state)
        {
            //проинформировать TU, чтобы тот сбросил транзакцию
        }
        
    }

    public interface IStateClientNoInvite
    {
        event StateNoInviteClientHandler NextState;
        event StateNoInviteClientHandler Terminated;
    //    event StateNoInviteClientHandler ChangeToTerminated();
        void ReceivedE();
        void ReceivedF();

    }

    public class StateIdleNoInvite:IStateClientNoInvite
    {
        public event StateNoInviteClientHandler NextState;
        public event StateNoInviteClientHandler Terminated;

        public void ReceivedE()
        {
            // хз, нужен ли это состояние вообще

        }
        public void ReceivedF()
        {
            // хз, нужен ли это состояние вообще
        }
    
    }

    class StateTryingNoInvite:IStateClientNoInvite
    {
        public event StateNoInviteClientHandler NextState;
        public event StateNoInviteClientHandler Terminated;

        public StateTryingNoInvite(string request, string ipAdress)
        {
            
        }
        
        public void ReceivedE()
        {
            //отослать запрос повторно

        }
        public void ReceivedF()
        {
            
            //перейти в состояние Terminated
            Terminated(new StateTerminatedNoInvite());
        }

       
    
    }

    class StateProceedingNoInvite:IStateClientNoInvite
    {
        public event StateNoInviteClientHandler NextState;
        public event StateNoInviteClientHandler Terminated;

        public void ReceivedE()
        {
            //отослать запрос повторно

        }
        public void ReceivedF()
        {
            //проинформировать TU
            Terminated(new StateTerminatedNoInvite());
        }
    
    }

    class StateCompleetedNoInvite:IStateClientNoInvite
    {
        public event StateNoInviteClientHandler NextState;
        public event StateNoInviteClientHandler Terminated;

        public void ReceivedF()
        {
            //ничего не предпринимать

        }
        public void ReceivedE()
        {
            //ничего не предпринимать

        }
    }

    public class StateTerminatedNoInvite:IStateClientNoInvite
    {
        public event StateNoInviteClientHandler NextState;
        public event StateNoInviteClientHandler Terminated;

        public void ReceivedF()
        {
            //ничего не предпринимать

        }
        public void ReceivedE()
        {
            //ничего не предпринимать

        }
    }
}
