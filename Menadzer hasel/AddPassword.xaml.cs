using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Menadzer_hasel
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class AddPassword : Page
    {
        public AddPassword()
        {
            this.InitializeComponent();
        }

        Database db = new Database();

        private async void GeneratePassword_Click(object sender, RoutedEventArgs e)
        {
            if (Length.Text != "" && Regex.IsMatch(Length.Text, @"^\d+$") && Int32.Parse(Length.Text) >= 4)
            {
                int length = Int32.Parse(Length.Text);
                string smallLetters = "qwertyuiopasdfghjklzxcvbnm";
                string bigLetters = "QWERTYUIOPASDFGHJKLZXCVBNM";
                string numbers = "1234567890";
                string specialChars = "!#$%&()*+,-./:<=>?@[]^_{|}";
                Random rand = new Random();
                string charsToRandomize = "";
                string password = "";
                bool requirements = true;

                if (allowSmallLetters.IsChecked == true)
                {
                    charsToRandomize += smallLetters;
                }
                if (allowBigLetters.IsChecked == true)
                {
                    charsToRandomize += bigLetters;
                }
                if (allowNumbers.IsChecked == true)
                {
                    charsToRandomize += numbers;
                }
                if (allowSpecialChars.IsChecked == true)
                {
                    charsToRandomize += specialChars;
                }

                if (charsToRandomize == "")
                {
                    var dialog = new MessageDialog("Minimum jedno pole musi zostać ustawione jako dozwolone");
                    await dialog.ShowAsync();
                }
                else
                {
                    do
                    {
                        requirements = true;
                        password = "";
                        for (int i = 0; i < length; i++)
                        {
                            int index = rand.Next(0, charsToRandomize.Length);
                            password += charsToRandomize[index];
                        }

                        if (requireSmallLetters.IsChecked == true)
                        {
                            if (password.IndexOfAny(smallLetters.ToCharArray()) == -1)
                            {
                                requirements = false;
                            }
                        }
                        if (requireBigLetters.IsChecked == true)
                        {
                            if (password.IndexOfAny(bigLetters.ToCharArray()) == -1)
                            {
                                requirements = false;
                            }
                        }
                        if (requireNumbers.IsChecked == true)
                        {
                            if (password.IndexOfAny(numbers.ToCharArray()) == -1)
                            {
                                requirements = false;
                            }
                        }
                        if (requireSpecialChars.IsChecked == true)
                        {
                            if (password.IndexOfAny(specialChars.ToCharArray()) == -1)
                            {
                                requirements = false;
                            }
                        }
                    }
                    while (requirements == false);
                    Password.Text = password;
                }
            }
            else
            {
                var dialog = new MessageDialog("Długość hasła musi być podana oraz musi być większa lub równa 4");
                await dialog.ShowAsync();
            }
        }

        private async void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (Name.Text != "" && Login.Text != "" && Password.Text != "")
            {
                db.addToDB(Name.Text, Login.Text, Password.Text);
            }
            else
            {
                var dialog = new MessageDialog("Pola nie mogą być puste");
                await dialog.ShowAsync();
            }
        }

        private void RequireSmallLetters_Checked(object sender, RoutedEventArgs e)
        {
            allowSmallLetters.IsChecked = true;
        }

        private void RequireBigLetters_Checked(object sender, RoutedEventArgs e)
        {
            allowBigLetters.IsChecked = true;
        }

        private void RequireNumbers_Checked(object sender, RoutedEventArgs e)
        {
            allowNumbers.IsChecked = true;
        }

        private void RequireSpecialChars_Checked(object sender, RoutedEventArgs e)
        {
            allowSpecialChars.IsChecked = true;
        }

        private void AllowNumbers_Unchecked(object sender, RoutedEventArgs e)
        {
            requireNumbers.IsChecked = false;
        }

        private void AllowSmallLetters_Unchecked(object sender, RoutedEventArgs e)
        {
            requireSmallLetters.IsChecked = false;
        }

        private void AllowBigLetters_Unchecked(object sender, RoutedEventArgs e)
        {
            requireBigLetters.IsChecked = false;
        }

        private void AllowSpecialChars_Unchecked(object sender, RoutedEventArgs e)
        {
            requireSpecialChars.IsChecked = false;
        }
    }
}
