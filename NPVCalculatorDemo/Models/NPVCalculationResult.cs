using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPVCalculatorDemo.Models
{
    public class NPVCalculationResult
    {
        public string CashFlow { get; set; }
        public string CalculationId { get; set; }
        public double Rate { get; set; }
        public double ComputedValue { get; set; }
    }
}
