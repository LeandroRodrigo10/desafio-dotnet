using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser
{
    /// <summary>
    /// Validation rules for updating a user. Ensures required fields are provided
    /// and formatted correctly. Username and phone are optional but, if
    /// supplied, must respect the defined constraints.
    /// </summary>
    public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserValidator()
        {
            // The user identifier must always be provided
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("User Id is required");

            // Email is mandatory and must be in a valid format
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format");

            // Username is optional; if supplied it cannot be blank and must be
            // at least three characters long
            When(x => x.Username != null, () =>
            {
                RuleFor(x => x.Username)
                    .Must(u => !string.IsNullOrWhiteSpace(u))
                    .WithMessage("Username cannot be empty")
                    .MinimumLength(3)
                    .WithMessage("Username must be at least 3 characters long");
            });

            // Phone is optional; if supplied it must contain between 10 and 15 digits
            RuleFor(x => x.Phone)
                .Matches(@"^\d{10,15}$").When(x => !string.IsNullOrWhiteSpace(x.Phone))
                .WithMessage("Phone must contain between 10 and 15 digits");

            // Password is optional; if supplied it must have at least eight characters
            RuleFor(x => x.Password)
                .MinimumLength(8).When(x => !string.IsNullOrWhiteSpace(x.Password))
                .WithMessage("Password must have at least 8 characters");
        }
    }
}
