using Aplikacja.modele;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataAccessLibrary;
namespace Aplikacja.Validators
{
    class OnlyEmailValidator : AbstractValidator<Uzytkownicy>
    {
        public OnlyEmailValidator()
        {
            RuleFor(p => p.email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName}: jest puste!")
                .Must(checkEmail).WithMessage("{PropertyName}: brak w bazie danych");
            //^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[.#?!@$%^&*-]).{8,}$

        }
        protected bool checkEmail(string email)
        {
            string recivedEmail = DataAccess.checkEmail(email)[0];
            if (recivedEmail == "1")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
