﻿using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace JobCostCalculator.Tests
{
    [TestFixture]
    public class JobCostCalculatorTests
    {
        private static object[] TestInvoiceSet =
        {
            new object[] { TestDataMother.CreateJob1(), TestDataMother.ExpectedInvoiceJob1 },
            new object[] { TestDataMother.CreateJob2(), TestDataMother.ExpectedInvoiceJob2 },
            new object[] { TestDataMother.CreateJob3(), TestDataMother.ExpectedInvoiceJob3 }
        };

        private static object[] TestStringSet =
       {
            new object[] { TestDataMother.ExpectedInputJob1, TestDataMother.ExpectedInvoiceJob1 },
            new object[] { TestDataMother.ExpectedInputJob2, TestDataMother.ExpectedInvoiceJob2 },
            new object[] { TestDataMother.ExpectedInputJob3, TestDataMother.ExpectedInvoiceJob3 },
            new object[] { TestDataMother.ExpectedInputEptyJob, TestDataMother.ExpectedOutputEptyJob }
        };

        private JobCostCalculator calculator;

        [SetUp]
        public void SetUp()
        {
            calculator = new JobCostCalculator()
            {
                ExtraMargin = 0.05m,
                Margin = 0.11m,
                SalesTax = 0.07m
            };
        }

        [TestCaseSource(nameof(TestInvoiceSet))]
        public void Ivoice_for_job_should_be_rendered(Job job, string expectedOutput)
        {
            var renderer = new InvoiceRenderer();

            string invoiceReport = renderer.Render(calculator.CalculateInvoice(job));

            Assert.AreEqual(expectedOutput, invoiceReport);
        }

        [TestCaseSource(nameof(TestStringSet))]
        public void Job_should_be_loaded_from_file_and_invoice_should_pe_persited_tofile(string expectedInput, string expectedOutput)
        {
            var outputFilename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestInvoice.txt");
            var inputFilename = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestJob.txt");

            File.Delete(inputFilename);
            Assert.False(File.Exists(inputFilename), "Test cannot run - file was not deleted");
            File.Delete(outputFilename);
            Assert.False(File.Exists(outputFilename), "Test cannot run - file was not deleted");
            SaveFile(expectedInput, inputFilename);

            var renderer = new InvoiceRenderer();
            var parser = new JobParser();
            var persister = new FileDao(parser, renderer)
            {
                InputFilename = inputFilename,
                OutputFilename = outputFilename,
            };

            var processor = new JobProcessor(calculator, persister);
            processor.Process();

            Assert.True(File.Exists(outputFilename), "File wasn't saved");
            Assert.AreEqual(expectedOutput, File.ReadAllText(outputFilename));
        }

        private void SaveFile(string content, string filename)
        {
            using (StreamWriter file = new StreamWriter(filename))
            {
                file.Write(content);
            }
        }
    }

    public static class TestDataMother
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

        public static string ExpectedInputJob1 =
@"extra-margin
envelopes 520.00
letterhead 1983.37 exempt
";

        public static string ExpectedInputJob2 =
@"t-shirts 294.04
";

        public static string ExpectedInputJob3 =
@"extra-margin
frisbees 19385.38 exempt
yo-yos 1829 exempt
";

        public static string ExpectedInputEptyJob =
@"
";
        public static string ExpectedOutputEptyJob =
@"total: $0.00
";
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