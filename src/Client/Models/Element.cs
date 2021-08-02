using System;

namespace Simucraft.Client.Models
{
    public class Element<T>
    {
        public Guid Id { get; set; }

        public T Value { get; set; }

        public static Element<T> Empty =>
            new Element<T>
            {
                Id = Guid.NewGuid(),
            };
    }
}
