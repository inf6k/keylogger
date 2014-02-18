using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Timers;
using System.Runtime.InteropServices;

namespace KeyLogger
{
    public partial class Client : ServiceBase
    {
        private readonly Double _interval = (new TimeSpan(1, 0, 0, 0)).TotalMilliseconds;
        private Timer m_Timer;

        static void Main()
        {
            System.ServiceProcess.ServiceBase.Run(new Client());
        }

        public Client()
        {
            InitializeLifetimeService();
            Init();
            
            this.ServiceName = "Client";
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = true;
        }

        private void Init()
        {
            m_Timer = new Timer();
            m_Timer.BeginInit();
            m_Timer.AutoReset = false;
            m_Timer.Enabled = true;
            m_Timer.Interval = 1000.0;
            m_Timer.Elapsed += m_Timer_Elapsed;
            m_Timer.EndInit();

            System.Runtime.InteropServices
        }

        private void m_Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //TODO WORK WORK WORK
            RestartTimer();
        }

        private void RestartTimer()
        {
            m_Timer.Interval = _interval;
            m_Timer.Start();
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            Start();
        }

        protected override void OnStop()
        {
            Stop();
            base.OnStop();
        }

        public void Start()
        {
            m_Timer.Start();
        }

        public new void Stop()
        {
            System.IO.StreamWriter file = new System.IO.StreamWriter("C:/Users/vmadmin/Documents/derp.txt", true);
            file.WriteLine(m_Timer);
            file.Close();

            m_Timer.Stop();
        }
    }
}
