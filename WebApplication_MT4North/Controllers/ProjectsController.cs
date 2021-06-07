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
        /// <summary>
        /// Get projects for current user
        /// </summary>
        /// <remarks></remarks>
        /// <returns>
        /// User's Projects
        /// </returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unautherized</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
        [HttpGet()]
        [Authorize]
        public async Task<ActionResult> GetCurrentUserProjects()
        {
            string userEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            
            // fetch all user-projects where the user is a member
            //var userProjects = await _context.UserProjects.Where(p => p.User.UserName == user.UserName).ToListAsync<UserProject>();
            var userProjects = await _context.UserProjects.Where(p => p.UserId == user.Id && p.Status == UserProjectStatus.Accepted).ToListAsync<UserProject>();
            // fetch projects from userProjects
            var projects = new List<Project>();
            foreach (var userProject in userProjects)
            {
                var project = _context.Projects.FirstOrDefault(p => p.ProjectId == userProject.ProjectId);
                projects.Add(project);
            }
            // return projects
            return Ok(projects);
        }

        /*
        // GET: api/Projects/All
        [HttpGet("All")]
        [Authorize(Roles = "AdminUser")]
        public async Task<ActionResult> GetProjects()
        {
            return Ok(await _context.Projects.ToListAsync());
        }*/

        // GET: api/Projects/{id}
        /// <summary>
        /// Get Project with id
        /// </summary>
        /// <remarks></remarks>
        /// <returns>
        /// Updated UserProject
        /// </returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unautherized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
        [Authorize()]
        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(int id)
        {
            // Fetch current user
            string userEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            // Get the project
            var project = await _context.Projects.FindAsync(id);

            var callerUserProject = await _context.UserProjects.FirstOrDefaultAsync<UserProject>(p => p.ProjectId == project.ProjectId && p.UserId == user.Id && (p.Rights == "RW" || p.Rights == "R"));
            if (callerUserProject == null)
            {
                // The user doesnt have READ rights to this project
                return Unauthorized();
            }

            if (project == null)
            {
                return NotFound();
            }

            return Ok(project);
        }

        // POST: api/Projects/
        /// <summary>
        /// Create a Project
        /// </summary>
        /// <remarks></remarks>
        /// <returns>
        /// Created Project
        /// </returns>
        /// <response code="201">Created</response>
        /// <response code="401">Unautherized</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status201Created)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
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
            var userProject = new UserProject();
            userProject.Project = project;
            userProject.User = user;
            userProject.Role = "Projektägare";
            userProject.Rights = "RW";
            userProject.Status = UserProjectStatus.Accepted;
            // Save user-project
            _context.UserProjects.Add(userProject);
            await _context.SaveChangesAsync();

            return CreatedAtAction("PostProject", new { project.ProjectId }, project);
        }

        // PUT: api/Projects/{id}
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// Update a Project
        /// </summary>
        /// <remarks></remarks>
        /// <returns>
        /// Updated Project
        /// </returns>
        /// <response code="204">OK No content</response>
        /// <response code="401">Unautherized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status204NoContent)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
        [Authorize()]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            // Check if the caller got the WRITE rights! Otherwise return Unauthorized
            string callerEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var caller = await _userManager.FindByEmailAsync(callerEmail);
            var callerUserProject = await _context.UserProjects.FirstOrDefaultAsync<UserProject>(p => p.ProjectId == project.ProjectId && p.UserId == caller.Id && (p.Rights == "RW" || p.Rights == "W"));
            if (callerUserProject == null)
            {
                // The caller doesnt have WRITE rights to this project
                return Unauthorized();
            }

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
        /// <summary>
        /// Delete a Project 
        /// </summary>
        /// <remarks></remarks>
        /// <returns>
        /// Deleted Project
        /// </returns>
        /// <response code="200">OK</response>
        /// <response code="401">Unautherized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
        [Authorize()]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Project>> DeleteProject(int id)
        {
            // Check if the caller got the WRITE rights! Otherwise return Unauthorized
            string callerEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var caller = await _userManager.FindByEmailAsync(callerEmail);
            var callerUserProject = await _context.UserProjects.FirstOrDefaultAsync<UserProject>(p => p.ProjectId == id && p.UserId == caller.Id && (p.Rights == "RW" || p.Rights == "W"));
            if (callerUserProject == null)
            {
                // The caller doesnt have WRITE rights to this project
                return Unauthorized();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            // Remove UserProjects for this project
            var userprojects = await _context.UserProjects.FirstOrDefaultAsync<UserProject>(p => p.ProjectId == id);
            _context.UserProjects.RemoveRange(userprojects);
            // Remove project
            _context.Projects.Remove(project);
            // Apply changes to database
            await _context.SaveChangesAsync();

            return Ok(project);
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
        }
    }
}
