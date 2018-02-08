using NUnit.Framework;
using System.Collections.Generic;

namespace JobCostCalculator.Tests
{
    [TestFixture]
    public class JobCostCalculatorTests
    {
        private static object[] TestInvoiceSet =
        {
            new object[] { JobsMother.CreateJob1(), JobsMother.ExpectedInvoiceJob1 },
            new object[] { JobsMother.CreateJob2(), JobsMother.ExpectedInvoiceJob2 },
            new object[] { JobsMother.CreateJob3(), JobsMother.ExpectedInvoiceJob3 }
        };

        [TestCaseSource(nameof(TestInvoiceSet))]
        public void Ivoice_for_job_should_be_calculated(Job job, string expectedOutput)
        {
            var calc = new JobCostCalculator(job);

            calc.ExtraMargin = 0.05m;
            calc.Margin = 0.11m;
            calc.SalesTax = 0.07m;

            string invoiceReport = calc.PrintOutInvoice();//later we will refactor to some renderer if needed- - TODO : invoiceCalculator and InvoiceRender

            System.Console.WriteLine(expectedOutput);
            System.Console.WriteLine();
            System.Console.WriteLine(invoiceReport);

            Assert.AreEqual(expectedOutput, invoiceReport);
        }

        public static class JobsMother
        {
            public static Job CreateJob1()
            {
                return new Job
                {
                    ApplyExtraMargin = true,
                    PrintItems = new List<PrintItem>
                    {
                        new PrintItem
                        {
                            Name = "envelopes",
                            PrintingCost =520.00m,
                        },
                        new PrintItem
                        {
                            Name = "letterhead",
                            PrintingCost =1983.37m,
                            TaxExempt = true
                        }
                    }
                };
            }

            public static string ExpectedInvoiceJob1 =
                @"envelopes: $556.40
letterhead: $1983.37
total: $2940.30
";

            public static Job CreateJob2()
            {
                return new Job
                {
                    PrintItems = new List<PrintItem>
                    {
                    new PrintItem
                    {
                    Name = "t-shirts",
                    PrintingCost =294.04m,
                }
            }
                };
            }

            public static string ExpectedInvoiceJob2 =
            @"t-shirts: $314.62
total: $346.96
";

            public static Job CreateJob3()
            {
                return new Job
                {
                    ApplyExtraMargin = true,
                    PrintItems = new List<PrintItem>
                {
                new PrintItem
                {
                    Name = "frisbees",
                    PrintingCost = 19385.38m,
                    TaxExempt = true
                },
                new PrintItem
                {
                    Name = "yo-yos",
                    PrintingCost =1829m,
                    TaxExempt = true
                }
            }
                };
            }

            public static string ExpectedInvoiceJob3 =
            @"frisbees: $19385.38
yo-yos: $1829.00
total: $24608.68
";
        }
    }
}
/*
Job 1:

extra-margin

envelopes 520.00

letterhead 1983.37 exempt

should output:

envelopes: $556.40

letterhead: $1983.37

total: $2940.30

Job 2:

t-shirts 294.04

output:

t-shirts: $314.62

total: $346.96

Job 3:

extra-margin

frisbees 19385.38 exempt

yo-yos 1829 exempt

output:

frisbees: $19385.38

yo-yos: $1829.00

total: $24608.68
 */