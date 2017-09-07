using System;

namespace Evento.Core.Domain
{
    public class Ticket : Entity
    {
        public Guid EventId { get; protected set; }
        public int Seating { get; protected set; }
        public decimal Price { get; protected set; }
        public Guid? UserId { get; protected set; }
        public string Username { get; protected set; }
        public DateTime? PurchasedAt { get; protected set; }
        public bool Purchased => UserId.HasValue;

        protected Ticket() { }

        public Ticket(Event @event, int seating, decimal price)
        {
            EventId = @event.Id;
            Seating = seating;
            Price = price;
        }

        public void Purchase(User user)
        {
            if(Purchased)
            {
                throw new Exception($"Ticket was already purchased by user: '{Username}' at: '{PurchasedAt}'.");
            }
            UserId = user.Id;
            Username = user.Name;
            PurchasedAt = DateTime.UtcNow;
        }

        public void Cancel()
        {
            if(!Purchased)
            {
                throw new Exception($"Ticket was not purchased and can not be canceled.");
            }
            UserId = null;
            Username = null;
            PurchasedAt = null;
        }
    }
}