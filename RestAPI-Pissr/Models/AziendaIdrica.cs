using System.ComponentModel.DataAnnotations.Schema;

namespace Data_Access_Layer.Entities
{
    public record AddAziendaIdricaRequest
    {
        public string Nome { get; set; }
        public string Indirizzo { get; set; }
    }

    public record UpdateAziendaIdricaRequest
    {
        public string Nome { get; set; }
        public string Indirizzo { get; set; }
    }
}
