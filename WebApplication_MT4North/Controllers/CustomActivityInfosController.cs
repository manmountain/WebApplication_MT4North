using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication_MT4North.Models;

namespace WebApplication_MT4North.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomActivityInfosController : ControllerBase
    {
        private readonly MT4NorthContext _context;

        public CustomActivityInfosController(MT4NorthContext context)
        {
            _context = context;
        }

        // GET: api/CustomActivityInfos
        [Authorize()]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomActivityInfo>>> GetCustomActivityInfos()
        {
            var customActivityInfos = await _context.CustomActivityInfos.ToListAsync();
            return customActivityInfos;
        }

        // GET: api/CustomActivityInfos/5
        [Authorize()]
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomActivityInfo>> GetCustomActivityInfo(int id)
        {
            var customActivityInfo = await _context.CustomActivityInfos.FindAsync(id);

            if (customActivityInfo == null)
            {
                return NotFound();
            }

            return customActivityInfo;
        }

        // PUT: api/CustomActivityInfos/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize()]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomActivityInfo(int id, CustomActivityInfo customActivityInfo)
        {
            if (id != customActivityInfo.CustomActivityInfoId)
            {
                return BadRequest();
            }

            _context.Entry(customActivityInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomActivityInfoExists(id))
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

        // POST: api/CustomActivityInfos
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize()]
        [HttpPost]
        public async Task<ActionResult<CustomActivityInfo>> PosCustomActivityInfo(CustomActivityInfo customActivityInfo)
        {
            _context.CustomActivityInfos.Add(customActivityInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomActivityInfo", new { id = customActivityInfo.CustomActivityInfoId }, customActivityInfo);
        }

        // DELETE: api/CustomActivityInfos/5
        [Authorize()]
        [HttpDelete("{id}")]
        public async Task<ActionResult<CustomActivityInfo>> DeleteCustomActivityInfo(int id)
        {
            var customActivityInfo = await _context.CustomActivityInfos.FindAsync(id);
            if (customActivityInfo == null)
            {
                return NotFound();
            }

            _context.CustomActivityInfos.Remove(customActivityInfo);
            await _context.SaveChangesAsync();

            return customActivityInfo;
        }

        private bool CustomActivityInfoExists(int id)
        {
            return _context.CustomActivityInfos.Any(e => e.CustomActivityInfoId == id);
        }
    }
}
