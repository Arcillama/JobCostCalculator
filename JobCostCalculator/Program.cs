using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCostCalculator
{
    // If program is executed without parameters it accept input from standart input. Input finishes with blank line
    // I dont do any validation or complex error handling if anything goes wrong it outputs exception
    // Goal was to design it with easily Extensibility and Maintenance, because of that i had to add more classes that if it wassingle run program
    // I havent used interfaces on all places as solution doesnt need then - if they will be needed later (because more complexity ewill be requested) it is easy to refacotr to use it

    // One of examples failed (you can see unittest) dont know if it becase some error in my .net framework version : calculation rounding of value 2940.3091
    //
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