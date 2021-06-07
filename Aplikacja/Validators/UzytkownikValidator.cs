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
    class UzytkownikValidator : AbstractValidator<Uzytkownicy>
    {
        public UzytkownikValidator()
        {
            RuleFor(p => p.email)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName}: jest puste!")
                .Must(EmailExistance).WithMessage("{PropertyName}: już istnieje w bazie")
                .Must(BeValidEmail).WithMessage("{PropertyName}: brak składni emaila!");
                

            RuleFor(p => p.imie)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName}: jest puste!")
                .Length(2, 15).WithMessage("Długość({TotalLength}) pola {PropertyName}: niewłaściwa!")
                .Must(BeValidFirstName).WithMessage("{PropertyName}: Zawiera niedozwolone znaki!");
            //^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[.#?!@$%^&*-]).{8,}$

            RuleFor(p => p.haslo)
                .Cascade(CascadeMode.Continue)
                .NotEmpty().WithMessage("{PropertyName}: jest puste!")
                .Length(8, 15).WithMessage("{PropertyName}: Długość({TotalLength}) niewłaściwa!")
                .Must(BeValidPasswordDigit).WithMessage("{PropertyName}: Nie zawiera cyfry!")
                .Must(BeValidPasswordLowerCase).WithMessage("{PropertyName}: Nie zawiera małej litery!")
                .Must(BeValidPasswordUpperCase).WithMessage("{PropertyName}: Nie zawiera dużej litery!")
                .Must(BeValidPassword).WithMessage("{PropertyName}: Hasło niewłaściwe!");
        }

        protected bool EmailExistance(string email)
        {
            string recivedEmail = DataAccess.checkEmail(email)[0];
            if (recivedEmail == "0")
            {
                Debug.WriteLine("nie ma jeszcze takiego emaila");
                return true;
            }
            else
            { 
                
                Debug.WriteLine("jest taki email w bazie");
                return false;
            }
        }
        protected bool BeValidEmail(string email)
        {
            Regex rx = new Regex(@"\b[\w\.-]+@[\w\.-]+\.\w{2,4}\b");
            Match match = rx.Match(email);
            if (match.Success && match.Value.Length == email.Length)
            {
                Debug.WriteLine("success");
                return true;
            }
            else
            {
                Debug.WriteLine("false");
                return false;
            }
        }

        protected bool BeValidPasswordLowerCase(string name)
        {
            Regex rx = new Regex(@"(?=.*[a-z])");
            Match match = rx.Match(name);
            if (match.Success)
            {
                Debug.WriteLine("LowerCase success");
                return true;
            }
            else
            {
                Debug.WriteLine("LowerCase false");
                return false;
            }
        }

        protected bool BeValidPasswordUpperCase(string name)
        {
            Regex rx = new Regex(@"(?=.*[A-Z])");
            Match match = rx.Match(name);
            if (match.Success)
            {
                Debug.WriteLine("UpperCase success");
                return true;
            }
            else
            {
                Debug.WriteLine("UpperCase false");
                return false;
            }
        }
        protected bool BeValidPasswordDigit(string name)
        {
            Regex rx = new Regex(@"(?=.*\d)");
            Match match = rx.Match(name);
            if (match.Success)
            {
                Debug.WriteLine("digit success");
                return true;
            }
            else
            {
                Debug.WriteLine("digit false");
                return false;
            }
        }
        protected bool BeValidPassword(string password)
        {
            Regex rx = new Regex(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[.#?!@$%^&*-]).{8,}$");
            Match match = rx.Match(password);
            if (match.Success && match.Value.Length == password.Length)
            {
                Debug.WriteLine(" password success");
                return true;
            }
            else
            {
                Debug.WriteLine("password false");
                return false;
            }
        }
        protected bool BeValidFirstName(string name)
        {
            name = name.Replace(" ", "");
            name = name.Replace("-", "");
            return name.All(Char.IsLetter);
        }
    }
}
