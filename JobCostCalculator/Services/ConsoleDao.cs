using System;
using System.Collections.Generic;
using System.Linq;

namespace JobCostCalculator
{
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