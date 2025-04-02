using System.ServiceProcess;
using LibreriaDSW;
using LibreriaDSW.Utils;

namespace ServicioWindows
{
    public partial class Service1 : ServiceBase
    {
        private ServicioUsuario ServicioUsuario;

        public Service1()
        {
            InitializeComponent();
            ServicioUsuario = new ServicioUsuario();
        }

        protected override void OnStart(string[] args)
        {
            Logger.EscribirLog("Servicio iniciado.");
            ServicioUsuario.Iniciar();
        }

        protected override void OnStop()
        {
            Logger.EscribirLog("Servicio detenido.");
            ServicioUsuario.Detener();
        }
    }
}
