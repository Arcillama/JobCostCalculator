namespace JobCostCalculator
{
    public class JobProcessor
    {
        private readonly JobCostCalculator calculator;
        private readonly IDao dao;

        public JobProcessor(JobCostCalculator calculator, IDao dao)
        {
            this.calculator = calculator;
            this.dao = dao;
        }

        public void Process()
        {
            dao.SaveInvoice(calculator.CalculateInvoice(dao.LoadJob()));
        }
    }
}