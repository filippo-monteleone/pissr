using System.Diagnostics;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Mapper;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWork;
using System.Diagnostics.Contracts;

namespace BusinessLogicLayer.Services;

public class AziendaAgricolaService : IDataService<AziendaAgricolaDTO>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly AutoMapper.Mapper mapper;

    public AziendaAgricolaService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        mapper = MapperConfig.InitializeAutomapper();
    }

    public async Task<IEnumerable<AziendaAgricolaDTO>> GetList()
    {
        var aziende = await _unitOfWork.AziendeAgricole.GetAll();

        return mapper.Map<IEnumerable<AziendaAgricola>, IEnumerable<AziendaAgricolaDTO>>(aziende);
    }

    public async Task<IEnumerable<CampoDTO>> GetCampi(int id)
    {
        var campi = mapper.Map<IEnumerable<CampoDTO>>(await _unitOfWork.Campi.GetAll());
        return campi.Where(x => x.AziendaAgricolaId == id);
    }

    /*
     * Resituisco le richieste in attesa di una certa data e se ci sono alcune che
     * sono scadute vengono segnate come tali su db
     */
    private async Task CheckPendingRichieste(DateTime now)
    {
        var proposte = from contratto in await _unitOfWork.Contratti.GetAll()
            where contratto.Stato == ContrattoStato.Pending
            select contratto;

        Contratto propostaValida = null;

        foreach (var proposta in proposte)
        {
            if (proposta.Tempo <= now)
            {
                proposta.Stato = ContrattoStato.Expired;
            }
            else
            {
                propostaValida = proposta;
            }
        }

        _unitOfWork.Complete();

    }

    // Prendo la mia richiesta se esiste e controllo che non sia scaduta altrimenti la segno come tale sul db
    public async Task<ContrattoDTO?> GetRichiesta(int id, ContrattoStato stato, DateTime now)
    {
        await CheckPendingRichieste(now);

        var proposta = from contratto in await _unitOfWork.Contratti.GetAll()
            where contratto.Stato == stato && contratto.AziendaAgricolaId == id
            select mapper.Map<ContrattoDTO>(contratto);

        if (stato == ContrattoStato.Accepted)
            proposta = from contratto in proposta
                where (contratto.Fine > now && contratto.Tempo < now) || (contratto.Fine is null && contratto.Tempo < now)
                orderby contratto.Tempo
                select contratto;

        return proposta.FirstOrDefault();
    }

    public async Task<IEnumerable<ContrattoDTO>> GetRichieste(DateTime now)
    {
        await CheckPendingRichieste(now);

        var proposta = from contratto in await _unitOfWork.Contratti.GetAll()
            select mapper.Map<ContrattoDTO>(contratto);
        return proposta;
    }

    public async Task<AziendaAgricolaDTO?> GetById(int id)
    {
        var azienda = await _unitOfWork.AziendeAgricole.GetById(id);

        return mapper.Map<AziendaAgricolaDTO>(azienda);
    }

    public async Task<AziendaAgricolaDTO?> Insert(AziendaAgricolaDTO entity)
    {
        var nuovaAzienda = await _unitOfWork.AziendeAgricole.Insert(mapper.Map<AziendaAgricola>(entity));

        _unitOfWork.Complete();

        return mapper.Map<AziendaAgricolaDTO>(nuovaAzienda);
    }

    public async Task<ContrattoDTO?> Insert(ContrattoDTO entity)
    {
        var nuovoContratto = await _unitOfWork.Contratti.Insert(mapper.Map<Contratto>(entity));

        var contratti  = (await _unitOfWork.Contratti.GetAll()).Where(contratto =>
            contratto.Tempo == entity.Tempo && contratto.Stato == ContrattoStato.Accepted);

        if (contratti.Any(contratto =>
                contratto.Tempo == entity.Tempo && contratto.Stato == ContrattoStato.Accepted))
            return null;

        _unitOfWork.Complete();

        return mapper.Map<ContrattoDTO>(nuovoContratto);
    }

    public async Task<AziendaAgricolaDTO?> Update(AziendaAgricolaDTO entity)
    {
        var modificataAzienda = await _unitOfWork.AziendeAgricole.Update(mapper.Map<AziendaAgricola>(entity));

        _unitOfWork.Complete();

        return mapper.Map<AziendaAgricolaDTO>(modificataAzienda);
    }

    public async Task<AziendaAgricolaDTO> Delete(AziendaAgricolaDTO entity)
    {
        var eliminataAzienda = await _unitOfWork.AziendeAgricole.Delete(mapper.Map<AziendaAgricola>(entity));

        _unitOfWork.Complete();

        return mapper.Map<AziendaAgricolaDTO>(entity);
    }
}