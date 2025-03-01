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
                    addPlatform(message);
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
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("--> Platform Published Event Detected");
                    Console.ResetColor();
                    return EventType.PlatformPublished;
                default:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("--> Could not determinate the event type");
                    Console.ResetColor();
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
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"--> Platform added!... ");
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
