using System;
using System.Collections.Generic;
using System.Linq;

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
        public IEnumerable<Ticket> PurchasedTickets => Tickets.Where(x => x.Purchased == true);
        public IEnumerable<Ticket> AvailableTickets => Tickets.Except(PurchasedTickets);


        protected Event() { }

        public Event(Guid id, string name, string description, DateTime startDate, DateTime endDate)
        {
            Id = id;
            SetName(name);
            SetDescription(description);
            StartDate = startDate;
            EndDate = endDate;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception($"Event with id: '{Id}' can not have an empty name");
            }
            Name = name;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new Exception($"Event with id: '{Id}' can not have an empty description");
            }
            Description = description;
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