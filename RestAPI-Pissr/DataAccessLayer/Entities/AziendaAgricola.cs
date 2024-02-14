using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public record AziendaAgricola
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Indirizzo { get; set; }
        public virtual ICollection<Campo> Campi { get; set; }
    }
}
