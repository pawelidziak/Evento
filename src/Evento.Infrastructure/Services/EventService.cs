using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Evento.Core.Domain;
using Evento.Core.Repositories;
using Evento.Infrastructure.DTO;
using Evento.Infrastructure.Extensions;

namespace Evento.Infrastructure.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public EventService(IEventRepository eventRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<EventDetailDto> GetAsync(Guid id)
        {
            var @event = await _eventRepository.GetAsync(id);

            return _mapper.Map<EventDetailDto>(@event);
        }

        public async Task<EventDetailDto> GetAsync(string name)
        {
            var @event = await _eventRepository.GetAsync(name);

            return _mapper.Map<EventDetailDto>(@event);
        }

        public async Task<IEnumerable<EventDto>> BrowseAsyns(string name = null)
        {
            var events = await _eventRepository.BrowseAsync(name);

            return _mapper.Map<IEnumerable<EventDto>>(events);

        }

        public async Task CreateAsync(Guid id, string name, string description, DateTime startDate, DateTime endDate)
        {
            var @event = await _eventRepository.GetAsync(name);
            if (@event != null)
            {
                throw new Exception($"Event named: '{name}' already exists");
            }
            @event = new Event(id, name, description, startDate, endDate);
            await _eventRepository.AddAsync(@event);
        }

        public async Task AddTicketsAsync(Guid eventId, int amount, decimal price)
        {
            var @event = await _eventRepository.GetOrFailAsync(eventId);

            @event.AddTicket(amount, price);
            await _eventRepository.UpdateAsync(@event);
        }

        public async Task UpdateAsync(Guid id, string name, string description)
        {
            var @event = await _eventRepository.GetAsync(name);
            if (@event != null)
            {
                throw new Exception($"Event named: '{name}' already exists");
            }

            @event = await _eventRepository.GetOrFailAsync(id);
            
            @event.SetName(name);
            @event.SetDescription(description);
            
            await _eventRepository.UpdateAsync(@event);
        }

        public async Task DeleteAsync(Guid id)
        {
            var @event = await _eventRepository.GetOrFailAsync(id);
            await _eventRepository.DeleteAsync(@event);
        }


    }
}