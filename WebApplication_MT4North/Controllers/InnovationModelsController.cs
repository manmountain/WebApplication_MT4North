using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication_MT4North.Models;

namespace WebApplication_MT4North.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InnovationModelsController : ControllerBase
    {
        private readonly CUSERSKAJO59SOURCEREPOSWEBAPPLICATION_MT4NORTHWEBAPPLICATION_MT4NORTHMODELSMEDTECHINNOVATIONMODEL_V2MDFContext _context;

        public InnovationModelsController(CUSERSKAJO59SOURCEREPOSWEBAPPLICATION_MT4NORTHWEBAPPLICATION_MT4NORTHMODELSMEDTECHINNOVATIONMODEL_V2MDFContext context)
        {
            _context = context;
        }

        // GET: api/InnovationModels
        [Route("~/api/GetAllInnovationModels")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InnovationModel>>> GetInnovationModels()
        {
            return await _context.InnovationModels.ToListAsync();
        }

        // GET: api/InnovationModels/5
        [Route("~/api/GetInnovationModel")]
        [HttpGet("{id}")]
        public async Task<ActionResult<InnovationModel>> GetInnovationModel(int id)
        {
            var innovationModel = await _context.InnovationModels.FindAsync(id);

            if (innovationModel == null)
            {
                return NotFound();
            }

            return innovationModel;
        }

        // PUT: api/InnovationModels/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Route("~/api/UpdateInnovationModel")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInnovationModel(int id, InnovationModel innovationModel)
        {
            if (id != innovationModel.InnovationModelId)
            {
                return BadRequest();
            }

            _context.Entry(innovationModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InnovationModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/InnovationModels
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Route("~/api/AddInnovationModel")]
        [HttpPost]
        public async Task<ActionResult<InnovationModel>> PostInnovationModel(InnovationModel innovationModel)
        {
            _context.InnovationModels.Add(innovationModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInnovationModel", new { id = innovationModel.InnovationModelId }, innovationModel);
        }

        // DELETE: api/InnovationModels/5
        [Route("~/api/DeleteInnovationModel")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<InnovationModel>> DeleteInnovationModel(int id)
        {
            var innovationModel = await _context.InnovationModels.FindAsync(id);
            if (innovationModel == null)
            {
                return NotFound();
            }

            _context.InnovationModels.Remove(innovationModel);
            await _context.SaveChangesAsync();

            return innovationModel;
        }

        private bool InnovationModelExists(int id)
        {
            return _context.InnovationModels.Any(e => e.InnovationModelId == id);
        }
    }
}
