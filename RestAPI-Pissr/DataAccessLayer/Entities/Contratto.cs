using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{

    public enum ContrattoStato
    {
        Accepted,
        Refused,
        Pending,
        Expired
    }

    public record Contratto
    {
        public int Id { get; set; }
        public DateTime Tempo { get; set; }
        public float Acqua { get; set; }
        public DateTime? Fine { get; set; } = null;


        public ContrattoStato Stato { get; set; }


        public int AziendaAgricolaId { get; set; }
        public virtual AziendaAgricola AziendaAgricola { get; set; }

        public int AziendaIdricaId { get; set; }
        public virtual AziendaIdrica AziendaIdrica { get; set; }
    }
}
