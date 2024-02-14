using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public enum EventoTipo
    {
        Temp,
        Rad,
        Hum,
        Rain,
        On,
        Off,
        Idle,
    }

    public record Evento
    {
        public int Id { get; set; }
        public EventoTipo Tipo { get; set; }
        public DateTime Tempo { get; set; }
        public string? Valore { get; set; }
        public int DispositivoId { get; set; }
        public virtual Dispositivo Dispositivo { get; set; }
        public int CampoId { get; set; }
        public virtual Campo Campo { get; set; }
    }
}
