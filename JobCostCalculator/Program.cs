using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCostCalculator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            bool saveToFile;
            IEnumerable<string> invoiceInput;
            if (args.Length == 2)
            {
                invoiceInput = File.ReadAllLines(args[0]);
                saveToFile = true;
            }
            else
            {
                invoiceInput = LoadInputFromConsole();
            }
        }

        private static List<string> LoadInputFromConsole()
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
    }
}