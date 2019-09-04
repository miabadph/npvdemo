using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NPVCalculatorDemo.Models;

namespace NPVCalculatorDemo.Controllers
{
    [Route("api/NPV")]
    [ApiController]
    public class NpvTblsController : ControllerBase
    {
        private readonly DB_A4CAF7_npvContext _context;

        public NpvTblsController(DB_A4CAF7_npvContext context)
        {
            _context = context;
        }

        // GET: api/NpvTbls
        [HttpGet]
        public IEnumerable<NpvTbl> GetNpvTbl()
        {
            return _context.NpvTbl;
        }

        // GET: api/NpvTbls/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNpvTbl([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var npvTbl = await _context.NpvTbl.FindAsync(id);
            var cId = await _context.NpvTbl.Select(o => o.CalculationId).Distinct().ToListAsync();
            if (npvTbl == null)
            {
                return NotFound();
            }

            return Ok(npvTbl);
        }

        [HttpGet("GetCalculationsById")]
        public async Task<IActionResult> GetCalculationsById(string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //var calculations = await _context.NpvTbl.Where(o => o.CalculationId == id).ToListAsync();
            var query = from c in _context.NpvTbl
                        join cf in _context.CashflowTbl on c.CalculationId equals cf.CalculationId into joined
                        from jr in joined.DefaultIfEmpty()
                        where c.CalculationId == id
                        select new NPVCalculationResult()
                        {
                            CalculationId = c.CalculationId,
                            CashFlow = jr.CashFlow,
                            Rate = c.Rate,
                            ComputedValue = c.ComputedValue

                        };
            var result = await query.ToListAsync();
            return Ok(result);
        }

        [HttpGet("GetAllCalculationId")]
        public async Task<IActionResult> GetAllCalculationIdAsync()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var cId = await _context.NpvTbl.Select(o => o.CalculationId).Distinct().ToListAsync();
            return Ok(cId);
        }

        [HttpPost("CalculateNPV")]
        public async Task<IActionResult> CalculateNPV(NPVCalculationInfo input)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (input.CashFlow.Count == 0 || input.InitialValue == 0)
            {
                return BadRequest();
            }

            var cId = DateTime.Now.ToString("yyyyMMddHHmmss");
            var cfStr = string.Join(",", input.CashFlow);
            List<NpvTbl> computedValues = new List<NpvTbl>();
            var rates = new List<double>();
            double templb = input.LowerBound;
            while (templb < input.UpperBound)
            {
                rates.Add(Math.Round(templb, 2));
                templb += input.Increment;
                if (templb >= input.UpperBound)
                {
                    rates.Add(Math.Round(input.UpperBound, 2));
                }
            }
            foreach (var rt in rates)
            {
                var compInfo = new NpvTbl();
                compInfo.CalculationId = cId;
                compInfo.ComputedValue = GetNPV(rt, input.InitialValue, input.CashFlow);
                compInfo.Rate = rt;
                _context.NpvTbl.Add(compInfo);
            }
            
            var cf = new CashflowTbl();
            cf.CalculationId = cId;
            cf.CashFlow = cfStr;
            _context.CashflowTbl.Add(cf);

            await _context.SaveChangesAsync();

            return Ok(cId);
        }

        public double GetNPV(double rate, double initialCost, List<double> cashFlows)
        {
            double npv = initialCost;
            if (npv > 0)
            {
                npv = (npv * - 1);
            }
            double dRate = Math.Round((rate / 100),2);
            for (var i = 0; i < cashFlows.Count; i++)
            {
                npv += cashFlows[i] / Math.Pow(1 + dRate, i + 1);
            }

            return npv;
        }

        [HttpPost]
        public async Task<IActionResult> PostNpvTbl([FromBody] NpvTbl npvTbl)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.NpvTbl.Add(npvTbl);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNpvTbl", new { id = npvTbl.Id }, npvTbl);
        }

        private bool NpvTblExists(int id)
        {
            return _context.NpvTbl.Any(e => e.Id == id);
        }
    }
}