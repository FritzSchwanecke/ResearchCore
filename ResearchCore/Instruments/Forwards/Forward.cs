using System;
using IReserachCore.Instruments;
using IReserachCore.Instruments.Forwards;

namespace ResearchCore.Instruments.Forwards
{
    public class Forward : IForward
    {
        public DateTime StartDate { get; set; }
        public DateTime Maturity { get; set; }
        public eCurrency Currency { get; set; }
        public double Nominal { get; set; }
        public double AgreedPrice { get; set; }
    }
}