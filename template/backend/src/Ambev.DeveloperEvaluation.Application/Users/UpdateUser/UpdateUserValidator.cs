using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser
{
    /// <summary>
    /// Validator for UpdateUserCommand
    /// </summary>
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User Id is required");

            RuleFor(x => x.Username)
                .NotEmpty().WithMessage("Username is required")
                .MinimumLength(3).WithMessage("Username must be at least 3 characters long");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Phone)
                .Matches(@"^\d{10,15}$").When(x => !string.IsNullOrEmpty(x.Phone))
                .WithMessage("Phone must contain between 10 and 15 digits");

            RuleFor(x => x.Password)
                .MinimumLength(8).When(x => !string.IsNullOrEmpty(x.Password))
                .WithMessage("Password must have at least 8 characters");
        }
    }
}
