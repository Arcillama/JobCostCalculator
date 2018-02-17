using System.Text;

namespace JobCostCalculator
{
    public class InvoiceRenderer
    {
        public string Render(Invoice invoice)
        {
            var sb = new StringBuilder();

            invoice.Items.ForEach(i => sb.AppendLine($"{i.Name}: ${i.PrintingCost:0.00}"));

            sb.AppendFormat("total: ${0:0.00}", invoice.Total);
            sb.AppendLine();

            return sb.ToString();
        }
    }
}