using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using System.Threading.Tasks;
using Microsoft.Toolkit.Uwp.UI.Controls;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace Menadzer_hasel
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class PasswordList : Page
    {
        public PasswordList()
        {
            this.InitializeComponent();
            getDB();
        }
        Database db = new Database();
        ObservableCollection<Account> passwordList = new ObservableCollection<Account>();
        ObservableCollection<Account> filteredList;
        bool arePasswordsHidden = true;

        private async void deleteRow_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("Czy na pewno chcesz usunąć element?");
            dialog.Commands.Add(new UICommand("Tak", delegate (IUICommand command)
            {
                if (filter.Text == "")
                {
                    db.removeFromDB(passwordList[passwordGrid.SelectedIndex].id);
                    passwordList.RemoveAt(passwordGrid.SelectedIndex);
                }
                else
                {
                    db.removeFromDB(filteredList[passwordGrid.SelectedIndex].id);
                    filteredList.RemoveAt(passwordGrid.SelectedIndex);
                    passwordList = db.result;
                }
            }));
            dialog.Commands.Add(new UICommand("Nie", delegate (IUICommand command)
            {

            }));
            await dialog.ShowAsync();
        }

        private async void copyPassword_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(300);
            int index = passwordGrid.SelectedIndex;
            getDB();
            if (arePasswordsHidden == true)
            {
                unhidePasswords();
                arePasswordsHidden = true;
            }
            var dataPackage = new DataPackage();
            dataPackage.SetText(filter.Text == "" ? passwordList[index].password : filteredList[index].password);
            Clipboard.SetContent(dataPackage);
            if (arePasswordsHidden == true)
            {
                hidePasswords();
            }
        }

        public void getDB()
        {
            db.getDB();
            passwordList = db.result;
        }

        private async void PasswordGrid_CellEditEnded(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridCellEditEndedEventArgs e)
        {
            if (arePasswordsHidden == false)
            {
                int index = passwordGrid.SelectedIndex;
                if (filter.Text == "")
                {
                    db.updateDB(passwordList[index].name, passwordList[index].login, passwordList[index].password, passwordList[index].id);
                }
                else
                {
                    db.updateDB(filteredList[index].name, filteredList[index].login, filteredList[index].password, filteredList[index].id);
                    passwordList = db.result;
                }
            }
            else
            {
                var dialog = new MessageDialog("Aby edytować element, wyłącz maskowanie haseł");
                await dialog.ShowAsync();
            }
        }

        public void hidePasswords()
        {
            getDB();
            foreach (var item in passwordList)
            {
                item.password = "*****";
                if (filter.Text == "")
                {
                    passwordGrid.ItemsSource = passwordList;
                }
                else
                {
                    filterAccounts();
                }
            }
            arePasswordsHidden = true;
        }

        public void unhidePasswords()
        {
            getDB();
            passwordGrid.ItemsSource = passwordList;
            if (filter.Text != "")
            {
                filterAccounts();
            }
            arePasswordsHidden = false;
        }

        public void filterAccounts()
        {
            filteredList = new ObservableCollection<Account>();
            if (filter.Text != "")
            {
                foreach (var item in passwordList)
                {
                    if (item.name.Contains(filter.Text))
                    {
                        filteredList.Add(item);
                    }
                }
                passwordGrid.ItemsSource = filteredList;
            }
            else
            {
                passwordGrid.ItemsSource = passwordList;
            }
        }

        private void filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            filterAccounts();   
        }

        private void HidePasswords_Checked(object sender, RoutedEventArgs e)
        {
            hidePasswords();
        }

        private void HidePasswords_Unchecked(object sender, RoutedEventArgs e)
        {
            unhidePasswords();
        }
    }
}
