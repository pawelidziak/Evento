using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Evento.Infrastructure.Commands.Events;
using Evento.Infrastructure.DTO;
using Evento.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace Evento.Api.Controllers
{
    [Route("[controller]")]
    public class EventsController : ApiControllerBase
    {

        private readonly IEventService _eventService;
        private readonly IMemoryCache _cache;

        public EventsController(IEventService eventService, IMemoryCache cache)
        {
            _eventService = eventService;
            _cache = cache;
        }

        // pobierz wszystkie wydarzenia 
        [HttpGet]
        public async Task<IActionResult> Get(string name)
        {
            var events = _cache.Get<IEnumerable<EventDto>>("events");
            if (events == null)
            {
                Console.WriteLine("Fetching from service.");
                events = await _eventService.BrowseAsyns(name);
                _cache.Set("events", events, TimeSpan.FromMinutes(1));
                // "events" lepiej by było dać do zmiennej, TimeSpan to ile czasu dane maja w cachu lezec
            }
            else{
                Console.WriteLine("Fetching from cache.");
            }
            return Json(events);
        }

        // pobierz szczegóły wydarzenia 
        [HttpGet("{eventId}")]
        public async Task<IActionResult> Get(Guid eventId)
        {
            var @event = await _eventService.GetAsync(eventId);
            if (@event == null)
            {
                return NotFound(); //404
            }
            return Json(@event);
        }

        // utwórz wydarzenie
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CreateEvent command)
        {
            command.EventId = Guid.NewGuid();
            await _eventService.CreateAsync(command.EventId, command.Name, command.Description, command.StartDate, command.EndDate);
            await _eventService.AddTicketsAsync(command.EventId, command.Tickets, command.Price);

            return Created($"/events/{command.EventId}", null); // 201
        }

        // edytuj wydarzenie
        [HttpPut("{eventId}")]
        public async Task<IActionResult> Put(Guid eventId, [FromBody]UpdateEvent command)
        {
            command.EventId = Guid.NewGuid();
            await _eventService.UpdateAsync(eventId, command.Name, command.Description);
            return NoContent(); //204
        }

        // usun wydarzenie /events/{id}
        [HttpDelete("{eventId}")]
        public async Task<IActionResult> Delete(Guid eventId)
        {
            await _eventService.DeleteAsync(eventId);
            return NoContent(); //204
        }
    }
}