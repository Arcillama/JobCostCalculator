using System.IO;

namespace JobCostCalculator
{
    public class FileDao : IDao
    {
        private readonly JobParser _parser;
        private readonly InvoiceRenderer _renderer;
        public string InputFilename { get; set; }
        public string OutputFilename { get; set; }

        public FileDao(JobParser parser, InvoiceRenderer renderer)
        {
            _parser = parser;
            _renderer = renderer;
        }

        public Job LoadJob()
        {
            return _parser.Parse(File.ReadLines(InputFilename));
        }

        public void SaveInvoice(Invoice invoice)
        {
            using (StreamWriter file = new StreamWriter(OutputFilename))
            {
                file.Write(_renderer.Render(invoice));
            }
        }
    }
}