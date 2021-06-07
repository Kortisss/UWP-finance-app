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


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Aplikacja
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DodawanieWpisu : Page
    {
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public DodawanieWpisu()
        {
            this.InitializeComponent();
            test.Text = localSettings.Values["loggedUser"].ToString();
        }
        private void btnDodajWpis(object sender, RoutedEventArgs e)
        {
            int idUzytkownika = Convert.ToInt32(DataAccess.sprawdzUzytkownika(test.Text));
            String kwotaWpisana = txtKwota.Text.Replace(",", ".");
            if (kwotaWpisana != "")
            {

                if (DatePickerData.SelectedDate != null)
                {
                    string data = DatePickerData.Date.ToString("yyyy-MM-dd");
                    double kwota = Convert.ToDouble(kwotaWpisana);
                    DataAccess.dodajWpisUzytkownika(kwota, txtOpis.Text, data, idUzytkownika, Convert.ToString(comboBoxTypWydatku.SelectedItem)); //trzeba jeszcze jakos zdjecie dodawac 
                    Frame.GoBack();
                }
                else
                {
                    test2.Text = "Wprowadz jakas date!";
                }
            }
            else
            {
                test2.Text = "Wprowadz jakas kwote!";
            }
        }

        private void btnCofnij(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void txtKwota_TextChanged(object sender, TextChangedEventArgs e)
        {
            int count = 0;
            foreach (char przec in txtKwota.Text)
                if (przec == ','||przec == '.')
                {
                    count++;
                    if (count > 1)
                    {
                        txtKwota.Text = "";
                    }
                }
            

            double kwotaWpisana = 0;
            try
            {
                kwotaWpisana = Convert.ToDouble(txtKwota.Text);
            }
            catch
            {
                txtKwota.Text = "";
            }
        }

        private void btnUstawienia_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(views.Ustawienia));
        }
    }
}
