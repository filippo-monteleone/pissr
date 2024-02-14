using System.Runtime.InteropServices.JavaScript;
using BusinessLogicLayer.Dtos;
using BusinessLogicLayer.Services;
using DataAccessLayer.Entities;
using DataAccessLayer.Repositories;
using DataAccessLayer.UnitOfWork;
using Moq;
using static System.Runtime.InteropServices.JavaScript.JSType;
using DispositivoTipo = DataAccessLayer.Entities.DispositivoTipo;
using EventoTipo = DataAccessLayer.Entities.EventoTipo;

namespace UnitTestBLL
{
    public class CampoServiceTest
    {
        private readonly CampoService _sut;
        private readonly Mock<IUnitOfWork> _campoRepoMock = new Mock<IUnitOfWork>();

        public CampoServiceTest()
        {
            _sut = new CampoService(_campoRepoMock.Object);
        }

        public static TheoryData<List<Evento>, DateTime, (float?, float?)> tempData => new()
        {
            {
                new List<Evento>
                {
                    new Evento() { Tipo = EventoTipo.Temp, Tempo = DateTime.Now, Valore = "10" },
                },
                DateTime.Now,
                (10f, 10f)
            },
            {
                new List<Evento>
                {
                    new Evento() { Tipo = EventoTipo.Temp, Tempo = DateTime.Now, Valore = "10" },
                    new Evento() { Tipo = EventoTipo.Temp, Tempo = DateTime.Now, Valore = "5" },
                    new Evento() { Tipo = EventoTipo.Temp, Tempo = DateTime.Now, Valore = "15" }
                },
                DateTime.Now,
                (15f, 5f)
            },
            {
                new List<Evento>
                {
                    new Evento() { Tipo = EventoTipo.Temp, Tempo = DateTime.Now, Valore = "15" },
                    new Evento() { Tipo = EventoTipo.Temp, Tempo = DateTime.Now, Valore = "15" }
                },
                DateTime.Now,
                (15f, 15f)
            }
        };

        public static TheoryData<List<Evento>, DateTime, float> rainData => new()
        {
            {
                new List<Evento>
                {
                    new Evento() {Tipo = EventoTipo.Rain, Tempo = new DateTime(2000, 11, 1, 0, 0, 1), Valore = "5"},
                    new Evento() {Tipo = EventoTipo.Rain, Tempo = new DateTime(2000, 11, 1, 0, 0, 2), Valore = "10"}
                },
                new DateTime(2000, 11, 1),
                7.5f
            },
            {
                new List<Evento>
                {
                    new Evento() {Tipo = EventoTipo.Rain, Tempo = new DateTime(2000, 11, 1, 0, 0, 1), Valore = "5"},
                    new Evento() {Tipo = EventoTipo.Rain, Tempo = new DateTime(2000, 11, 1, 0, 0, 1), Valore = "10"}
                },
                new DateTime(2000, 11, 1),
                3.75f
            },
            {
                new List<Evento>
                {
                    new Evento() {Tipo = EventoTipo.Rain, Tempo = new DateTime(2000, 11, 1, 0, 0, 1), Valore = "5"},
                    new Evento() {Tipo = EventoTipo.Rain, Tempo = new DateTime(2000, 10, 31, 23, 0, 0), Valore = "10"}
                },
                new DateTime(2000, 11, 1),
                0f
            }
        };

        public static TheoryData<List<Evento>, DateTime, float> radData => new()
        {
            {
                new List<Evento>
                {
                    new Evento() {Tipo = EventoTipo.Rad, Tempo = DateTime.Now, Valore = "5"},
                    new Evento() {Tipo = EventoTipo.Rad, Tempo = DateTime.Now, Valore = "10"}
                },
                DateTime.Now,
                10f
            },
            {
                new List<Evento>
                {
                    new Evento() {Tipo = EventoTipo.Rad, Tempo = DateTime.Now, Valore = "5"},
                    new Evento() {Tipo = EventoTipo.Rad, Tempo = DateTime.Now, Valore = "10"}
                },
                DateTime.Now.AddDays(-1),
                0f
            },
            {
                new List<Evento>
                {
                    new Evento() {Tipo = EventoTipo.Rad, Tempo = DateTime.Now, Valore = "5"},
                    new Evento() {Tipo = EventoTipo.Rad, Tempo = DateTime.Now, Valore = "5"}
                },
                DateTime.Now,
                5f
            },
            {
                new List<Evento>
                {

                },
                DateTime.Now,
                0f
            }
        };

