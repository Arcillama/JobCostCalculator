﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JobCostCalculator
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var calc = new JobCostCalculator();

            calc.Calculate();
        }
    }
}