using Aplikacja.modele;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DataAccessLibrary;

namespace Aplikacja.Validators
{
    class UzytkownikLoginValidator : AbstractValidator<Uzytkownicy>
    {
        public UzytkownikLoginValidator()
        {
            RuleFor(p => p.email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName}: jest puste!")
                .Must(checkEmail).WithMessage("{PropertyName}: brak w bazie danych");
            //^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[.#?!@$%^&*-]).{8,}$

            RuleFor(p => p.haslo)
                .NotEmpty().WithMessage("{PropertyName}: jest puste!");
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
