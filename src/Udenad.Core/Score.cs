using System.ComponentModel;

namespace Udenad.Core
{
    public enum Score : byte
    {
        [Description("5 - perfect response")]
        S5 = 5,

        [Description("4 - correct response after a hesitation")]
        S4 = 4,

        [Description("3 - correct response recalled with serious difficulty")]
        S3 = 3,


        [Description("2 - incorrect response; where the correct one seemed easy to recall")]
        S2 = 2,

        [Description("1 - incorrect response; the correct one remembered")]
        S1 = 1,

        [Description("0 - complete blackout")]
        S0 = 0
    }
}