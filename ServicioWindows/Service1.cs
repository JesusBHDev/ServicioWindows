using System;
using System.Collections.Generic;
using System.ServiceProcess;
using System.Timers;
using ServicioWindows.Models;
using ServicioWindows.Services;
using ServicioWindows.Utils;

namespace ServicioWindows
{
    public partial class Service1 : ServiceBase
    {
        private Timer timer;
        private UsuarioService usuarioService;
        private CorreoService correoService;
        private HashSet<int> usuariosFelicitadosHoy; // Almacena los IDs de usuarios ya felicitados

        public Service1()
        {
            InitializeComponent();
            usuarioService = new UsuarioService();
            correoService = new CorreoService();
            usuariosFelicitadosHoy = new HashSet<int>();
        }

        protected override void OnStart(string[] args)
        {
            Logger.EscribirLog("Servicio iniciado.");

            timer = new Timer(60 * 1000); // Ejecutar cada minuto
            timer.Elapsed += new ElapsedEventHandler(VerificarCumpleaños);
            timer.Start();

            VerificarCumpleaños(null, null);
        }

        protected override void OnStop()
        {
            Logger.EscribirLog("Servicio detenido.");
            timer.Stop();
        }

        private void VerificarCumpleaños(object sender, ElapsedEventArgs e)
        {
            try
            {
                List<Usuario> usuarios = usuarioService.ObtenerUsuarios();
                List<UsuarioLog> historial = usuarioService.ObtenerHistorialUsuarios();

                if (usuarios.Count == 0)
                {
                    Logger.EscribirLog("No se encontraron usuarios.");
                    return;
                }

                DateTime hoy = DateTime.UtcNow.Date;

                foreach (var usuario in usuarios)
                {
                    if (DateTime.TryParse(usuario.FechaNac, out DateTime fechaNacimiento))
                    {
                        if (fechaNacimiento.Day == hoy.Day && fechaNacimiento.Month == hoy.Month)
                        {
                            // Verificar si ya se envió el correo hoy
                            if (!usuariosFelicitadosHoy.Contains(usuario.ID))
                            {
                                correoService.EnviarCorreoConExcel(usuario.Correo, usuario.Nombre, historial);
                                usuariosFelicitadosHoy.Add(usuario.ID);
                                Logger.EscribirLog($"Correo con historial enviado a {usuario.Nombre} ({usuario.Correo})");
                            }
                            else
                            {
                                Logger.EscribirLog($"Correo ya enviado hoy a {usuario.Nombre}, no se enviará nuevamente.");
                            }
                        }
                    }
                    else
                    {
                        Logger.EscribirLog($"Error al convertir la fecha de nacimiento para {usuario.Nombre}.");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.EscribirLog($"Error en VerificarCumpleaños: {ex.Message}");
            }
        }
    }
}
