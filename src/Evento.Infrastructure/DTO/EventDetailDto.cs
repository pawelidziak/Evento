using System.Collections.Generic;

namespace Evento.Infrastructure.DTO
{
    public class EventDetailDto : EventDto
    {
        public IEnumerable<TicketDto> Tickets { get; set; }
    }
}