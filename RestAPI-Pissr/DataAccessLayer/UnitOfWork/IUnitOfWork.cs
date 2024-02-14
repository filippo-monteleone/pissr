using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;

namespace DataAccessLayer.UnitOfWork;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<AziendaAgricola> AziendeAgricole { get; }
    IGenericRepository<AziendaIdrica> AziendeIdriche { get; }
    IGenericRepository<Campo> Campi { get; }
    IGenericRepository<Dispositivo> Dispositivi { get; }
    IGenericRepository<Evento> Eventi { get; }
    IGenericRepository<Contratto> Contratti { get;  }

    int Complete();
    void Clear();
}