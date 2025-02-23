using System.Collections.Generic;
using System;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;
using CommandsService.Models;

namespace CommandsService.Controllers
{
    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;
        public CommandsController(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"--> Getting Commands for the platform {platformId} from CommandService");
            Console.ResetColor();

            // Si no existe la plataforma, regresa Not Found
            if (!_repository.PlatformExists(platformId)) return NotFound();

            var commands = _repository.GetAllCommandsForPlatform(platformId);

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
        }

        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"--> Getting an specific Command (platformId: {platformId}, commandId: {commandId} ) from CommandService");
            Console.ResetColor();

            var command = _repository.GetCommand(platformId, commandId);

            if(command == null) return NotFound();

            return Ok(_mapper.Map<CommandReadDto>(command));
        }

        [HttpPost]
        public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"--> Creating Command for platformId: {platformId} from CommandService");
            Console.ResetColor();

            // Si no existe la plataforma, regresa Not Found
            if (!_repository.PlatformExists(platformId)) return NotFound();

            // convertimos el command DTO a command Model para poder usarlo en EF
            var command = _mapper.Map<Command>(commandDto);

            _repository.CreateCommand(platformId, command);

            _repository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(command);
            
            return CreatedAtRoute(nameof(GetCommandForPlatform),                 
                new { platformId = platformId, commandId = commandReadDto.Id}, 
                commandReadDto);
        }

    }
}
