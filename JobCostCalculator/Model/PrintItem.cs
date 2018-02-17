using System;

namespace JobCostCalculator
{
    public class PrintItem
    {
        public string Name { get; set; }

        public Decimal PrintingCost { get; set; }

        public bool TaxExempt { get; set; }
    }
}