using System.Collections.Generic;

namespace JobCostCalculator
{
    public class Job
    {
        public bool ApplyExtraMargin { get; set; }

        public List<PrintItem> PrintItems { get; set; }
    }
}