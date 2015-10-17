using Microsoft.VisualBasic.ApplicationServices;
using NLog;

namespace Eon.Kiosk.Service
{
    public sealed class SingleInstanceApplicationWrapper : WindowsFormsApplicationBase
    {
        private App app;

        /// <summary>
        /// Ctor.
        /// </summary>
        public SingleInstanceApplicationWrapper()
        {
            IsSingleInstance = true;
        }

        /// <summary>
        /// When overridden in a derived class, allows for code to run when the application starts.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Boolean"/> that indicates if the application should continue starting up.
        /// </returns>
        /// <param name="eventArgs"><see cref="T:Microsoft.VisualBasic.ApplicationServices.StartupEventArgs"/>. Contains the command-line arguments of the application and indicates whether the application startup should be canceled.</param>
        protected override bool OnStartup(StartupEventArgs eventArgs)
        {
            app = new App();
            app.Run();
            return false;
        }

        /// <summary>
        ///     При запуске второго приложения.
        /// </summary>
        /// <param name="e">Параметры запуска.</param>
        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs e)
        {
            LogManager.GetCurrentClassLogger().Warn("Application already running");
            base.OnStartupNextInstance(e);
        }
    }
}