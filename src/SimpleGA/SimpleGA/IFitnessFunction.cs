namespace SimpleGA
{
    /// <summary>
    /// Defines the contract for a fitness function.
    /// </summary>
    public interface IFitnessFunction
    {
        /// <summary>
        /// Every fitness function should Evaluate an Individual's fitness.
        /// </summary>
        /// <param name="individual"></param>
        /// <returns></returns>
        public double Evaluate(Individual individual);
    }
}