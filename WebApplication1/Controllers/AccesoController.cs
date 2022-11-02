using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;

using WebApplication1.Models;

using System.Data.SqlClient;
using System.Data;
using Microsoft.Data.SqlClient;

namespace WebApplication1.Controllers
{
    public class AccesoController : Controller
    {

        static readonly string cadena = "Data Sourse=DESKTOP-QRAVBP7; Initial Catalog=petHome; Integrated Security=true";

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Registrar()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar(Usuario oUsuario, SqlConnection cn, SqlConnection pn)
        {
            bool registrado;
            string mensaje;

            if (oUsuario.Clave == oUsuario.ConfirmarClave)
            {
                oUsuario.Clave = ConvertirSha256(oUsuario.Clave);
            }
            else
            {
                ViewData["Mensaje"] = "Las constraseñas no coinciden";
                return View();
            }

            SqlConnection sqlConnection = new SqlConnection(cadena);
            using (SqlConnection cn = sqlConnection)
            {

                SqlCommand cmd = new SqlCommand("sp_RegistrarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.Parameters.Add("Registrado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                cmd.Parameters.Add("Mensaje", SqlDbType.VarChar, 100).Direction = ParameterDirection.Output;
                cmd.CommandType = CommandType.StoredProcedure;

                pn.Open();

                cmd.ExecuteNonQuery();

                registrado = Convert.ToBoolean(cmd.Parameters["Registrado"].Value);
                mensaje = cmd.Parameters["Mensaje"].Value.ToString();
            }

            ViewData["Mensaje"] = mensaje;
            if (registrado)
            {
                return RedirectToAction("Login", "Acceso");
            }
            else
            {
                return View();
            }

        }



        public static string ConvertirSha256(string texto)
        {
            //using System.Text;
            //Usar la referencia de "System.Security.Cryptography"

            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256.Create())
                {
                    Encoding enc = Encoding.UTF8;
                    byte[] result = hash.ComputeHash(enc.GetBytes(texto));
                    foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));

                 }

              return Sb.ToString();

          }

    }
}

