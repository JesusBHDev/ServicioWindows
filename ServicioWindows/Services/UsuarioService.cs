using ServicioWindows.Utils;
using ServicioWindows.Models;
using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;

namespace ServicioWindows.Services
{
    public class UsuarioService
    {
        private string apiUrl = "http://localhost:52127/Services/Usuarios.svc/ObtenerUsuarios";
        private string apiUrl2 = "http://localhost:52127/Services/UsuarioLogs.svc/ObtenerHistorialUsuarios";
        public List<Usuario> ObtenerUsuarios()
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    string json = client.DownloadString(apiUrl);

                    var resultado = JsonConvert.DeserializeObject<dynamic>(json);
                    string usuariosJson = resultado.ObtenerUsuariosResult.ToString();
                    List<Usuario> usuarios = JsonConvert.DeserializeObject<List<Usuario>>(usuariosJson);

                    Logger.EscribirLog($"Usuarios obtenidos: {usuarios.Count}");
                    return usuarios;
                }
            }
            catch (Exception ex)
            {
                Logger.EscribirLog($"Error al obtener usuarios: {ex.Message}");
                return new List<Usuario>();
            }
        }

        public List<UsuarioLog> ObtenerHistorialUsuarios()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = client.GetAsync(apiUrl2).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResponse = response.Content.ReadAsStringAsync().Result;
                        var resultado = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                        return JsonConvert.DeserializeObject<List<UsuarioLog>>(resultado.ObtenerHistorialUsuariosResult.ToString());
                    }
                    else
                    {
                        Logger.EscribirLog("Error al obtener historial de usuarios.");
                        return new List<UsuarioLog>();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.EscribirLog($"Error en ObtenerHistorialUsuarios: {ex.Message}");
                return new List<UsuarioLog>();
            }
        }

    }
}
