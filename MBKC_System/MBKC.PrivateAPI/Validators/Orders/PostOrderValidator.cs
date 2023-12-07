using FluentValidation;
using MBKC.PrivateAPI.Validators.OrderDetails;
using MBKC.Repository.Enums;
using MBKC.Service.DTOs.Orders;

namespace MBKC.PrivateAPI.Validators.Orders
{
    public class PostOrderValidator: AbstractValidator<PostOrderRequest>
    {
        public PostOrderValidator()
        {
            RuleFor(x => x.OrderPartnerId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.");
            
            RuleFor(x => x.DisplayId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.");
            
            RuleFor(x => x.Address)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.");

            RuleFor(x => x.ShipperName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.");
            
            RuleFor(x => x.ShipperPhone)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.");
            
            RuleFor(x => x.CustomerName)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.");
            
            RuleFor(x => x.CustomerPhone)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.");
            
            RuleFor(x => x.Note)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.");
            
            RuleFor(x => x.PaymentMethod)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .IsEnumName(typeof(OrderEnum.PaymentMethod), caseSensitive: false).WithMessage("{PropertyName} is required payment methods such as: Cash or Cashless");
            
            RuleFor(x => x.DeliveryFee)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} is required greater than or equal to 0 VNĐ.");
            
            
            RuleFor(x => x.SubTotalPrice)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} is required greater than or equal to 0 VNĐ.");
            
            RuleFor(x => x.TotalStoreDiscount)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} is required greater than or equal to 0 VNĐ.");
            
            RuleFor(x => x.FinalTotalPrice)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} is required greater than or equal to 0 VNĐ.");
            
            RuleFor(x => x.TotalStoreDiscount)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} is required greater than or equal to 0 VNĐ.");
            
            RuleFor(x => x.Tax)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} is required greater than or equal to 0 VNĐ.");

            RuleFor(x => x.PartnerOrderStatus)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .NotNull().WithMessage("{PropertyName} is not null.")
                .NotEmpty().WithMessage("{propertyName} is not empty.")
                .IsEnumName(typeof(OrderEnum.Status), caseSensitive: false).WithMessage("{PropertyName} is required some statuses such as: Upcoming or Preparing.");

            RuleFor(x => x.PartnerId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable Id in the system.");
            
            RuleFor(x => x.StoreId)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("{PropertyName} is not suitable Id in the system.");

            RuleFor(x => x.Cutlery)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .GreaterThan(0).WithMessage("{PropertyName} is required greater than 0.");

            RuleFor(x => x.StorePartnerCommission)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .InclusiveBetween(0, 100).WithMessage("{PropertyName} is required in range from 0% to 100%");
            
            RuleFor(x => x.TaxPartnerCommission)
                .Cascade(CascadeMode.StopOnFirstFailure)
                .InclusiveBetween(0, 100).WithMessage("{PropertyName} is required in range from 0% to 100%");

            RuleForEach(x => x.OrderDetails).SetValidator(new PostOrderDetailValidator());
        }
    }
}
