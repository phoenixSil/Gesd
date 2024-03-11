using FluentValidation;

namespace Gesd.Api.Dtos.Validators
{
    public class FileValidator : AbstractValidator<IFileDto>
    {
        public FileValidator()
        {
            // pas de validation a faire ici 
        }
    }
}
