using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Entities
{
    public enum DispositivoTipo
    {
        Sensore,
        Attuatore
    }

    public record Dispositivo
    {
        public int Id { get; set; }
        public DispositivoTipo Tipo { get; set; }
        public int CampoId { get; set; }
        public virtual Campo Campo { get; set; }
    }
}
