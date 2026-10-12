using FluentValidation;
using WebLab.Models;

namespace WebLab.Validators
{
    public class FeedbackViewModelValidator : AbstractValidator<FeedbackViewModel>
    {
        public FeedbackViewModelValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Ім'я є обов'язковим для заповнення.")
                .MinimumLength(3).WithMessage("Ім'я повинно містити щонайменше 3 символи.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Електронна адреса є обов'язковою.")
                .EmailAddress().WithMessage("Введіть коректну адресу електронної пошти.");

            RuleFor(x => x.Age)
                .InclusiveBetween(18, 100).WithMessage("Вік повинен бути від 18 до 100 років.");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Повідомлення не може бути порожнім.")
                .MinimumLength(5).WithMessage("Повідомлення повинно містити мінімум 5 символів.");
        }
    }
}
