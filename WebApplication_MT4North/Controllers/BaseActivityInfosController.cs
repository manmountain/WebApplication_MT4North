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
    public class BaseActivityInfosController : ControllerBase
    {
        private readonly MT4NorthContext _context;

        public BaseActivityInfosController(MT4NorthContext context)
        {
            _context = context;
        }

        // GET: api/BaseActivityInfos
        [Authorize()]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BaseActivityInfo>>> GetBaseActivityInfos()
        {
            var baseActivityInfos = await _context.BaseActivityInfos.ToListAsync();
            return baseActivityInfos;
        }

        // GET: api/BaseActivityInfos/5
        [Authorize()]
        [HttpGet("{id}")]
        public async Task<ActionResult<BaseActivityInfo>> GetBaseActivityInfo(int id)
        {
            var baseActivityInfo = await _context.BaseActivityInfos.FindAsync(id);

            if (baseActivityInfo == null)
            {
                return NotFound();
            }

            return baseActivityInfo;
        }

        // PUT: api/BaseActivityInfos/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize(Roles = "AdminUser")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBaseActivityInfo(int id, BaseActivityInfo baseActivityInfo)
        {
            if (id != baseActivityInfo.BaseActivityInfoId)
            {
                return BadRequest();
            }

            _context.Entry(baseActivityInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BaseActivityInfoExists(id))
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

        // POST: api/BaseActivityInfos
        // To protect from overposting attacks, enable the specific properties you want to bind to, for'
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize(Roles = "AdminUser")]
        [HttpPost]
        public async Task<ActionResult<BaseActivityInfo>> PosBaseActivityInfo(BaseActivityInfo baseActivityInfo)
        {
            _context.BaseActivityInfos.Add(baseActivityInfo);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBaseActivityInfo", new { id = baseActivityInfo.BaseActivityInfoId }, baseActivityInfo);
        }

        // DELETE: api/BaseActivityInfos/5
        [Authorize(Roles = "AdminUser")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<BaseActivityInfo>> DeleteBaseActivityInfo(int id)
        {
            var baseActivityInfo = await _context.BaseActivityInfos.FindAsync(id);
            if (baseActivityInfo == null)
            {
                return NotFound();
            }

            _context.BaseActivityInfos.Remove(baseActivityInfo);
            await _context.SaveChangesAsync();

            return baseActivityInfo;
        }

        private bool BaseActivityInfoExists(int id)
        {
            return _context.BaseActivityInfos.Any(e => e.BaseActivityInfoId == id);
        }
    }
}
