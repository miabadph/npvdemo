using System;
using System.Collections.Generic;

namespace NPVCalculatorDemo.Models
{
    public partial class NpvTbl
    {
        public int Id { get; set; }
        public string CalculationId { get; set; }
        public double Rate { get; set; }
        public double ComputedValue { get; set; }
    }
}