        public static TheoryData<List<Evento>, List<Dispositivo>, DateTime, List<(int, (EventoTipo, DateTime))>> statusDispositivi => new()
        {
            {
                new List<Evento>()
                {
                    new Evento() {DispositivoId = 1, Tipo = EventoTipo.On, Tempo = new DateTime(2000, 11, 1, 0, 0, 0)},
                    new Evento() {DispositivoId = 1, Tipo = EventoTipo.Hum, Tempo = new DateTime(2000, 11, 1, 1, 0, 0)},
                    new Evento() {DispositivoId = 2, Tipo = EventoTipo.Off, Tempo = new DateTime(2000, 11, 1)},
                    new Evento() {DispositivoId = 2, Tipo = EventoTipo.On, Tempo = new DateTime(2000, 10, 31)},
                    new Evento() {DispositivoId = 3, Tipo = EventoTipo.Off, Tempo = new DateTime(2000, 11, 1)},
                },
                new List<Dispositivo>
                {
                    new Dispositivo() {Id = 1, Tipo = DispositivoTipo.Attuatore},
                    new Dispositivo() {Id = 2, Tipo = DispositivoTipo.Attuatore},
                    new Dispositivo() {Id = 3, Tipo = DispositivoTipo.Attuatore}
                },
                new DateTime(2000, 11, 2, 0, 0, 0),
                new List<(int id, (EventoTipo, DateTime))>
                {
                    (1, (EventoTipo.On, new DateTime(2000, 11, 1, 0, 0, 0))),
                    (2, (EventoTipo.Off, new DateTime(2000, 11, 1))),
                    (3, (EventoTipo.Off, new DateTime(2000, 11, 1)))
                }
            },
        };

        public static TheoryData<List<Evento>, List<Dispositivo>, List<DateTime>, float> HumRimasta => new()
        {
            {
                new List<Evento>(),
                new List<Dispositivo>(),
                new List<DateTime>() { DateTime.Now },
                70f
            },
            {
                new List<Evento>()
                {
                    new Evento() {Id = 1, Tipo = EventoTipo.Temp, Valore = "25", Tempo = DateTime.Now},
                    new Evento() {Id = 2, Tipo = EventoTipo.Rad, Valore = "15", Tempo = DateTime.Now}
                },
                new List<Dispositivo>(),
                new List<DateTime>() {DateTime.Now},
                64.5f
            },
            {
                new List<Evento>()
                {
                    new Evento() {Id = 1, Tipo = EventoTipo.Temp, Valore = "25", Tempo = DateTime.Now},
                    new Evento() {Id = 2, Tipo = EventoTipo.Rad, Valore = "15", Tempo = DateTime.Now},
                    new Evento() {Id = 3, Tipo = EventoTipo.Rain, Valore = "10", Tempo = DateTime.Now}
                },
                new List<Dispositivo>(),
                new List<DateTime>() { DateTime.Now },
                68.3f
            },
            {
                new List<Evento>()
                {
                    new Evento() {Tipo = EventoTipo.Temp, Valore = "25", Tempo = new DateTime(2000, 11, 26, 15, 0, 0)},
                    new Evento() {Tipo = EventoTipo.Rad, Valore = "15", Tempo = new DateTime(2000, 11, 26, 15, 0, 0)},
                    new Evento() {Tipo = EventoTipo.Rain, Valore = "10", Tempo = new DateTime(2000, 11, 26, 15, 0, 0)},
                    new Evento() {Tipo = EventoTipo.On, Valore = "", Tempo = new DateTime(2000, 11, 26), DispositivoId = 1}
                },
                new List<Dispositivo>()
                {
                    new Dispositivo() {Id = 1, Tipo = DispositivoTipo.Attuatore}
                },
                new List<DateTime>() { new DateTime(2000, 11, 26, 18, 0, 0) },
                86.3f
            },
            {
                new List<Evento>()
                {
                    new Evento() {Tipo = EventoTipo.Temp, Valore = "25", Tempo = new DateTime(2000, 11, 26, 15, 0, 0)},
                    new Evento() {Tipo = EventoTipo.Rad, Valore = "15", Tempo = new DateTime(2000, 11, 26, 15, 0, 0)},
                    new Evento() {Tipo = EventoTipo.Rain, Valore = "10", Tempo = new DateTime(2000, 11, 26, 15, 0, 0)},
                    new Evento() {Tipo = EventoTipo.On, Valore = "", Tempo = new DateTime(2000, 11, 26, 6, 0, 0), DispositivoId = 1},
                    new Evento() {Tipo = EventoTipo.Off, Valore = "", Tempo = new DateTime(2000, 11, 27), DispositivoId = 1},
                    new Evento() {Tipo = EventoTipo.Temp, Valore = "25", Tempo = new DateTime(2000, 11, 27)},
                    new Evento() {Tipo = EventoTipo.Rad, Valore = "15", Tempo = new DateTime(2000, 11, 27)},
                    new Evento() {Tipo = EventoTipo.Rain, Valore = "10", Tempo = new DateTime(2000, 11, 27)},
                },
                new List<Dispositivo>()
                {
                    new Dispositivo() {Id = 1, Tipo = DispositivoTipo.Attuatore, CampoId = 1}
                },
                new List<DateTime>() {new DateTime(2000, 11, 26, 23, 59, 59), new DateTime(2000, 11, 27)},
                84.6f
            }
        };

