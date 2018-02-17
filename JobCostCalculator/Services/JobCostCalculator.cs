using System;
using System.Linq;

namespace JobCostCalculator
{
    public class JobCostCalculator
    {
        public Decimal ExtraMargin { get; set; }
        public Decimal Margin { get; set; }
        public Decimal SalesTax { get; set; }

        public Invoice CalculateInvoice(Job job)
        {
            /*
                The final cost is rounded to the nearest even cent. Individual items are
                rounded to the nearest cent.
            */
            decimal CalculateItemCost(PrintItem item) => item.PrintingCost * (1 + (item.TaxExempt ? 0M : SalesTax));

            decimal CalculateMargin(PrintItem item) => item.PrintingCost * (Margin + (job.ApplyExtraMargin ? ExtraMargin : 0m));

            var invoice = new Invoice();

            var calc = job.PrintItems
                 .Select(item => new
                 {
                     Name = item.Name,
                     ItemCost = decimal.Round(CalculateItemCost(item), 2, MidpointRounding.AwayFromZero),
                     ItemMargin = CalculateMargin(item),
                 })
                 .ToList();

            invoice.Items = calc
                .Select(item => new InvoiceItem
                {
                    Name = item.Name,
                    PrintingCost = item.ItemCost
                })
                .ToList();

            decimal total = calc.Aggregate(0M, (sum, item) => sum + item.ItemCost + item.ItemMargin);

            invoice.Total = decimal.Round(total, 2, MidpointRounding.ToEven);

            return invoice;
        }
    }
}