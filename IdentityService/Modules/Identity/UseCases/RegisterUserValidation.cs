using FluentValidation;

namespace IdentityService.Modules.Identity.UseCases;

internal sealed class RegisterUserValidation : AbstractValidator<RegisterDto>
{

    public RegisterUserValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithName("name").WithMessage("Name is required")
            .MaximumLength(100).WithName("name").WithMessage("Name cannot exceed 100 characters");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithName("email").WithMessage("Email is required")
            .EmailAddress().WithName("email").WithMessage("Invalid email format");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithName("password").WithMessage("Password is required")
            .MinimumLength(8).WithName("password").WithMessage("Password must have at least 8 characters")
            .MaximumLength(21).WithName("password").WithMessage("Password must have at least 21 characters")
            .Must(ContainUppercase).WithName("password").WithMessage("Password must contain at least one uppercase letter")
            .Must(ContainSymbol).WithName("password").WithMessage("Password must contain at least one symbol");
    }
    
    private static bool ContainUppercase(string password)
    {
        return password.Any(char.IsUpper);
    }

    private static bool ContainSymbol(string password)
    {
        return password.Any(c => !char.IsLetterOrDigit(c));
    }
}