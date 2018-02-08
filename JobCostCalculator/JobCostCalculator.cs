using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JobCostCalculator
{
    /*
 At InnerWorkings a "job" is a group of print items. For example,

a job can be a run of business cards, envelopes, and letterhead together.

Some items qualify as being sales tax free, whereas, by default, others

are not. Sales tax is 7%.

InnerWorkings also applies a margin, which is the percentage above printing

cost that is charged to the customer. For example, an item that costs $100

to print that has a margin of 11% will cost:

item: $100 -> $7 sales tax = $107

job: $100 -> $11 margin

total: $100 + $7 + $11 = $118

The base margin is 11% for all jobs this problem. Some jobs have an

"extra margin" of 5%. These jobs that are flagged as extra margin have

an additional 5% margin (16% total) applied.
    */
    public class Job
    {
        public bool ApplyExtraMargin { get; set; }

        public List<PrintItem> PrintItems { get; set; }
    }

    public class Invoice
    {
        public decimal Total { get; set; }

        public List<InvoiceItem> Items { get; set; }
    }

    public class JobCostCalculator
    {
        private readonly Job job;

        public Decimal ExtraMargin { get; set; }

        public Decimal Margin { get; set; }

        public Decimal SalesTax { get; set; }

        public JobCostCalculator(Job job)
        {
            this.job = job;
        }

        public Invoice CalculateInvoice()
        {
            /*
                The final cost is rounded to the nearest even cent. Individual items are
                rounded to the nearest cent.
            */

            //TODO RoundingUtils.RoundToNearestCent maybe test for that

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

        private decimal CalculateItemCost(PrintItem item)
        {
            return item.PrintingCost * (1 + (item.TaxExempt ? 0M : SalesTax));
        }

        private decimal CalculateMargin(PrintItem item)
        {
            return item.PrintingCost * (Margin + (job.ApplyExtraMargin ? ExtraMargin : 0m));
        }

        public string PrintOutInvoice()
        {
            var sb = new StringBuilder();
            var invoice = CalculateInvoice();

            invoice.Items.ForEach(i => sb.AppendLine($"{i.Name}: ${i.PrintingCost:0.00}"));

            sb.AppendFormat("total: ${0:0.00}", invoice.Total);
            sb.AppendLine();

            return sb.ToString();
        }
    }

    public class InvoiceItem
    {
        public string Name { get; set; }

        public Decimal PrintingCost { get; set; }
    }

    public class PrintItem
    {
        public string Name { get; set; }

        public Decimal PrintingCost { get; set; }

        public bool TaxExempt { get; set; }
    }

    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }

        public static IEnumerable<T> ProcessEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
                yield return item;
            }
        }
    }
}