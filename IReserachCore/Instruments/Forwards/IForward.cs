using System;

namespace IReserachCore.Instruments.Forwards
{
    public interface IForward
    {
        DateTime StartDate { get; set; }
        DateTime Maturity { get; set; }
        eCurrency Currency { get; set; }
        double Nominal { get; set; }
        double AgreedPrice { get; set; }
    }
}