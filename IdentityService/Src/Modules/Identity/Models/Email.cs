using System.Net.Mail;

namespace IdentityService.Modules.Identity.Models;

internal class Email
{
    public string Address { get; }

    public Email(string email)
    {
        if (!IsValidEmail(email))
            throw new ArgumentException("E-mail inválido.", nameof(email));

        Address = email;
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}