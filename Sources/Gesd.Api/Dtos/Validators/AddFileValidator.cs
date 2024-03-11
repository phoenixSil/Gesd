using FluentValidation;

using Microsoft.AspNetCore.Identity;

namespace Gesd.Api.Dtos.Validators
{
    public class AddFileValidator : AbstractValidator<FileToAddDto>
    {
        public AddFileValidator()
        {
            Include(new FileValidator());
        }
    }
}
