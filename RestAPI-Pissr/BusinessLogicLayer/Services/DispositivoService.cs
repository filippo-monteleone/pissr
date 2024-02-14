using System.Runtime.InteropServices;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Mapper;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWork;
using DispositivoTipo = DataAccessLayer.Entities.DispositivoTipo;
using EventoTipo = BusinessLogicLayer.Dtos.EventoTipo;

namespace BusinessLogicLayer.Services;

public class DispositivoService : IDataService<DispositivoDTO>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AutoMapper.Mapper _mapper;

    public DispositivoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = MapperConfig.InitializeAutomapper();
    }

    public async Task<IEnumerable<DispositivoDTO>> GetList()
    {
        var dispositivi = await _unitOfWork.Dispositivi.GetAll();
        return _mapper.Map<IEnumerable<DispositivoDTO>>(dispositivi);
    }

    public async Task<DispositivoDTO?> GetById(int id)
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

    public async Task<EventoDto?> Insert(EventoDto entity)
    {
        var dispositivo = await _unitOfWork.Dispositivi.GetById(entity.DispositivoId);

        if (dispositivo == null)
            return null;

        switch (dispositivo.Tipo)
        {
            case DispositivoTipo.Attuatore when (int)entity.Tipo is < 4:
                return null;
        }

        var nuovoEvento = await _unitOfWork.Eventi.Insert(_mapper.Map<Evento>(entity));

        _unitOfWork.Complete();

        return _mapper.Map<EventoDto>(nuovoEvento);
    }

    public async Task<EventoDto?> Update(EventoDto entity)
    {
        var dispositivo = await _unitOfWork.Dispositivi.GetById(entity.DispositivoId);

        if (dispositivo == null)
            return null;

        switch (dispositivo.Tipo)
        {
            case DispositivoTipo.Attuatore when (int)entity.Tipo is < 4:
                return null;
        }

        var eventoModificato = await _unitOfWork.Eventi.Update(_mapper.Map<Evento>(entity));

        try
        {
            _unitOfWork.Complete();
        }
        catch (Exception ex)
        {
            return null;
        }

        return _mapper.Map<EventoDto>(eventoModificato);
    }

    public async Task<IEnumerable<EventoDto>?> GetEventi(int id)
    {
        var eventi = (await _unitOfWork.Eventi.GetAll()).Where(x => x.DispositivoId == id);
        return _mapper.Map<IEnumerable<EventoDto>>(eventi);
    }

    public async Task<EventoDto?> GetEvento(int id, int eventoId)
    {
        var evento = (await _unitOfWork.Eventi.GetAll()).FirstOrDefault(x => x.DispositivoId == id && x.Id == eventoId, null);
        return _mapper.Map<EventoDto>(evento);
    }

    public Task<DispositivoDTO?> Update(DispositivoDTO entity)
    {
        throw new NotImplementedException();
    }

    public async Task<DispositivoDTO?> Delete(DispositivoDTO entity)
    {
        if (await _unitOfWork.Dispositivi.GetById(entity.Id) == null)
            return null;

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