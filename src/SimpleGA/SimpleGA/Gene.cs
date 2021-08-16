using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleGA
{
    /// <summary>
    /// A Gene represents a unique value.
    /// </summary>
    public class Gene : IEquatable<Gene>
    {
        public Gene(int val)
        {
            if (val < 0 || val > 1 )
            {
                throw new ArgumentException("Gene value should be 0 or 1.");
            }
            Id = Guid.NewGuid();
            Value = val;
        }
        
        public Guid Id { get; }
        public double Value { get; }

        public bool Equals(Gene other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id) && Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Gene) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
