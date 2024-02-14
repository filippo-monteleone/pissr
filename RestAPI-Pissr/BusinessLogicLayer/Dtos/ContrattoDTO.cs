using DataAccessLayer.Entities;

namespace BusinessLogicLayer.Dtos;

public class ContrattoDTO
{
    public int Id { get; set; }
    public DateTime Tempo { get; set; }
    public float Acqua { get; set; }
    public DateTime? Fine { get; set; }

    public ContrattoStato Stato { get; set; }

    public int AziendaAgricolaId { get; set; }

    public int AziendaIdricaId { get; set; }
}