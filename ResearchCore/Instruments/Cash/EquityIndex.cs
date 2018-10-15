using IReserachCore.Instruments;
using IReserachCore.Instruments.Cash;

namespace ResearchCore.Instruments.Cash
{
    public class EquityIndex : IEquityIndex
    {
        public eIndexType Type { get; set; }
        public double Level { get; set; }
        public eCurrency Currency { get; set; }
    }
}