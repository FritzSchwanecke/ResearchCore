namespace IReserachCore.Instruments.Cash
{
    public interface IEquityIndex
    {
        eIndexType Type { get; set; }
        double Level { get; set; }
        eCurrency Currency { get; set; }
    }
}