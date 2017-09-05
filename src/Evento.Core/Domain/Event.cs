using System;
using System.Collections.Generic;

namespace Evento.Core.Domain
{
    // klasa domenowa reprezentująca wydarzenie
    public class Event : Entity
    {
        private ISet<Ticket> _tickets = new HashSet<Ticket>();
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public DateTime CreatedAt { get; protected set; }
        public DateTime StartDate { get; protected set; }
        public DateTime EndDate { get; protected set; }
        public DateTime UpdatedAt { get; protected set; }

        // kolekcja biletów do wydarzenia, tylko do odczytu (dlatego IEnumerable a nie List), a dodawanie odbywa się poprzez odpowiednie metody w klasie Event
        // operujemy na prywanym _tickets, a konsumentowi zwracamy ISet widoczny jako IEnumerable
        public IEnumerable<Ticket> Tickets => _tickets;

        protected Event() { }

        public Event(Guid id, string name, string description, DateTime startDate, DateTime endDate)
        {
            Id = id;
            Name = name;
            Description = description;
            StartDate = startDate;
            EndDate = endDate;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        // metoda dodająca Ticket do Event'u
        public void AddTicket(int amount, decimal price)
        {
            // obliczenie miejsc
            var seating = _tickets.Count + 1;

            for (var i = 0; i < amount; i++)
            {
                _tickets.Add(new Ticket(this, 0, price));
                seating++;
            }
        }

    }
}