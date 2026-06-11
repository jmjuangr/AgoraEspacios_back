using AgoraEspacios.Business.Services;
using AgoraEspacios.Models.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;

namespace AgoraEspacios.Business.Services
{
    public class N8nService : IN8nService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<N8nService> _logger;

        public N8nService(
            HttpClient httpClient,
            IConfiguration configuration,
            ILogger<N8nService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task EnviarReservaCreadaAsync(Reserva reserva, string nif)
        {
            // Cojo la URL del webhook desde la configuracion
            var webhookUrl = _configuration["N8n:ReservaCreadaWebhookUrl"];
            if (string.IsNullOrWhiteSpace(webhookUrl))
            {
                // Si no hay URL no llamo a n8n
                return;
            }

            try
            {
                // se manda nif a n8n
                using var response = await _httpClient.PostAsJsonAsync(webhookUrl, new { nif });
                if (!response.IsSuccessStatusCode)
                {
                    // si n8n no responde se deja en logs
                    _logger.LogWarning(
                        "Error al llamar al webhook n8n de reserva creada. ReservaId: {ReservaId}, UsuarioId: {UsuarioId}, StatusCode: {StatusCode}",
                        reserva.Id,
                        reserva.UsuarioId,
                        (int)response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                // error si n8n no responde
                _logger.LogError(
                    ex,
                    "No se pudo llamar al webhook n8n de reserva creada. ReservaId: {ReservaId}, UsuarioId: {UsuarioId}",
                    reserva.Id,
                    reserva.UsuarioId);
            }
        }
    }
}
