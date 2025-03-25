using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using ClosedXML.Excel;
using ServicioWindows.Utils;
using ServicioWindows.Models;
using ServicioWindows.Services;

namespace ServicioWindows.Services
{
    public class CorreoService
    {
        private string correoEmisor = "jesus.bh.dev@gmail.com";
        private string contraseña = "ieulsuzmyqlpivxi"; // Usa variables de entorno para mayor seguridad

        public void EnviarCorreoConExcel(string destinatario, string nombre, List<UsuarioLog> historial)
        {
            try
            {
                // 1. Generar el archivo Excel
                string rutaArchivo = GenerarExcel(historial);

                using (SmtpClient client = new SmtpClient("smtp.gmail.com"))
                {
                    client.Port = 587;
                    client.Credentials = new NetworkCredential(correoEmisor, contraseña);
                    client.EnableSsl = true;

                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress(correoEmisor);
                    mail.To.Add(destinatario);
                    mail.Subject = "¡Feliz Cumpleaños!";
                    mail.Body = $"Hola {nombre},\n\n¡Feliz cumpleaños! Adjunto encontrarás el historial de usuarios.\n\nSaludos,\nTu Empresa";

                    // 2. Adjuntar el archivo Excel
                    if (File.Exists(rutaArchivo))
                    {
                        mail.Attachments.Add(new Attachment(rutaArchivo));
                    }

                    // 3. Enviar correo
                    client.Send(mail);
                    Logger.EscribirLog($"Correo con Excel enviado correctamente a {destinatario}.");
                }

                // 4. Eliminar el archivo después de enviarlo
                File.Delete(rutaArchivo);
            }
            catch (Exception ex)
            {
                Logger.EscribirLog($"Error al enviar correo a {destinatario}: {ex.Message}");
            }
        }

        private string GenerarExcel(List<UsuarioLog> historial)
        {
            // Genera un nombre de archivo único usando GUID
            string rutaArchivo = Path.Combine(Path.GetTempPath(), $"Historial_{Guid.NewGuid()}.xlsx");

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Historial");

                // Encabezados
                worksheet.Cell(1, 1).Value = "ID";
                worksheet.Cell(1, 2).Value = "Nombre";
                worksheet.Cell(1, 3).Value = "Apellido P";
                worksheet.Cell(1, 4).Value = "Apellido M";
                worksheet.Cell(1, 5).Value = "Correo";
                worksheet.Cell(1, 6).Value = "Acción";
                worksheet.Cell(1, 7).Value = "Fecha";
                worksheet.Cell(1, 8).Value = "UsuarioID";

                // Aplicar estilo a los encabezados
                var headerRange = worksheet.Range("A1:H1");
                headerRange.Style.Font.Bold = true; // Negrita
                headerRange.Style.Font.FontColor = XLColor.White; // Texto en blanco
                headerRange.Style.Fill.BackgroundColor = XLColor.Black; // Fondo negro
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center; // Centrar texto
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center; // Alinear verticalmente al centro

                // Ajustar altura de la fila del encabezado
                worksheet.Row(1).Height = 20;

                // Llenar los datos con alternancia de colores
                int fila = 2;
                foreach (var log in historial)
                {
                    worksheet.Cell(fila, 1).Value = log.ID;
                    worksheet.Cell(fila, 2).Value = log.Nombre;
                    worksheet.Cell(fila, 3).Value = log.ApellidoP;
                    worksheet.Cell(fila, 4).Value = log.ApellidoM;
                    worksheet.Cell(fila, 5).Value = log.Correo;
                    worksheet.Cell(fila, 6).Value = log.Accion;
                    worksheet.Cell(fila, 7).Value = log.Fecha;
                    worksheet.Cell(fila, 8).Value = log.UsuarioID;

                    // Alternar colores: Si la fila es par, fondo gris; si es impar, fondo blanco
                    if (fila % 2 == 0)
                    {
                        worksheet.Range($"A{fila}:H{fila}").Style.Fill.BackgroundColor = XLColor.LightGray;
                    }
                    else
                    {
                        worksheet.Range($"A{fila}:H{fila}").Style.Fill.BackgroundColor = XLColor.White;
                    }

                    fila++;
                }

                // Ajustar tamaño de columnas automáticamente
                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(rutaArchivo);
            }

            return rutaArchivo;
        }

    }
}
