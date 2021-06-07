using System;
using System.Collections.Generic;
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
using Aplikacja.modele;
using Aplikacja.Validators;
using FluentValidation.Results;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace Aplikacja
{
    public sealed partial class Rejestracja : Page
    {
        
        private bool wyslano = false;
        public static Random rnd = new Random();
        int a = rnd.Next(0, 10000);

        Uzytkownicy uzytkownik = new Uzytkownicy();
        DispatcherTimer dt;
        public Rejestracja()
        {
            this.InitializeComponent();
            dt = new DispatcherTimer();
            dt.Interval = new TimeSpan(0, 0, 0, 0, 50);
            dt.Tick += Dt_Tick;
            dt.Start();
        }

        private void Dt_Tick(object sender, object e)
        {
            var cl = Window.Current.CoreWindow.GetKeyState(Windows.System.VirtualKey.CapitalLock);
            if (cl.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Locked))
            {
                CapsLock.Text = "CapsLock jest włączony";
            }
            else
            {
                CapsLock.Text = "";
            }
        }

        private void btnRejestracja_Click(object sender, RoutedEventArgs e)
        {
            BindingList<string> errList = new BindingList<string>();

            uzytkownik.email = email.Text;
            uzytkownik.imie = imie.Text;
            uzytkownik.haslo = haslo.Password;

            //Validate my data

            UzytkownikValidator validator = new UzytkownikValidator();

            ValidationResult results = validator.Validate(uzytkownik);

            if (results.IsValid == false)
            {
                foreach (ValidationFailure faliure in results.Errors)
                {
                    errList.Add($" {faliure.ErrorMessage}");
                }
            }
            

            Output.ItemsSource = errList;

            //tu dodaj do bazy
            if (errList.Count == 0)
            {
                if (!StandardPopup.IsOpen)
                {
                    StandardPopup.IsOpen = true;
                }

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("uwpApplication", "uwpapplication.uwpapplication@interia.pl"));
                message.To.Add(new MailboxAddress(uzytkownik.imie, uzytkownik.email));
                message.Subject = "Kod aktywacyjny konto UWP; Aplikacja finansowa";

                string txt = @"Dziękujemy za rejestrację,<br>
                               <p>Wpisz ten kod do swojej aplikacji aby założyć konto:</p><p>" + a.ToString()+ @"</p><br>
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
                //DataAccess.dodajUzytkownika(uzytkownik.email, uzytkownik.imie, uzytkownik.haslo);
                //Thread.Sleep(2000);
                //Frame.GoBack();
            }
        }

        private void btnPowrot_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void potwierdz_Click(object sender, RoutedEventArgs e)
        {
            if (wyslano)
            {
                if (kodTextBox.Text == a.ToString())
                {
                    Debug.WriteLine("kody się zgadzają!!!");
                    DataAccess.dodajUzytkownika(uzytkownik.email, uzytkownik.imie, uzytkownik.haslo);
                    Debug.WriteLine("poprawnie zarejestrowano użytkownika!");
                    wyslano = false;
                    Frame.GoBack();
                }
                else
                {
                    txtWyswietl.Text="Błędny kod, spróbuj ponownie.";
                }
                
            }
            
        }

        private void Cofnij_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void haslo_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ToolTip toolTip = new ToolTip();
            toolTip.Content = 
@"schemat hasła: 
* długość od 8-15 znaków
* minimum jedna: 
    -cyfra 
    -mała litera
    -duża litera
    -znak specjalny: .#?!@$%^&*- ";
            ToolTipService.SetToolTip(haslo, toolTip);
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (revealModeCheckBox.IsChecked == true)
            {
               haslo.PasswordRevealMode = PasswordRevealMode.Visible;
            }
            else
            {
                haslo.PasswordRevealMode = PasswordRevealMode.Hidden;
            }
        }
    }
}
