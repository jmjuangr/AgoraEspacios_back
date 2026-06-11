using AgoraEspacios.Models.Entities;

namespace AgoraEspacios.Business.Services
{
    public interface IN8nService
    {
        // avisar a n8n cuando se crea una reserva
        Task EnviarReservaCreadaAsync(Reserva reserva, string nif);
    }
}
