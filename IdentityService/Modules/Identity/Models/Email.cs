using System.Net.Mail;
using FluentValidation;
using FluentValidation.Results;

namespace IdentityService.Modules.Identity.Models;

internal class Email
{
    public string Address { get; }

    public Email(string email)
    {
        if (!IsValidEmail(email))
            throw new ValidationException([
                new ValidationFailure("email", "Invalid email format.")
            ]);
        
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