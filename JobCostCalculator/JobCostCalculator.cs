using System;
using System.Collections.Generic;
using System.IO;
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
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }
        }
    }

    public class JobProcessor
    {
        private readonly JobCostCalculator calculator;
        private readonly IDao dao;

        public JobProcessor(JobCostCalculator calculator, IDao dao)
        {
            this.calculator = calculator;
            this.dao = dao;
        }

        public void Process()
        {
            dao.SaveInvoice(calculator.CalculateInvoice(dao.LoadJob()));
        }
    }

    public class FileDao : IDao
    {
        private readonly JobParser _parser;
        private readonly InvoiceRenderer _renderer;
        public string InputFilename { get; set; }
        public string OutputFilename { get; set; }

        public FileDao(JobParser parser, InvoiceRenderer renderer)
        {
            _parser = parser;
            _renderer = renderer;
        }

        public Job LoadJob()
        {
            return _parser.Parse(File.ReadLines(InputFilename));
        }

        public void SaveInvoice(Invoice invoice)
        {
            using (StreamWriter file = new StreamWriter(OutputFilename))
            {
                file.Write(_renderer.Render(invoice));
            }
        }
    }

    public class Invoice
    {
        public List<InvoiceItem> Items { get; set; }
        public decimal Total { get; set; }
    }

    public class InvoiceItem
    {
        public string Name { get; set; }

        public Decimal PrintingCost { get; set; }
    }

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

    public class Job
    {
        public bool ApplyExtraMargin { get; set; }

        public List<PrintItem> PrintItems { get; set; }
    }

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

    public class JobParser
    {
        public Job Parse(IEnumerable<string> input)
        {
            bool extraMargin = input.FirstOrDefault().Trim() == "extra-margin";

            var job = new Job
            {
                ApplyExtraMargin = extraMargin,
                PrintItems = input
                    .Skip(extraMargin ? 1 : 0)
                    .Select(ReadPrintItem)
                    .ToList()
            };

            return job;
        }

        private PrintItem ReadPrintItem(string line)
        {
            var tokens = line.Trim().Split(' ');
            return new PrintItem
            {
                Name = tokens[0],
                PrintingCost = Decimal.Parse(tokens[1]),
                TaxExempt = tokens.Length > 2 && tokens[2] == "exempt",
            };
        }
    }

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

    public class PrintItem
    {
        public string Name { get; set; }

        public Decimal PrintingCost { get; set; }

        public bool TaxExempt { get; set; }
    }

    public interface IDao
    {
        Job LoadJob();

        void SaveInvoice(Invoice invoice);
    }
}