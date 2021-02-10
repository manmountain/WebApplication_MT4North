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
    public class RegisteredUserProjectsController : ControllerBase
    {
        private readonly CUSERSKAJO59SOURCEREPOSWEBAPPLICATION_MT4NORTHWEBAPPLICATION_MT4NORTHMODELSMEDTECHINNOVATIONMODEL_V2MDFContext _context;

        public RegisteredUserProjectsController(CUSERSKAJO59SOURCEREPOSWEBAPPLICATION_MT4NORTHWEBAPPLICATION_MT4NORTHMODELSMEDTECHINNOVATIONMODEL_V2MDFContext context)
        {
            _context = context;
        }

        // GET: api/RegisteredUserProjects
        [Route("~/api/GetAllRegisteredUserProjects")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RegisteredUserProject>>> GetRegisteredUserProjects()
        {
            return await _context.RegisteredUserProjects.ToListAsync();
        }

        // GET: api/RegisteredUserProjects/5
        [Route("~/api/GetRegisteredUserProject")]
        [HttpGet("{id}")]
        public async Task<ActionResult<RegisteredUserProject>> GetRegisteredUserProject(int id)
        {
            var registeredUserProject = await _context.RegisteredUserProjects.FindAsync(id);

            if (registeredUserProject == null)
            {
                return NotFound();
            }

            return registeredUserProject;
        }

        // PUT: api/RegisteredUserProjects/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Route("~/api/UpdateRegisteredUserProject")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRegisteredUserProject(int id, RegisteredUserProject registeredUserProject)
        {
            if (id != registeredUserProject.RegisteredUserProjectId)
            {
                return BadRequest();
            }

            _context.Entry(registeredUserProject).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegisteredUserProjectExists(id))
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

        // POST: api/RegisteredUserProjects
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Route("~/api/AddRegisteredUserProject")]
        [HttpPost]
        public async Task<ActionResult<RegisteredUserProject>> PostRegisteredUserProject(RegisteredUserProject registeredUserProject)
        {
            _context.RegisteredUserProjects.Add(registeredUserProject);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRegisteredUserProject", new { id = registeredUserProject.RegisteredUserProjectId }, registeredUserProject);
        }

        // DELETE: api/RegisteredUserProjects/5
        [Route("~/api/DeleteRegisteredUserProject")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<RegisteredUserProject>> DeleteRegisteredUserProject(int id)
        {
            var registeredUserProject = await _context.RegisteredUserProjects.FindAsync(id);
            if (registeredUserProject == null)
            {
                return NotFound();
            }

            _context.RegisteredUserProjects.Remove(registeredUserProject);
            await _context.SaveChangesAsync();

            return registeredUserProject;
        }

        private bool RegisteredUserProjectExists(int id)
        {
            return _context.RegisteredUserProjects.Any(e => e.RegisteredUserProjectId == id);
        }
    }
}