        [Theory]
        [MemberData(nameof(tempData))]
        public async void GetTemp(List<Evento> eventi, DateTime data, (float?, float?) expected)
        {
            var campo = new Campo()
            {
                Eventi = eventi
            };
            _campoRepoMock.Setup(x => x.Campi.GetById(1)).ReturnsAsync(campo);

            var temp = await _sut.GetTemp(1, data);

            Assert.Equal(expected, temp);
        }

        [Theory]
        [MemberData(nameof(rainData))]
        public async void GetRain(List<Evento> eventi, DateTime data, float expected)
        {
            var campo = new Campo()
            {
                Eventi = eventi
            };
            _campoRepoMock.Setup(x => x.Campi.GetById(1)).ReturnsAsync(campo);

            var temp = await _sut.GetRain(1, data);

            Assert.Equal(expected, temp);
        }

        [Theory]
        [MemberData(nameof(radData))]
        public async void GetRad(List<Evento> eventi, DateTime data, float expected)
        {
            var campo = new Campo()
            {
                Eventi = eventi
            };
            _campoRepoMock.Setup(x => x.Campi.GetById(1)).ReturnsAsync(campo);

            var radMax = await _sut.GetMaxRad(1, data);

            Assert.Equal(expected, radMax);
        }

        [Theory]
        [MemberData(nameof(statusDispositivi))]
        public async void GetStatusDispositivi(List<Evento> eventi, List<Dispositivo> dispositivi, DateTime data, List<(int, (EventoTipo, DateTime))> expected)
        {
            var campo = new Campo()
            {
                Dispositivi = dispositivi,
                Eventi = eventi
            };
            _campoRepoMock.Setup(x => x.Campi.GetById(1)).ReturnsAsync(campo);

            var statusDispositivi = await _sut.GetStatusDispositivi(1, data);

            Assert.Equal(expected, statusDispositivi);
        }

        [Fact]
        public async void GetTempoIrrigazioneAutomatico()
        {
            var campo = new Campo()
            {
                S1 = 0.3f,
                S2 = 0.4f,
                Rd = 0.40,
                Aws = 175,
                Ac = 0.25f,
                Fr = 1,
                Ae = 92,
                Kc = 0.80,
                Adattivo = true,
                Dispositivi = new List<Dispositivo>() { new Dispositivo() { Id = 1, Tipo = DispositivoTipo.Attuatore } },
                Eventi = new List<Evento> { new Evento() { DispositivoId = 1, Tipo = EventoTipo.On, Tempo = new DateTime(2000, 11, 1) } }
            };
            _campoRepoMock.Setup(x => x.Campi.GetById(1)).ReturnsAsync(campo);

            var tempoIrrigazione = await _sut.GetIrriguoNetto(1, new DateTime(2000, 11, 2), 0);

            Assert.Equal(2.3f, tempoIrrigazione, 1f);
        }

