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
    public class RegisteredUsersController : ControllerBase
    {
        private readonly MT4NorthContext _context;

        public RegisteredUsersController(MT4NorthContext context)
        {
            _context = context;
        }

        // GET: api/RegisteredUsers
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegisteredUser>>> GetRegisteredUsers()
        {
            return await _context.RegisteredUsers.ToListAsync();
        }

        // GET: api/RegisteredUsers/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RegisteredUser>> GetRegisteredUser(int id)
        {
            var registeredUser = await _context.RegisteredUsers.FindAsync(id);

            if (registeredUser == null)
            {
                return NotFound();
            }

            return registeredUser;
        }

        // PUT: api/RegisteredUsers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegisteredUser(int id, RegisteredUser registeredUser)
        {
            if (id != registeredUser.UserId)
            {
                return BadRequest();
            }

            _context.Entry(registeredUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegisteredUserExists(id))
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

        // POST: api/RegisteredUsers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<RegisteredUser>> PostRegisteredUser(RegisteredUser registeredUser)
        {
            _context.RegisteredUsers.Add(registeredUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRegisteredUser", new { id = registeredUser.UserId }, registeredUser);
        }

        // DELETE: api/RegisteredUsers/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<RegisteredUser>> DeleteRegisteredUser(int id)
        {
            var registeredUser = await _context.RegisteredUsers.FindAsync(id);
            if (registeredUser == null)
            {
                return NotFound();
            }

            _context.RegisteredUsers.Remove(registeredUser);
            await _context.SaveChangesAsync();

            return registeredUser;
        }

        private bool RegisteredUserExists(int id)
        {
            return _context.RegisteredUsers.Any(e => e.UserId == id);
        }
    }
}
