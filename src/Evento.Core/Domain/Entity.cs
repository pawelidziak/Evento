using System;

namespace Evento.Core.Domain
{
    // klasa domenowa reprezentująca encję posiadającą unikalny identyfikator
    public abstract class Entity
    {
        public Guid Id { get; protected set; }

        protected Entity()
        {
            Id = Guid.NewGuid();
        }
    }
}