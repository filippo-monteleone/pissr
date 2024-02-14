using DataAccessLayer.Entities;

namespace RestAPI_Pissr.Models
{
    public record AddContratto
    {
        public float Acqua { get; set; }
        public int AziendaIdricaId { get; set; }
    }

    public record UpdateContratto
    {
        public float Acqua { get; set; }
        public ContrattoStato Stato { get; set; }
    }
}
