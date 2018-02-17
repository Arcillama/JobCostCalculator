using System.Collections.Generic;

namespace JobCostCalculator
{
    public class Invoice
    {
        public List<InvoiceItem> Items { get; set; }
        public decimal Total { get; set; }
    }
}