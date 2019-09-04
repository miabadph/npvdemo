using System;
using System.Collections.Generic;

namespace NPVCalculatorDemo.Models
{
    public partial class CashflowTbl
    {
        public int Id { get; set; }
        public string CalculationId { get; set; }
        public string CashFlow { get; set; }
    }
}
