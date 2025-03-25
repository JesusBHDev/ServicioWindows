using System;
using System.IO;

namespace ServicioWindows.Utils
{
    public static class Logger
    {
        private static string rutaLog = AppDomain.CurrentDomain.BaseDirectory + "\\log.txt";

        public static void EscribirLog(string mensaje)
        {
            using (StreamWriter sw = new StreamWriter(rutaLog, true))
            {
                sw.WriteLine($"{DateTime.Now}: {mensaje}");
            }
        }
    }
}
