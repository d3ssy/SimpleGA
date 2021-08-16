using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace SimpleGA
{
    /// <summary>
    /// An Individual is a unique collection of binary genes.
    /// </summary>
    public class Individual : IComparable<Individual>, IEquatable<Individual>
    {
        private readonly List<Gene> _dna = new List<Gene>();

        public Individual()
        {
            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Constructs an Individual with a randomized dna sequence consisting of a collection of binary genes. 
        /// </summary>
        /// <param name="geneCount">Number of genes for this individual. Must be an even number.</param>
        public Individual(int geneCount) : this()
        {
            if (geneCount < 2)
            {
                throw new ArgumentException("Gene count must be at least 2.");
            }

            if (geneCount % 2 != 0)
            {
                throw new ArgumentException("Gene count must be even.");
            }

            for (int i = 0; i < geneCount; i++)
            {
                Random random = new Random();
                int randomBinary = (random.NextDouble() - 0.5) * 2 > 0 ? 1 : 0;
                _dna.Add(new Gene(randomBinary));
            }
        }

        /// <summary>
        /// The binary string representation of the Individual's genes (a string of 0's and 1's).
        /// </summary>
        public string DnaBinarySequence
        {
            get
            {
                if (_dna != null && _dna.Any())
                {
                    var s = GetBinarySequence();
                    return s;
                }

                return string.Empty;
            }
        }

        public bool IsElite { get; set; }
        public Guid Id {get; }
        public List<Gene> Dna => _dna;
        public double Fitness { get; set; } = 0.0;
        public int GeneCount => _dna.Count;

        public int CompareTo(Individual other)
        {
            return other.Fitness.CompareTo(Fitness);
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("Individual: [ ");
            for (int i = 0; i < GeneCount; i++)
            {
                stringBuilder.Append(i == GeneCount - 1 ? $"{Dna[i].Value} ]" : $"{Dna[i].Value}, ");
            }

            return stringBuilder.ToString();
        }

        public bool Equals(Individual other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_dna, other._dna) && Id.Equals(other.Id) && Fitness.Equals(other.Fitness);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Individual) obj);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        private string GetBinarySequence()
        {
            var result = new StringBuilder();
            foreach (var gene in _dna)
            {
                result.Append(gene.Value.ToString(CultureInfo.InvariantCulture));
            }

            return result.ToString();
        }
    }
}
