using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Aplikacja.modele
{
    public class Uzytkownicy
    {
        public string email { get; set; }
        public string imie { get; set; }
        public string haslo { get; set; }
    }
}
