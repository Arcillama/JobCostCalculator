namespace JobCostCalculator
{
    public interface IDao
    {
        Job LoadJob();

        void SaveInvoice(Invoice invoice);
    }
}