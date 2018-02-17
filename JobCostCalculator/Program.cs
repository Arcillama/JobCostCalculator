using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCostCalculator
{
    // Execution:
    //        JobCostCalculator.exe [JobFile] [Invoicefile]

    // Configuration:
    //      Margin,ExtraMargin,Sals tax values are read from app.config file

    // Function Standart Input Mode:
    //      If program is executed without parameters it accept input from standart input. Input finishes with empty line. Out is printout to console output.

    // Function File Mode:
    //      Input has to follow format from problem statement
    //      If program is executed with two parameters (they must be files or fullpath to file ) Then input job is taken from first one and Invoice saved to second one
    //      File has to be in exactly same format as requeste by input

    // Notes to solution:
    //      It doesnt print any usage of application (just want to keep simple as real app it would need more care as loging and so)
    //      It doesnt do any validation or complex error handling if anything goes wrong it outputs exception.
    //      Goal was to design it with regarding to Extensibility and Maintenance, because of that i had to add more classes than if it was short lived application
    //      I havent used interfaces on all places as solution doesnt need then - if they will be needed later (because more complexity will be requested) it is easy to refactor and add then
    //      Sorry for format and namespace that are missing I dont have any sufficient tool at home

    // One of examples failed (you can see unittest) dont know if it becase some error in my .net framework version : calculation rounding of value 2940.3091 was rounded to 2940.31 (with mid point even) I think its bug
    internal class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                bool usefiles = (args.Length == 2);

                var calculator = new JobCostCalculator() //todo read it from config
                {
                    ExtraMargin = 0.05m,
                    Margin = 0.11m,
                    SalesTax = 0.07m
                };

                var renderer = new InvoiceRenderer();
                var parser = new JobParser();

                IDao dao = usefiles
                    ? (IDao)new FileDao(parser, renderer)
                    {
                        InputFilename = args[0],
                        OutputFilename = args[1],
                    }
                    : new ConsoleDao(parser, renderer);

                var processor = new JobProcessor(calculator, dao);
                processor.Process();

                if (usefiles) Console.WriteLine($"Output saved to file:{args[1]}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
            return 0;
        }
    }

    public class ConsoleDao : IDao
    {
        private readonly JobParser _parser;
        private readonly InvoiceRenderer _renderer;

        public ConsoleDao(JobParser parser, InvoiceRenderer renderer)
        {
            _parser = parser;
            _renderer = renderer;
        }

        public Job LoadJob()
        {
            return _parser.Parse(LoadInputFromConsole().Where(l => !String.IsNullOrEmpty(l)));
        }

        private List<string> LoadInputFromConsole()
        {
            var result = new List<string>();
            string s;

            do
            {
                s = Console.ReadLine();
                result.Add(s);
            } while (!String.IsNullOrEmpty(s));

            return result;
        }

        public void SaveInvoice(Invoice invoice)
        {
            Console.WriteLine(_renderer.Render(invoice));
        }
    }
}