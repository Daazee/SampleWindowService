using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TestService
{
    public partial class ForeignTransferService : ServiceBase
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly Timer timer = new Timer();

        public ForeignTransferService()
        {
            log4net.Config.XmlConfigurator.Configure();
            InitializeComponent();


            timer.Enabled = false;
            timer.AutoReset = false;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }

        public ForeignTransferService(IContainer container)
        {
            container.Add(this);

            InitializeComponent();


        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Transfer();
        }

        void Transfer()
        {
            try
            {

                log.Info($"Transfer at {DateTime.Now.ToString()}");
            }
            catch (Exception)
            {

                throw;
            }
            finally{
                timer.Start();
            }

        }

        protected override void OnStart(string[] args)
        {
            log.Info("Service started");
            int interval = 60000; //every minute

            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["POLLING_INTERVAL_SECONDS"]))
            {
                try
                {
                    interval = Convert.ToInt32(ConfigurationManager.AppSettings["POLLING_INTERVAL_SECONDS"]) * 1000;
                    timer.Interval = interval;
                }
                catch { }
            }

            log.Debug($"Checking for outbound transactions every {interval} ms");
            Transfer();
        }
        protected override void OnStop()
        {
            log.Info("stopped");
            timer.Stop();

        }

        protected override void OnContinue()
        {
            log.Info("continuing - checking for new transactions");

            Transfer();

        }

        protected override void OnPause()
        {
            log.Info("pausing");
            timer.Stop();
        }
    }
}
