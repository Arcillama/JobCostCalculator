using System.Text;

namespace JobCostCalculator
{
    public class JobRenderer
    {
        public string Render(Job job)
        {
            var sb = new StringBuilder();

            if (job.ApplyExtraMargin) sb.AppendLine("extra-margin");
            job.PrintItems.ForEach(i =>
            {
                sb.Append($"{i.Name} {i.PrintingCost:0.00}");
                if (i.TaxExempt) sb.Append(" exempt");
                sb.AppendLine();
            });

            return sb.ToString();
        }
    }
}