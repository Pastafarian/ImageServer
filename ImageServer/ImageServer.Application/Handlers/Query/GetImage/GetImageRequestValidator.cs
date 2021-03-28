using FluentValidation;
using ImageServer.Application.Requests;

namespace ImageServer.Application.Handlers.Query.GetImage
{
    public class GetImageRequestValidator : AbstractValidator<GetImageRequest>
    {
        public GetImageRequestValidator()
        {
            RuleFor(x => x.FileName).NotEmpty().WithMessage("You must include a file name");
            RuleFor(x => x.BackgroundColor).Matches("^#([0-9A-F]{3}){1,2}$").When(x => !string.IsNullOrWhiteSpace(x.BackgroundColor)).WithMessage("Background colour must be a valid hex value.");
            RuleFor(x => x.MaxWidth).GreaterThan(0).When(x => x.MaxWidth.HasValue).WithMessage("Max with must be greater than 0");
            RuleFor(x => x.MaxHeight).GreaterThan(0).When(x => x.MaxHeight.HasValue).WithMessage("Max height must be greater than 0");
            RuleFor(x => x.MaxWidth).NotEmpty().When(x => !x.MaxHeight.HasValue).WithMessage("Max with or max height must be specified");
            RuleFor(x => x.MaxHeight).NotEmpty().When(x => !x.MaxWidth.HasValue).WithMessage("Max with or max height must be specified");
        }
    }
}
