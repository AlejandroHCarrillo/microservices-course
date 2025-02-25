using System;
using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CommandsService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }
        
        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    // TODO: 
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine($"Dertemining event {notificationMessage}");

            var evenType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (evenType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform Publiched Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Could not determinate the event type");
                    return EventType.Undetermined;
            }
        }


        private void addPlatform(string platformPublisedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

                var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublisedMessage);

                try
                {
                    var plat = _mapper.Map<Platform>(platformPublishedDto);
                    if (!repo.ExternalPlatExists(plat.ExternalID))
                    {
                        repo.CreatePlatform(plat);
                        repo.SaveChanges();
                    }
                    else {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine($"--> Platform Already exists... ");
                    }
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"--> Could not add Platform to DB {ex.Message}");
                } finally { 
                    Console.ResetColor();
                }
            }
        }

    }

    enum EventType { 
        PlatformPublished,
        Undetermined
    }
}
