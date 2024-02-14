using System.Collections;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.JavaScript;
using System.Xml.Schema;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Mapper;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWork;
using Microsoft.EntityFrameworkCore.Design.Internal;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DispositivoTipo = DataAccessLayer.Entities.DispositivoTipo;
using EventoTipo = DataAccessLayer.Entities.EventoTipo;

namespace BusinessLogicLayer.Services;

public class CampoService : IDataService<CampoDTO>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AutoMapper.Mapper _mapper;

    public CampoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = MapperConfig.InitializeAutomapper();
    }


    /*
     * Ottiene la temperatura minima e massima di un campo in una certa data
     */
    public async Task<(float? max, float? min)> GetTemp(int id, DateTime data)
    {
        var campo = await _unitOfWork.Campi.GetById(id);
        var eventicollection = campo.Eventi.Where(q => q.Tipo == EventoTipo.Temp);
        var eventi = campo.Eventi.Where(q => q.Tipo == EventoTipo.Temp
                                                                              && q.Tempo.ToString("d") == data.ToString("d"))
            .Select(q => float.Parse(q.Valore)).ToList()
            .Aggregate(new
            {
                TMin = float.MaxValue,
                TMax = float.MinValue
            }, (accumulator, q) => new
            {
                TMin = Math.Min(accumulator.TMin, q),
                TMax = Math.Max(accumulator.TMax, q)
            });

        (float?, float?) temp = (eventi.TMax, eventi.TMin);

        return (temp.Item1, temp.Item2);
    }


    /*
     * Ottiene quanta pioggia e' caduta in un certo campo in una certa data
     */
    public async Task<float> GetRain(int id, DateTime data)
    {

        var campo = await _unitOfWork.Campi.GetById(id);

        var rain = from evento in campo.Eventi
                   where evento.Tipo == EventoTipo.Rain && evento.Tempo.ToString("d") == data.ToString("d")
                   group evento by evento.Tempo
            into g
                   select g.Max(q => float.Parse(q.Valore));

        var totRain = rain.DefaultIfEmpty(0).Sum();

        return totRain < 5 ? 0 : (totRain - 5) * .75f;
    }

    /*
     * Ottiene la massima radiazione solare in un certo campo in una certa data
     */
    public async Task<float> GetMaxRad(int id, DateTime data)
    {
        var campo = await _unitOfWork.Campi.GetById(id);

        var rad = from evento in campo.Eventi
                  where evento.Tipo == EventoTipo.Rad && evento.Tempo.ToString("d") == data.ToString("d")
                  select float.Parse(evento.Valore); ;

        return rad.DefaultIfEmpty(0).Max();
    }

    /*
     * Ottiene lo status dei dispositivi di un campo in una certa data.
     * Ritorna una lista con id dei dispositivi e il loro l'ultimo evento
     */
    public async Task<List<(int id, (EventoTipo, DateTime))>> GetStatusDispositivi(int id, DateTime data)
    {
        var campo = await _unitOfWork.Campi.GetById(id);
        IEnumerable<Evento> futureEvents = new List<Evento>();
        var temp = campo.Eventi.ToList();

        for (var i = 0; i < temp.Count; i++)
        {
            if (temp[i].Tempo > data)
            {
                futureEvents=futureEvents.Append(temp[i]);
            }
        }

        //var futureEvents = from evnt in campo.Eventi where evnt.Tempo > data select evnt;
        var currentEvents = (from evnt in campo.Eventi 
            where !futureEvents.Contains(evnt) 
                && (evnt.Tipo == EventoTipo.On || evnt.Tipo == EventoTipo.Off || evnt.Tipo == EventoTipo.Idle)
                orderby evnt.Tempo descending
            select (evnt.DispositivoId, (evnt.Tipo, evnt.Tempo))).ToList();

        for (int i = 0; i < currentEvents.Count(); i++)
        {
            var evnt = currentEvents[i].DispositivoId;
            for (int j = i+1; j < currentEvents.Count(); j++)
            {
                if (currentEvents[j].DispositivoId == evnt)
                    currentEvents.RemoveAt(j);
            }

        }

        return currentEvents;
    }


    /*
     * Ottiene il totale di acqua usata sul un campo
     */
    public async Task<float> GetIrriguoNetto(int id, DateTime data, double? ActualSoilMoisture)
    {
        var campo = await _unitOfWork.Campi.GetById(id);

        if (!campo.Adattivo)
        {
            if (ActualSoilMoisture < campo.Hum)
            {
                double um = campo.Hum - (ActualSoilMoisture ?? 0);
                return (campo.S1 * campo.S2 * (float) um * 100) / (campo.Fr * campo.Ae);
            }

            return 0;
        }

        return (campo.S1 * campo.S2 * (float)campo.Mad * 100) / (campo.Fr * campo.Ae);
    }

    /*
     * Acqua realmente usata su un campo in una certa data
     */
    public async Task<double> GetAcquaConsumata(int id, DateTime data)
    {

        // Vado Prima a controllare gli eventi del giorno precedente e prendo l'ultimo stato per ogni disp
        var campo = await _unitOfWork.Campi.GetById(id);
        var evntPrima = await GetStatusDispositivi(id, data.AddDays(-1));
        List<double> test = new();

        for (int i = 0; i < campo.Dispositivi.Count; i++)
        {
            var disp = campo.Dispositivi.ElementAt(i);

            var ultimoEvento = evntPrima.FirstOrDefault((x) => x.id == disp.Id, defaultValue: (id: disp.Id, (EventoTipo.Off, data)));
            // Se nel giorno precedente non c'erano eventi o se era off setto il nessunEventoPrima a true
            bool attuatoreAccesoDaIeri = ultimoEvento.Item2.Item1 == EventoTipo.On;

            var eventiOggi = (from evnt in campo.Eventi
                where (evnt.Tipo is EventoTipo.On or EventoTipo.Off) && evnt.DispositivoId == disp.Id &&
                      new DateTime(data.Year, data.Month, data.Day) <= evnt.Tempo
                orderby evnt.Tempo ascending
                select evnt).ToList();

            var queueEventi = new Queue<Evento>();

            if (attuatoreAccesoDaIeri)
            {


                if (eventiOggi.Count == 0)
                {
                    return (data - ultimoEvento.Item2.Item2).TotalHours * campo.Fr;
                }

                queueEventi.Enqueue(new Evento() { Tipo = EventoTipo.On, Tempo = ultimoEvento.Item2.Item2 });
            }

            Evento? tempEvento = null;

            for (int j = 0; j < eventiOggi.Count; j++)
            {

                if (queueEventi.Count > 0 && tempEvento?.Tipo != eventiOggi[j].Tipo)
                {
                    tempEvento = eventiOggi[j];
                    queueEventi.Enqueue(eventiOggi[j]);
                } else if (queueEventi.Count == 0)
                {
                    tempEvento = eventiOggi[j];
                    queueEventi.Enqueue(eventiOggi[j]);
                }

                if (eventiOggi.Count - 1 == j && eventiOggi[j].Tipo == EventoTipo.On)
                {
                    queueEventi.Enqueue(new Evento() { Tipo = EventoTipo.Off, Tempo = data});
                }
            }

            while (queueEventi.Count > 0)
            {

                var evento = queueEventi.Dequeue();

                while (queueEventi.Peek().Tipo != EventoTipo.Off)
                {
                    queueEventi.Dequeue();
                }

                var eventoDopo = queueEventi.Dequeue();
                var timespan = eventoDopo.Tempo - evento.Tempo;

                test.Add(timespan.TotalHours * campo.Fr);
            }

        }

        return test.Aggregate(0d, (sum, value) => sum += value);
    }

    // Calcolo Etc del campo in una certa data
    public async Task<float> GetEtc(int id, DateTime data)
    {
        var (tmin, tmax) = await GetTemp(id, data);

        if (tmin == null || tmax == null)
            return 0f;

        var rad = await GetMaxRad(id, data);

        var campo = await _unitOfWork.Campi.GetById(id);

        var tAvg = (float)(tmax + tmin) / 2;
        var atmPressure = 101.3 * Math.Pow(((293 - 0.0065 * campo.Altitudine) / 293), 5.26);
        var gamma = 0.00065 * atmPressure;
        var delta = 4098 * Math.Pow((0.6108 * Math.Exp(17.27 * tAvg / (tAvg + 237.3))) / (tAvg + 237.3), 2);
        var pet = 0.5 * delta / (delta + gamma) * rad;

        return (float)(pet * campo.Kc);
    }

    // Calcolo l'umidita' rimasta
    public async Task<double> UmiditaRimasta(int id, DateTime data)
    {
        var acquaConsumata = await GetAcquaConsumata(id, data);
        var pioggia = await GetRain(id, data);
        var acquaPersa = await GetEtc(id, data);

        var campo = await _unitOfWork.Campi.GetById(id);

        if (campo.SoilMoisture is null)
            campo.SoilMoisture = campo.Sws;
        campo.SoilMoisture += (acquaConsumata + pioggia) - acquaPersa;

        _unitOfWork.Complete();

        return campo.SoilMoisture ?? 0;
    }

    public async Task<IEnumerable<CampoDTO>> GetList()
    {
        var campi = await _unitOfWork.Campi.GetAll();
        return _mapper.Map<IEnumerable<CampoDTO>>(campi);
    }

    public async Task<CampoDTO?> GetById(int id)
    {
        var campo = await _unitOfWork.Campi.GetById(id);

        return _mapper.Map<CampoDTO>(campo);
    }

    public async Task<IEnumerable<DispositivoDTO>> GetDispositivi(int id)
    {
        var dispositivi = (await _unitOfWork.Campi.GetById(id))?.Dispositivi;

        return _mapper.Map<IEnumerable<DispositivoDTO>>(dispositivi);
    }

    public async Task<DispositivoDTO> GetDispositivo(int id)
    {
        var dispositivo = await _unitOfWork.Dispositivi.GetById(id);

        return _mapper.Map<DispositivoDTO>(dispositivo);
    }

    public async Task<DispositivoDTO?> Insert(DispositivoDTO entity)
    {
        if (await _unitOfWork.Campi.GetById(entity.CampoId) == null)
        {
            return null;
        }

        var dispositivo = await _unitOfWork.Dispositivi.Insert(_mapper.Map<Dispositivo>(entity));

        _unitOfWork.Complete();

        return _mapper.Map<DispositivoDTO>(dispositivo);
    }

    public async Task<CampoDTO?> Insert(CampoDTO entity)
    {

        if (await _unitOfWork.AziendeAgricole.GetById(entity.AziendaAgricolaId) == null)
        {
            return null;
        }

        var campo = await _unitOfWork.Campi.Insert(_mapper.Map<Campo>(entity));

        _unitOfWork.Complete();

        return _mapper.Map<CampoDTO>(campo);
    }

    public async Task<CampoDTO?> Update(CampoDTO entity)
    {
        var campo = await _unitOfWork.Campi.GetById(entity.Id);

        if (campo == null)
            return null;

        entity.AziendaAgricolaId = campo.AziendaAgricolaId;

        _mapper.Map(entity, campo);

        _unitOfWork.Complete();

        return _mapper.Map<CampoDTO>(campo);
    }

    public async Task<DispositivoDTO?> Update(DispositivoDTO entity)
    {

        if (await _unitOfWork.Dispositivi.GetById(entity.Id) == null)
            return null;

        var dispositivo = await _unitOfWork.Dispositivi.Update(_mapper.Map<Dispositivo>(entity));

        _unitOfWork.Complete();

        return _mapper.Map<DispositivoDTO>(dispositivo);
    }

    public async Task<CampoDTO?> Delete(CampoDTO entity)
    {

        var campo = await _unitOfWork.Campi.Delete(_mapper.Map<Campo>(entity));

        try
        {
            _unitOfWork.Complete();
        }
        catch (Exception ex)
        {
            return null;
        }


        return _mapper.Map<CampoDTO>(campo);
    }

    public async Task<DispositivoDTO?> Delete(DispositivoDTO entity)
    {
        var dispositivo = await _unitOfWork.Dispositivi.Delete(_mapper.Map<Dispositivo>(entity));

        try
        {
            _unitOfWork.Complete();
        }
        catch (Exception ex)
        {
            return null;
        }

        return _mapper.Map<DispositivoDTO>(dispositivo);
    }
}