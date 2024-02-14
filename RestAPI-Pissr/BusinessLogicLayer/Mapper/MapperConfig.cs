using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BusinessLogicLayer.Dtos;
using DataAccessLayer.Entities;

namespace BusinessLogicLayer.Mapper
{
    public class MapperConfig
    {
        public static AutoMapper.Mapper InitializeAutomapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AziendaAgricolaDTO, AziendaAgricola>();
                cfg.CreateMap<AziendaAgricola, AziendaAgricolaDTO>();
                cfg.CreateMap<AziendaIdricaDTO, AziendaIdrica>();
                cfg.CreateMap<AziendaIdrica, AziendaIdricaDTO>();
                cfg.CreateMap<CampoDTO, Campo>();
                cfg.CreateMap<Campo, CampoDTO>();
                cfg.CreateMap<DispositivoDTO, Dispositivo>();
                cfg.CreateMap<Dispositivo, DispositivoDTO>();
                cfg.CreateMap<EventoDto, Evento>();
                cfg.CreateMap<Evento, EventoDto>();
                cfg.CreateMap<ContrattoDTO, Contratto>();
                cfg.CreateMap<Contratto, ContrattoDTO>();
            });

            var mapper = new AutoMapper.Mapper(config);
            return mapper;
        }
    }
}
