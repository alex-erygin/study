using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Owin.Hosting;
using NLog;
using Application = System.Windows.Application;

namespace Eon.Kiosk.Service
{
    public class App : Application
    {
        private const string AppTitle = "Sample App";
        private NotifyIcon icon;
        private static IDisposable webApp = null;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {         
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;
            App.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            RunNGinx();
	        RunWebApi();
            InitializeTray();
        }

	    private void RunWebApi()
	    {
			var currentIp = ConfigurationManager.AppSettings["WebServiceBaseUrl"];
			StartWebApi(currentIp);
		}

		private static void StartWebApi(string baseAddress)
		{
			logger.Debug("try start web-api on {0}", baseAddress);
			webApp = WebApp.Start<Startup>(baseAddress);
			logger.Debug("Service successfully started on address {0}", baseAddress);
		}

		protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                icon.Dispose();
                webApp.Dispose();
                base.OnExit(e);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void InitializeTray()
        {
            icon = new NotifyIcon
            {
                Icon = WebApiHost.Properties.Resources.favicon,
                Visible = true,
                ContextMenu = new ContextMenu(
                    new[]
                    {
                        new MenuItem("Open web page", OnOpenWebPagelick), 
                        new MenuItem("Exit", OnCloseClick),
                    }),
                Text = AppTitle
                    
            };

            icon.ShowBalloonTip(500, AppTitle, "Application is ready", ToolTipIcon.Info);
        }

        private static void RunNGinx()
        {
            KillNginxProcesses();

			var path = @"c:\Projects\study\nginx asp.net web api\nginx\nginx.exe";

			var startInfo = new ProcessStartInfo(path);
            startInfo.Verb = "runas";
            startInfo.WorkingDirectory = Path.GetDirectoryName(path);

            Process.Start(startInfo);
        }

        private static void KillNginxProcesses()
        {
            Process.GetProcessesByName("nginx").ToList().ForEach(x =>
            {
                try
                {
                    x.Kill();
                }
                catch
                {
                }
            });
        }


        private static void OnCloseClick(object sender, EventArgs eventArgs)
        {
            Current.Shutdown();
        }

        private static void OnOpenWebPagelick(object sender, EventArgs eventArgs)
        {
            Process.Start("http://localhost:22222");
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Logger logger = LogManager.GetCurrentClassLogger();
            if (!(e.ExceptionObject is Exception))
            {
                logger.Error((object)string.Format("Unhandled non-CLR exception occured ({0})", e.ExceptionObject));
            }
            else
            {
                logger.Error((object)string.Format("Domain unhandled exception of type {0} occured ({1})", e.GetType().Name, e.ExceptionObject));
            }
        }
    }
}