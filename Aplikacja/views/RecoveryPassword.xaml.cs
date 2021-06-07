using Aplikacja.modele;
using Aplikacja.Validators;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using DataAccessLibrary;
using MimeKit;
using MailKit.Net.Smtp;
using System.Diagnostics;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Aplikacja.views
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class RecoveryPassword : Page
    {
        Uzytkownicy uzytkownik = new Uzytkownicy();
        private bool wyslano = false;
        public static Random rnd = new Random();
        int a = rnd.Next(0, 10000);
        public RecoveryPassword()
        {
            this.InitializeComponent();
            
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            if (e.Parameter is string && !string.IsNullOrWhiteSpace((string)e.Parameter)) 
            {
                UsernameTextBox.Text = e.Parameter.ToString();
            }
        }
        private void ResetPassword(object sender, RoutedEventArgs e)
        {
            string czyjest;
            czyjest = DataAccess.checkEmail(UsernameTextBox.Text.ToString())[0];
            
            if (czyjest == "1")
            {
                if (!StandardPopup.IsOpen)
                {
                    StandardPopup.IsOpen = true;
                    grid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("uwpApplication", "uwpapplication.uwpapplication@interia.pl"));
                message.To.Add(new MailboxAddress("", UsernameTextBox.Text.ToString()));
                message.Subject = "Kod odzyskiwania hasła konto UWP; Aplikacja finansowa";

                string txt = @"Odzyskiwanie hasła,<br>
                               <p>Wpisz ten kod do swojej aplikacji aby zmienić swoje hasło:</p><p>" + a.ToString() + @"</p><br>
                               <p>-- UWPApplication</p>";

                message.Body = new TextPart("Html")
                {
                    Text = txt
                };

                using (var client = new SmtpClient())
                {
                    client.Connect("poczta.interia.pl", 587, false);

                    // Note: only needed if the SMTP server requires authentication
                    client.Authenticate("uwpapplication.uwpapplication@interia.pl", "1234567Mm.");
                    Debug.WriteLine("The mail has been sent successfully !!");
                    client.Send(message);
                    client.Disconnect(true);

                    wyslano = true;
                }

            }
            else
            {
                ErrorMessage.Text = "Nie znaleziono konta o podanym adresie e-mail. Sprawdz email i spróbuj ponownie";
            }
            

        }

        private void goBack(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ActiveButton();   
        }

        private void UsernameTextBox_Loaded(object sender, RoutedEventArgs e)
        {
            ActiveButton();
        }
        private void ActiveButton()
        {
            BindingList<string> errList = new BindingList<string>();
            OnlyEmailValidator validator = new OnlyEmailValidator();
            uzytkownik.email = UsernameTextBox.Text.ToString();
            ValidationResult results2 = validator.Validate(uzytkownik);
            if (results2.IsValid == false)
            {
                foreach (ValidationFailure faliure in results2.Errors)
                {
                    errList.Add($" {faliure.ErrorMessage}");
                }
            }
            if (errList.Count == 0)
            {
                PasswordResset.IsEnabled = true;
            }
 
            else
            {
                PasswordResset.IsEnabled = false;
            }
        }

        private void potwierdz_Click(object sender, RoutedEventArgs e)
        {
            if (wyslano)
            {
                if (kodTextBox.Text == a.ToString())
                {
                    Debug.WriteLine("kody się zgadzają!!!");
                    wyslano = false;
                    if (!StandardPopup2.IsOpen)
                    {
                        StandardPopup.IsOpen = false;
                        StandardPopup2.IsOpen = true;
                        grid.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }

                }
                else
                {
                    txtWyswietl.Text = "Błędny kod, spróbuj ponownie.";
                }

            }
        }

        private void Cofnij_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void potwierdz_Click2(object sender, RoutedEventArgs e)
        {
            BindingList<string> errList = new BindingList<string>();

            uzytkownik.haslo = haslo1.Password;

            //Validate my data

            ZmienHasloValidator validator = new ZmienHasloValidator();

            ValidationResult results = validator.Validate(uzytkownik);

            if (results.IsValid == false)
            {
                foreach (ValidationFailure faliure in results.Errors)
                {
                    errList.Add($" {faliure.ErrorMessage}");
                }
            }

            if (errList.Count == 0)
            {
                if (haslo1.Password.Equals(haslo2.Password))
                {
                    int idUzytkownika = Convert.ToInt32(DataAccess.sprawdzUzytkownika(UsernameTextBox.Text));
                    DataAccess.updateHasloUzytkownika(haslo1.Password, idUzytkownika);
                    Frame.GoBack();
                }
                else
                {
                    txtNiepasuje.Text = "Hasła się nie zgadzają.";
                }
            }
            else
            {
                txtNiepasuje.Text = "Hasło musi zawierać 1 znak specjalny,1 liczbę, 1 dużą literę,\n Długość hasła 8-15 znaków";
            }
            
        }
    }
}
