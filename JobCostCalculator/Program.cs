using JobCostCalculator.Properties;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace JobCostCalculator
{
    // Execution:
    //        JobCostCalculator.exe [JobFile] [Invoicefile]
    //
    // Configuration:
    //      Margin,ExtraMargin,SalesTax values are read from app.config file

    // Standart Input Mode:
    //      If program is executed without parameters it accept input from standart input. Input finishes with empty line. Output is printout to console output.
    //      Input has to follow format from problem statement

    // File Mode:
    //      If program is executed with two parameters (they must be files or fullpath to file ) Then input job is taken from first one and Invoice saved to second one
    //      File has to be in exactly same format as on problem statement

    // Notes to solution:
    //      It doesnt print any usage of application (just want to keep simple as real app it would need more care as loging and so)
    //      It doesnt do any validation or complex error handling if anything goes wrong it outputs exception.
    //      Goal was to design it with regarding to Extensibility and Maintenance, because of that i had to add more classes than if it was short lived application
    //      I havent used interfaces on all places as solution doesnt need then - keep it simple unless required to make complex
    //            - if they will be needed later it is easy to refactor and add them
    //      Sorry for format and namespace that are missing I dont have any sufficient tool at home

    //Known bugs:
    //      One of examples failed (you can see failed unittests) dont know if it becase some error in my .net framework (or i am missing point somewhere out there) but issue is :
    //  calculation rounding of value 2940.3091 was rounded to 2940.31 (with mid point even) I think its bug as it possibilities to round are: .30 or .32 surely not .31
    //
    // Also sorry for mess in git interactive rebase dont work on my box, idealy some commit should be squashed together
    internal class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                bool usefiles = (args.Length == 2);

                var calculator = new JobCostCalculator()
                {
                    ExtraMargin = Settings.Default.ExtraMargin,
                    Margin = Settings.Default.Margin,
                    SalesTax = Settings.Default.SalesTax,
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
}