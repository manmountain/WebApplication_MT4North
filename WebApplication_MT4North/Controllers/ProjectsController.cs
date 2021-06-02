using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication_MT4North.Models;
using WebApplication_MT4North.Resources;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication_MT4North.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectsController : ControllerBase
    {
        private readonly MT4NorthContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProjectsController(
            ILogger<AccountController> logger,
            MT4NorthContext context,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }


        // GET: api/Projects
        [HttpGet()]
        [Authorize]
        public async Task<ActionResult> GetCurrentUserProjects()
        {
            string userEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user != null)
            {
                // fetch all user-projects where the user is a member
                //var userProjects = await _context.UserProjects.Where(p => p.User.UserName == user.UserName).ToListAsync<UserProject>();
                var userProjects = await _context.UserProjects.Where(p => p.UserId == user.Id).ToListAsync<UserProject>();
                // fetch projects from userProjects
                var projects = new List<Project>();
                foreach (var userProject in userProjects)
                {
                    var project = _context.Projects.FirstOrDefault(p => p.ProjectId == userProject.ProjectId);
                    projects.Add(project);
                }
                // return the projects
                return Ok(projects);
            }
            var errorResult = new ErrorResult();
            errorResult.Message = "User not found";
            return BadRequest(errorResult);
            //return BadRequest(new ErrorResult({ Message = "User not found" });
        }

        // GET: api/Projects/All
        [HttpGet("All")]
        [Authorize(Roles = "AdminUser")]
        public async Task<ActionResult> GetProjects()
        {
            return Ok(await _context.Projects.ToListAsync());
        }

        // GET: api/Projects/{id}
        [Authorize()]
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        // POST: api/Projects/
        [Authorize()]
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(Project project)
        {
            // Fetch current user
            string userEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);

            // Save project
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            
            // Create a userProject with the current user as owner
            // TODO: Kolla om användarna existerar?
            // TODO: Det borde skapas upp UserProject för användaren som skapar upp projektet
            var userProject = new UserProject();
            userProject.Project = project;
            userProject.User = user;
            userProject.Role = "Projektägare";
            userProject.Rights = "RW";
            // Save user-project
            _context.UserProjects.Add(userProject);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostProject", new { project.ProjectId }, project);
        }

        // PUT: api/Projects/{id}
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize()]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            if (id != project.ProjectId)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
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

        // DELETE: api/Projects/{id}
        [Authorize()]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Project>> DeleteProject(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return project;
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}