        [Fact]
        public async void GetTempoIrrigazioneManuale()
        {
            var campo = new Campo()
            {
                S1 = 0.3f,
                S2 = 0.4f,
                Rd = 0.40,
                Aws = 175,
                Ac = 0.25f,
                Fr = 1,
                Ae = 92,
                Hum = 80,
                Adattivo = false,
                Dispositivi = new List<Dispositivo>() { new Dispositivo() { Id = 1, Tipo = DispositivoTipo.Attuatore } },
                Eventi = new List<Evento> { new Evento() { DispositivoId = 1, Tipo = EventoTipo.On, Tempo = new DateTime(2000, 11, 1) } }
            };
            _campoRepoMock.Setup(x => x.Campi.GetById(1)).ReturnsAsync(campo);

            var tempoIrrigazione = await _sut.GetIrriguoNetto(1, new DateTime(2000, 11, 2), 45);

            Assert.Equal(4.6f, tempoIrrigazione, 1f);
        }



        public static TheoryData<List<Evento>, List<Dispositivo>, double, DateTime> AcquaConsumata => new()
        {
            {
                new List<Evento>
                {
                    new Evento() {Id = 7, DispositivoId = 1, Tipo = EventoTipo.On, Tempo = DateTime.Parse("2/17/2024 1:51:17 AM")},
                    new Evento() {Id = 10, DispositivoId = 1, Tipo = EventoTipo.Off, Tempo = DateTime.Parse("2/17/2024 4:51:17 AM")},
                    new Evento() {Id = 11, DispositivoId = 1, Tipo = EventoTipo.On, Tempo = DateTime.Parse("2/18/2024 12:51:17 AM")},
                    new Evento() {Id = 13, DispositivoId = 1, Tipo = EventoTipo.Off, Tempo = DateTime.Parse("2/18/2024 2:51:17 AM")},
                    new Evento() {Id = 14, DispositivoId = 1, Tipo = EventoTipo.Off, Tempo = DateTime.Parse("2/18/2024 3:51:17 AM")},
                },
                new List<Dispositivo>
                {
                    new Dispositivo() {Id = 1, Tipo = DispositivoTipo.Attuatore},
                },
                2d,
                new DateTime(2024, 2, 18, 23, 59, 59)
            },

            {
                new List<Evento>
                {
                    new Evento() {Id = 12, DispositivoId = 3, Tipo = EventoTipo.On, Tempo = DateTime.Parse("2024-02-18T03:13:29.7056019")},
                    new Evento() {Id = 14, DispositivoId = 3, Tipo = EventoTipo.Off, Tempo = DateTime.Parse("2024-02-18T06:13:29.7056019")},
                },
                new List<Dispositivo>
                {
                    new Dispositivo() {Id = 3, Tipo = DispositivoTipo.Attuatore},
                },
                3d,
                new DateTime(2024, 2, 18, 23, 59, 59)
            },
            {
                new List<Evento>
                {
                    new Evento() {DispositivoId = 1, Tipo = EventoTipo.On, Tempo = new DateTime(2000, 10, 31)}
                },
                new List<Dispositivo>
                {
                    new Dispositivo() { Id = 1, Tipo = DispositivoTipo.Attuatore }
                },
                42d,
                new DateTime(2000, 11, 1, 18, 0, 0)
            },
            {
                new List<Evento>
                {
                    new Evento() {DispositivoId = 1, Tipo = EventoTipo.On, Tempo = new DateTime(2000, 11, 1, 12, 0, 0)}
                },
                new List<Dispositivo>
                {
                    new Dispositivo() { Id = 1, Tipo = DispositivoTipo.Attuatore}
                },
                6d,
                new DateTime(2000, 11, 1, 18, 0, 0)
            },
            {
                new List<Evento>
                {
                    new Evento() {DispositivoId = 1, Tipo = EventoTipo.Off, Tempo = new DateTime(2000, 10, 31)}
                },
                new List<Dispositivo>
                {
                    new Dispositivo() { Id = 1, Tipo = DispositivoTipo.Attuatore }
                },
                0d,
                new DateTime(2000, 11, 1, 18, 0, 0)
            },
            {
                new List<Evento>
                {
                    new Evento() {DispositivoId = 1, Tipo = EventoTipo.Off, Tempo = new DateTime(2000, 10, 31)},
                    new Evento() {DispositivoId = 1, Tipo = EventoTipo.On, Tempo = new DateTime(2000, 11, 1, 2, 0, 0)},
                    new Evento() {DispositivoId = 1, Tipo = EventoTipo.Off, Tempo = new DateTime(2000, 11, 1, 4, 0, 0)}
                },
                new List<Dispositivo>
                {
                    new Dispositivo() { Id = 1, Tipo = DispositivoTipo.Attuatore }
                },
                2d,
                new DateTime(2000, 11, 1, 18, 0, 0)
            },
            {
                new List<Evento>
                {
                    new Evento() {DispositivoId = 1, Tipo = EventoTipo.Off, Tempo = new DateTime(2000, 10, 31)},
                    new Evento() {DispositivoId = 1, Tipo = EventoTipo.On, Tempo = new DateTime(2000, 11, 1, 2, 0, 0)},
                    new Evento() {DispositivoId = 1, Tipo = EventoTipo.Off, Tempo = new DateTime(2000, 11, 1, 12, 0, 0)},
                    new Evento() {DispositivoId = 1, Tipo = EventoTipo.On, Tempo = new DateTime(2000, 11, 1, 16, 0, 0)},
                    new Evento() {DispositivoId = 1, Tipo = EventoTipo.On, Tempo = new DateTime(2000, 11, 1, 17, 0, 0)},
                    new Evento() {DispositivoId = 1, Tipo = EventoTipo.Off, Tempo = new DateTime(2000, 11, 1, 18, 0, 0)},
                },
                new List<Dispositivo>
                {
                    new Dispositivo() {Id = 1, Tipo = DispositivoTipo.Attuatore}
                },
                12d,
                new DateTime(2000, 11, 1, 18, 0, 0)
            },
            {
                new List<Evento>
                {
                    new Evento() {DispositivoId = 1, Tipo = EventoTipo.On, Tempo = new DateTime(2000, 11, 1, 12, 0, 0)},
                    new Evento() {DispositivoId = 2, Tipo = EventoTipo.On, Tempo = new DateTime(2000, 11, 1, 12, 0, 0)},
                    new Evento() {DispositivoId = 3, Tipo = EventoTipo.Off, Tempo = new DateTime(2000, 11, 1, 12, 0, 0)}
                },
                new List<Dispositivo>
                {
                    new Dispositivo() {Id = 1, Tipo = DispositivoTipo.Attuatore},
                    new Dispositivo() {Id = 2, Tipo = DispositivoTipo.Attuatore}
                },
                12d,
                new DateTime(2000, 11, 1, 18, 0, 0)
            },

        };

        [Theory]
        [MemberData(nameof(AcquaConsumata))]
        public async void GetAcquaConsumata(List<Evento> eventi, List<Dispositivo> dispositivi, double acqua, DateTime ora)
        {
            var campo = new Campo()
            {
                Id = 1,
                S1 = 0.3f,
                S2 = 0.4f,
                Rd = 0.40,
                Aws = 175,
                Ac = 0.25f,
                Fr = 1,
                Ae = 100,
                Dispositivi = dispositivi,
                Eventi = eventi
            };
            _campoRepoMock.Setup(x => x.Campi.GetById(1)).ReturnsAsync(campo);

            var acquaConsumata = await _sut.GetAcquaConsumata(1, ora);

            Assert.Equal(acqua, acquaConsumata, 1f);
        }

    }
}