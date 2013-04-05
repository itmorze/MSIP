using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private DispatcherTimer timerE;// = new DispatcherTimer();
        private DispatcherTimer timerF;
        private int countE, countF;
        private const int t1 = 500, t2=4; //t1, ms; t2, s 
       
        public TransactionClinetNoInvite(DispatcherTimer e, DispatcherTimer f)
        {
            countE = t1;
            countF = 64*t1/1000;
            timerE = e;
            timerF = f;
        }

        public void Start(string request, string ipAdress)
        {
            timerE = new DispatcherTimer();
            timerF = new DispatcherTimer();
            //устанавливаем статус Trying
            stateTransaction = new StateTryingNoInvite(request, ipAdress);
            
            //подписываемся на событие перехода в состояние Terminated
            stateTransaction.Terminated += Terminated;

            // В случае истечения таймера в состоянии произойдет выполнение метода
            timerE.Tick += (object sender, object e) => { if (stateTransaction != null) stateTransaction.ReceivedE(ref countE); };
            stateTransaction.RefreshCountE += RefreshTimerE;

            timerF.Tick += (object sender, object e) => { if (stateTransaction != null) stateTransaction.ReceivedF(ref countF); };

            //задаем первоначальные параметры таймера и запускаем сам таймер
            timerE.Interval=new TimeSpan(0,0,0,0,countE);
            timerF.Interval = new TimeSpan(0, 0, 0, countF);
            timerE.Start();
            timerF.Start();

            

        }

        internal void RefreshTimerE()
        {
            //запускаем таймер с новым значением
            if(timerE.IsEnabled)
                timerE.Stop();

            timerE.Interval=new TimeSpan(0,0,0,0,countE);
            timerE.Start();

        }

        internal void RefreshTimerF()
        {
            //исходя из состояния обновляем таймер
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







    public class Timer
    {
        private bool isTimerRunning;
        public event EventHandler Tick;

        public async void StartTimer(int ms)
        {
            this.isTimerRunning = true;

            while (this.isTimerRunning)
            {
                await Task.Delay(ms);
            }
            Tick(this, new EventArgs());
        }

        public void StopTimer()
        {
            this.isTimerRunning = false;
        }

    }
}
