using NUnit.Framework;
using JobCostCalculator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCostCalculator.Tests
{
    [TestFixture]
    public class JobCostCalculatorTests
    {
        [Test]
        public void Ivoice_for_job_should_be_calculated()
        {
            var calc = new JobCostCalculator();
            calc.Calculate();

            Assert.Fail();
        }
    }
}