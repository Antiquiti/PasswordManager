using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Popups;

namespace Menadzer_hasel
{
    class Database
    {
        public SQLite.SQLiteConnection conn;
        public ObservableCollection<Account> result { get; set; }
        public void initializeDB(string dbName)
        {
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, dbName);
            conn = new SQLite.SQLiteConnection(path, true);
            conn.CreateTable<Account>();
        }


        public async void addToDB(string name, string login, string password)
        {
            initializeDB("passwords.db");
            var acc = new Account
            {
                name = name,
                login = login,
                password = password
            };
            conn.Insert(acc);
            var dialog = new MessageDialog("Konto zostało pomyślnie dodane");
            await dialog.ShowAsync();
            
        }

        public void getDB()
        {
            initializeDB("passwords.db");
            result = new ObservableCollection<Account>();
            var query = conn.Table<Account>();
            List<Account> list = query.ToList();
            foreach (var item in list)
            {
                result.Add(item);
            }       
        }

        public void removeFromDB(int index)
        {
            initializeDB("passwords.db");
            var query = conn.Table<Account>();
            conn.Execute("DELETE FROM Account WHERE Id = ?", index);
            getDB();
        }

        public void updateDB(string name, string login, string password, int index)
        {
            initializeDB("passwords.db");
            var query = conn.Table<Account>();
            conn.Execute("UPDATE Account SET name = ?, login = ?, password = ? WHERE Id = ?", name, login, password, index);
            getDB();
        }
    }
}
