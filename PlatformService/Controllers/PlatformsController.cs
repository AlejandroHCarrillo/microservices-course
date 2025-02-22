using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(IPlatformRepo repository, IMapper mapper, ICommandDataClient commandDataClient)
        {
            _repository = repository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms...");
            var platformList = _repository.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platformList));
        }

        [HttpGet("{id}", Name = "GetPlatformById")] // see video 2:01:10 
        public ActionResult<PlatformReadDto> GetPlatformById(int id)
        {
            Console.WriteLine("--> Getting Platform...");
            var platformItem = _repository.GetPlatformById(id);
            if (platformItem != null)
            {
                return Ok(_mapper.Map<PlatformReadDto>(platformItem));
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
        {
            Console.WriteLine("--> Creating Platform desde el PlatformService...");
            var platformModel = _mapper.Map<Platform>(platformCreateDto);
            _repository.CreatePlatform(platformModel);
            _repository.SaveChanges();

            var platfromReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            // aqui enviamos el mensage al servicio de commandos
            // de que se ha creado una nueva plataforma
            // usando el nuevo httpclient SendPlatformToCommand
            try
            { 
                await _commandDataClient.SendPlatformToCommand(platfromReadDto);
            }
            catch (Exception ex)
            {
                // Si el servicio de commandos esta caido enviara una excepcion
                // informando que no se pudo enviar el mensaje al command service
                Console.WriteLine($"--> Could not send the messagge synchronosyly to the command service: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetPlatformById), new { id = platfromReadDto.Id }, platfromReadDto);
        }



    }
}
