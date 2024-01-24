using System.Text.RegularExpressions;

namespace DOT_NET_WEB_API_AUTH.Services.User
{
    public partial class UserService
    {
        public static bool IsValidEmail(string email)
        {
            // Utilizar una expresión regular para validar el formato del correo electrónico
            string pattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);

            return regex.IsMatch(email);
        }
    }
}
