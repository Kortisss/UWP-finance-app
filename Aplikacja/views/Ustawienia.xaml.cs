using Aplikacja.modele;
using Aplikacja.Validators;
using DataAccessLibrary;
using FluentValidation.Results;
using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.ComponentModel;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Aplikacja.views
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class Ustawienia : Page
    {
        Uzytkownicy uzytkownik = new Uzytkownicy();
        private bool wyslano = false;
        public static Random rnd = new Random();
        int a = rnd.Next(0, 10000);

        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public Ustawienia()
        {
            this.InitializeComponent();
            aktEmail.Text = localSettings.Values["loggedUser"].ToString();
        }

        private void btnZmienEmail(object sender, RoutedEventArgs e)
        {
            if (DataAccess.sprawdzUzytkownika(txtZmienEmail.Text).Equals("Blad"))
            {
                txtError.Text = "";
                int idUzytkownika = Convert.ToInt32(DataAccess.sprawdzUzytkownika(aktEmail.Text));
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("uwpApplication", "uwpapplication.uwpapplication@interia.pl"));
                message.To.Add(new MailboxAddress("uzytkownik", txtZmienEmail.Text));
                message.Subject = "Kod aktywacyjny konto UWP; Aplikacja finansowa";

                string txt = @"Czy na pewno chcesz zmienić swojego maila? ,<br>
                               <p>Jeśli tak, wpisz ten kod do swojej aplikacji aby zmienić adres mail:</p><p>" + a.ToString() + @"</p><br>
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
                if (!StandardPopup.IsOpen)
                {
                    StandardPopup.IsOpen = true;
                }
            }
            else
            { txtError.Text = "taki adres znajduje się już w bazie!"; }
        }
               
        private void btnCofnij(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void btnZmienHaslo(object sender, RoutedEventArgs e)
        {
            BindingList<string> errList = new BindingList<string>();

            uzytkownik.haslo = psBZmienHaslo.Password;

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

            Output.ItemsSource = errList;

            if (errList.Count == 0)
            {
                if (psBZmienHaslo.Password.Equals(psBZmienHasloPotwierdz.Password))
                {
                    int idUzytkownika = Convert.ToInt32(DataAccess.sprawdzUzytkownika(aktEmail.Text));
                    DataAccess.updateHasloUzytkownika(psBZmienHaslo.Password, idUzytkownika);
                    Frame.GoBack();
                }
                else { txtError2.Text = "Hasla nie sa identyczne!"; }
            }
        }

        private void potwierdz_Click(object sender, RoutedEventArgs e)
        {
            int idUzytkownika = Convert.ToInt32(DataAccess.sprawdzUzytkownika(aktEmail.Text));
            if (wyslano)
            {
                if (kodTextBox.Text == a.ToString())
                {
                    Debug.WriteLine("kody się zgadzają!!!");
                    DataAccess.updateEmailUzytkownika(txtZmienEmail.Text, idUzytkownika);
                    Debug.WriteLine("poprawnie zarejestrowano użytkownika!");
                    wyslano = false;
                    localSettings.Values["loggedUser"] = txtZmienEmail.Text;
                    Frame.GoBack();
                }
                else
                {
                    txtWyswietl.Text = "Błędny kod, spróbuj ponownie.";
                }

            }
        }

        private void psBZmienHaslo_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
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
                ToolTipService.SetToolTip(psBZmienHaslo, toolTip);
            }
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (revealModeCheckBox.IsChecked == true)
            {
                 psBZmienHaslo.PasswordRevealMode = PasswordRevealMode.Visible;
                psBZmienHasloPotwierdz.PasswordRevealMode = PasswordRevealMode.Visible;
            }
            else
            {
                psBZmienHaslo.PasswordRevealMode = PasswordRevealMode.Hidden;
                psBZmienHasloPotwierdz.PasswordRevealMode = PasswordRevealMode.Hidden;
            }
        }
    }
}
