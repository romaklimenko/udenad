namespace Udenad.Core
{
    public enum Score : byte
    {
        /// <summary>
        ///     5 - perfect response
        /// </summary>
        S5 = 5,

        /// <summary>
        ///     4 - correct response after a hesitation
        /// </summary>
        S4 = 4,

        /// <summary>
        ///     3 - correct response recalled with serious difficulty
        /// </summary>
        S3 = 3,

        /// <summary>
        ///     2 - incorrect response; where the correct one seemed easy to recall
        /// </summary>
        S2 = 2,

        /// <summary>
        ///     1 - incorrect response; the correct one remembered
        /// </summary>
        S1 = 1,

        /// <summary>
        ///     0 - complete blackout.
        /// </summary>
        S0 = 0
    }
}