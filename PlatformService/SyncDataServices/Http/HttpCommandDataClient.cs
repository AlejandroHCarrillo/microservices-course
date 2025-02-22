using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configurarion;

        public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configurarion = configuration;
        }
        /// <summary>
        /// Este metodo construye un request con la plataforma creada y
        /// lo manda al sercio de comando
        /// usa la url llamada CommandService guardada en el appsettings.XXX.json 
        /// para acceder al endpoint del commandService
        /// ejemplo: http://localhost:6000/api/c/platforms/
        /// </summary>
        /// <param name="plat"></param>
        /// <returns></returns>
        public async Task SendPlatformToCommand(PlatformReadDto plat)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(plat),
                Encoding.UTF8,
                "application/json"
                );
            var response = await _httpClient.PostAsync($"{_configurarion["CommandService"]}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> Sync Post to Command Service was OK!");
            }
            else
            {
                Console.WriteLine("--> Sync Post to Command Service Failed!!!");
            }
        }
    }
}
