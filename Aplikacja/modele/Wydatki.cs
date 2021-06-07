using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aplikacja.models
{
    class Wydatki
    {
        public int id { get; set; }
        public string kwota { get; set; }
        public string opis { get; set; }
        public DateTime data { get; set; }
        public string zdjecie_paragonu { get; set; }
        public int uzytkownicy_id { get; set; }
        public string typ_wydatku { get; set; }
    }
}
