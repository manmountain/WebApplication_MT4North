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
    public class UserProjectsController : ControllerBase
    {
        private readonly MT4NorthContext _context;
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProjectsController(
            ILogger<AccountController> logger,
            MT4NorthContext context,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }


        // GET: api/UserProjects
        [HttpGet("")]
        [Authorize]
        public async Task<ActionResult> GetCurrentUserProjects()
        {
            string userEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user != null)
            {
                // fetch all user-projects where the user is a member
                var userProjects = await _context.UserProjects.Where(p => p.User.UserName == user.UserName).ToListAsync<UserProject>();
                foreach(var userProject in userProjects)
                {
                    _context.Projects.FirstOrDefault(p => p.ProjectId == userProject.ProjectId);
                    //_context.Users.FirstOrDefault(u => u.Id == userProject.User.Id);
                }
                return Ok(userProjects);
            }
            return NotFound();
        }

        [HttpGet("All")]
        [Authorize(Roles="AdminUser")]
        public async Task<ActionResult> GetAllProjects()
        {
            var userProjects = await _context.UserProjects.ToListAsync<UserProject>();
            foreach (var userProject in userProjects)
            {
                _context.Projects.FirstOrDefault(p => p.ProjectId == userProject.ProjectId);
            }
            return Ok(userProjects);
        }

        [Authorize()]
        [HttpGet("Project/{projectId}")]
        public async Task<ActionResult> GetAllForProject(int projectId)
        {
            // fetch all user-projects for project with id projectId 
            var userProjects = await _context.UserProjects.Where(p => p.Project.ProjectId == projectId).ToListAsync<UserProject>();
            foreach (var userProject in userProjects)
            {
                await _context.Projects.Where(p => p.ProjectId == userProject.ProjectId).ToListAsync<Project>();
                await _context.Users.Where(u => u.Id == userProject.UserId).ToListAsync<ApplicationUser>();
            }

            // return found user-projects
            return Ok(userProjects);
        }

        // DELETE: api/UserProjects/{id}
        [Authorize()]
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserProject>> DeleteUserProject(int id)
        {
            var userProject = await _context.UserProjects.FindAsync(id);
            if (userProject == null)
            {
                return NotFound();
            }

            /*
            // check so that userProjects corresponds with the user trying to delete it
            string userEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (userProject.User.UserName != user.UserName)
            {
                return Unauthorized();
            }

            //TODO: FIXME: 
            // Should probably check permission and such also
            if (!userProject.Rights.Equals("RW"))
            {
                return Unauthorized();
            }
            _context.Projects.Remove(userProject.Project);*/
            _context.UserProjects.Remove(userProject);
            await _context.SaveChangesAsync();

            return userProject;
        }
    }
}
