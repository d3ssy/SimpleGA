namespace SimpleGA.Interfaces
{
    /// <summary>
    /// Defines the contract for a fitness function.
    /// </summary>
    public interface IFitnessFunction
    {
        /// <summary>
        /// Evaluates an Individual's fitness.
        /// </summary>
        /// <param name="individual">Individual.</param>
        /// <returns>Fitness.</returns>
        double Evaluate(Individual individual);

        /// <summary>
        /// Decode binary sequence into parameters.
        /// </summary>
        /// <param name="individual">Individual.</param>
        /// <returns>Decoded sequence as array of double.</returns>
        double[] DecodeDnaBinarySequence(Individual individual);
    }
}