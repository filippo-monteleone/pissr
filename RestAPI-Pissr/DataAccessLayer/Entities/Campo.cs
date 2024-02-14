using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Entities
{
    public record Campo
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public ushort Ettari { get; set; } = 0;
        public ushort Altitudine { get; set; } = 0;
        public bool Attivo { get; set; } = true;
        public bool Adattivo { get; set; } = false;

        public string TipoSistemaIrrigazione { get; set; } = String.Empty;
        public int Ae { get; set; } = 0;
        public float Fr { get; set; } = 0;
        public float S1 { get; set; } = 0;
        public float S2 { get; set; } = 0;

        public int Aws { get; set; } = 0;

        public double Rd { get; set; } = 0;
        public float Ac { get; set; } = 0;
        public double Hum { get; set; } = 0;
        public double Kc { get; set; } = 0;
        public double Sws => Rd * Aws;
        public double Mad => Sws * Ac;
        public double? SoilMoisture { get; set; } = null;


        public int AziendaAgricolaId { get; set; }
        public virtual AziendaAgricola AziendaAgricola { get; set; }

        public virtual ICollection<Dispositivo> Dispositivi { get; set; }
        public virtual ICollection<Evento> Eventi { get; set; }
    }
}
