using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace SimpleGA
{
    /// <summary>
    /// A Gene represents a unique value.
    /// </summary>
    public class Gene
    {
        public Gene(double val)
        {
            Id = Guid.NewGuid();
            Value = val;
        }
        
        public Guid Id { get; private set; }
        public double Value { get; }
    }
}
