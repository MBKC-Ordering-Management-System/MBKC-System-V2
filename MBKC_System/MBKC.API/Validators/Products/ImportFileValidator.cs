using FluentValidation;
using MBKC.Service.DTOs.Products;
using MBKC.Service.Utils;

namespace MBKC.API.Validators.Products
{
    public class ImportFileValidator : AbstractValidator<ImportFileRequest>
    {
        public ImportFileValidator()
        {

            RuleFor(ip => ip.file)
            .Cascade(CascadeMode.Stop)
            .NotNull().WithMessage("{PropertyName} is not null.")
            .ChildRules(ckcr =>
            {

                ckcr.RuleFor(cpr => cpr.FileName)
                    .Cascade(CascadeMode.Stop)
                    .Must(FileUtil.HaveSupportedFileTypeExcel).WithMessage("File excel is required extension type .xlsx.");
            });
        }
    }
}
