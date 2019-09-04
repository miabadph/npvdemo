using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NPVCalculatorDemo.Models
{
    public class NPVCalculationInfo
    {
        public List<double> CashFlow { get; set; }
        public double InitialValue { get; set; }
        public double LowerBound { get; set; }
        public double UpperBound { get; set; }
        public double Increment { get; set; }
    }
}
