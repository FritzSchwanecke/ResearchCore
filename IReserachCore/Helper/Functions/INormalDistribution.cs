namespace IReserachCore.Helper.Functions
{
    public interface INormalDistribution
    {
        IErfc CodyErfcFunction { get; set; }
        double NormCdf(double z);
        double InverseNormCdf(double u);
        double NormPdf(double x);
    }
}