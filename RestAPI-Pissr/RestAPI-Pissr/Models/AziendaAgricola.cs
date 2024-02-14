namespace RestAPI_Pissr.Models
{
    public record AddAziendaAgricolaRequest
    {
        public string Nome { get; set; }
        public string Indirizzo { get; set; }
    }

    public class UpdateAziendaAgricolaRequest
    {
        public string Nome { get; set; }
        public string Indirizzo { get; set; }
    }
}
