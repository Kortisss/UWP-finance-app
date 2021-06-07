using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Data.Sqlite;
using Windows.Storage;
using System.IO;
using System.Collections.ObjectModel;

namespace DataAccessLibrary
{
    public static class DataAccess
    {
        public async static void InitializeDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("appDatabase.db", CreationCollisionOption.OpenIfExists);
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "appDatabase.db");
            using (SqliteConnection db =
               new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                string tableUzytkowicy = "CREATE TABLE IF NOT EXISTS \"uzytkownicy\" ( `uzytkownicy_id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, `email` TEXT NOT NULL UNIQUE, `imie` TEXT NOT NULL, `haslo` TEXT NOT NULL )";
                string tableWydatki = "CREATE TABLE IF NOT EXISTS \"wydatki\" ( `id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, `kwota` FLOAT NOT NULL, `opis` TEXT, `data` TEXT NOT NULL, `zdjecie_paragonu` BLOB UNIQUE, `uzytkownicy_id` INTEGER, `typWydatku` TEXT, FOREIGN KEY(`uzytkownicy_id`) REFERENCES `uzytkownicy`(`uzytkownicy_id`))";

                SqliteCommand createTable1 = new SqliteCommand(tableUzytkowicy, db);
                SqliteCommand createTable2 = new SqliteCommand(tableWydatki, db);

                createTable1.ExecuteReader();
                createTable2.ExecuteReader();
            }
        }
        public static string sprawdzUzytkownika(string email)
        {

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "appDatabase.db");
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("select uzytkownicy_id from uzytkownicy where email=@email", db);

                selectCommand.Parameters.AddWithValue("@email", email);
                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    return query.GetString(0);
                    
                }

                db.Close();
            }
                return "Blad";
            
        }
        public static void updateHasloUzytkownika(string haslo, int uzytkownicy_id)
        {

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "appDatabase.db");
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand updateCommand = new SqliteCommand
                    ("UPDATE uzytkownicy set haslo = @haslo where uzytkownicy_id=@uzytkownicy_id", db);

                updateCommand.Parameters.AddWithValue("@haslo", haslo);
                updateCommand.Parameters.AddWithValue("@uzytkownicy_id", uzytkownicy_id);

                updateCommand.ExecuteReader();

                db.Close();
            }
        }
        public static void updateEmailUzytkownika(string email, int uzytkownicy_id)
        {

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "appDatabase.db");
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand updateCommand = new SqliteCommand
                    ("UPDATE uzytkownicy set email = @email where uzytkownicy_id=@uzytkownicy_id", db);

                updateCommand.Parameters.AddWithValue("@email", email);
                updateCommand.Parameters.AddWithValue("@uzytkownicy_id", uzytkownicy_id);

                updateCommand.ExecuteReader();

                db.Close();
            }
        }
        public static ObservableCollection<String> GetData(string email)
        {
            ObservableCollection<String> entries = new ObservableCollection<string>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "appDatabase.db");
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("select id, kwota, opis, data, typWydatku from wydatki, uzytkownicy where uzytkownicy.uzytkownicy_id = wydatki.uzytkownicy_id and uzytkownicy.email= @email", db);

                selectCommand.Parameters.AddWithValue("@email", email);
                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(query["id"].ToString() + " " + query["kwota"].ToString() + " " + query["data"].ToString() + " " + query["typWydatku"].ToString() + " " + query["opis"].ToString());
                }

                db.Close();
            }

            return entries;
        }
        public static List<string> GetDataForDiagramsKwota(string email)
        {
            List<String> listaKwota = new List<string>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "appDatabase.db");
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("select sum(kwota) as kwota, typWydatku from wydatki, uzytkownicy where uzytkownicy.uzytkownicy_id = wydatki.uzytkownicy_id and uzytkownicy.email=@email group by typWydatku", db);

                selectCommand.Parameters.AddWithValue("@email", email);
                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {

                    listaKwota.Add(query["kwota"].ToString());
                }
                db.Close();
            }
            return listaKwota;
        }
        public static List<string> GetDataForDiagramsTyp(string email)
        {
            List<String> listaTyp = new List<string>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "appDatabase.db");
            using (SqliteConnection db = new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("select sum(kwota) as kwota, typWydatku from wydatki, uzytkownicy where uzytkownicy.uzytkownicy_id = wydatki.uzytkownicy_id and uzytkownicy.email=@email group by typWydatku", db);

                selectCommand.Parameters.AddWithValue("@email", email);
                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    listaTyp.Add(query["typWydatku"].ToString());
                }
                db.Close();
            }

            return listaTyp;
        }
        public static void dodajUzytkownika(string email, string imie, string haslo)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "appDatabase.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand
                {
                    Connection = db,

                    // Use parameterized query to prevent SQL injection attacks
                    CommandText = "INSERT INTO uzytkownicy(email, imie, haslo) VALUES (@email, @imie, @haslo)"
                };
                insertCommand.Parameters.AddWithValue("@email", email);
                insertCommand.Parameters.AddWithValue("@imie", imie);
                insertCommand.Parameters.AddWithValue("@haslo", haslo);

                insertCommand.ExecuteReader();

                db.Close();
            }
        }
        public static void dodajWpisUzytkownika(double kwota, string opis, string data, int uzytkownicy_id, string typWydatku)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "appDatabase.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand
                {
                    Connection = db,

                    // Use parameterized query to prevent SQL injection attacks
                    CommandText = "INSERT INTO wydatki(kwota, opis, data, uzytkownicy_id, typWydatku) VALUES (@kwota, @opis, @data, @uzytkownicy_id, @typWydatku)"
                };
                insertCommand.Parameters.AddWithValue("@kwota", kwota);
                insertCommand.Parameters.AddWithValue("@opis", opis);
                insertCommand.Parameters.AddWithValue("@data", data);
                insertCommand.Parameters.AddWithValue("@uzytkownicy_id", uzytkownicy_id);
                insertCommand.Parameters.AddWithValue("@typWydatku", typWydatku);
                //Potrzeba jeszcze dodawania zdjecia
                insertCommand.ExecuteReader();

                db.Close();
            }
        }
        public static void usunWpisUzytkownika(int id)
        {
            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "appDatabase.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand deleteCommand = new SqliteCommand
                {
                    Connection = db,
                    CommandText = "DELETE FROM wydatki where id = @id"
                };
                deleteCommand.Parameters.AddWithValue("@id", id);
                deleteCommand.ExecuteReader();
                db.Close();
            }


        }
    public static List<String> checkUser(string email, string haslo)
        {
            List<String> entries = new List<string>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "appDatabase.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                   ($"select exists( select email, haslo from uzytkownicy where email = \"{email}\" and haslo = \"{haslo}\" limit 1) ", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                   entries.Add(query.GetString(0));
                }
                db.Close();
            }
            return entries;
        }
        public static List<String> checkEmail(string email)
        {
            List<String> entries = new List<string>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "appDatabase.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                   ($"select exists( select email, haslo from uzytkownicy where email = \"{email}\" limit 1) ", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(query.GetString(0));
                }
                db.Close();
            }
            return entries;
        }
        public static List<String> checkPassword(string email)
        {
            List<String> entries = new List<string>();

            string dbpath = Path.Combine(ApplicationData.Current.LocalFolder.Path, "appDatabase.db");
            using (SqliteConnection db =
              new SqliteConnection($"Filename={dbpath}"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                   ($"select exists( select email, haslo from uzytkownicy where email = \"{email}\" limit 1) ", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(query.GetString(0));
                }
                db.Close();
            }
            return entries;
        }
    }
}
