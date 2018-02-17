using System;
using System.Collections.Generic;
using System.Linq;

namespace JobCostCalculator
{
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
                    .Where(l => !String.IsNullOrEmpty(l))
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
}