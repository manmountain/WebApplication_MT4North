using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApplication_MT4North.Models;

namespace WebApplication_MT4North.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly MT4NorthContext _context;
        private readonly ILogger<ActivitiesController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public ActivitiesController(
            ILogger<ActivitiesController> logger,
            MT4NorthContext context,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        // GET  /api/Activities
        // all activitys for user
        [Authorize()]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Activity>>> GetActivities()
        {
            string userEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound();
            }

            // Get all user projects for the user where the user have R or RW permissions
            var userProjects = await _context.UserProjects.Where(p => p.User.UserName == user.UserName && p.Status == UserProjectStatus.Accepted && (p.Rights == "RW" || p.Rights == "R")).ToListAsync<UserProject>();
            // Get all activities from all of the projects the user is a member of
            var activities = new List<Activity>();
            foreach (var userProject in userProjects)
            {
                var projectActivitys = await _context.Activities.Where(a => a.ProjectId == userProject.ProjectId).ToListAsync<Activity>();
                activities.AddRange(projectActivitys);
            }
            // Load Project, CustomActivityInfo, BaseActivityInfo from database
            foreach (var activity in activities)
            {
                var project = await _context.Projects.Where(p => p.ProjectId == activity.ProjectId).ToListAsync<Project>();
                //var customActivities = await _context.CustomActivityInfos.Where(c => c.ActivityId == activity.ActivityId).ToListAsync<CustomActivityInfo>();
                /* System.Text.Json.JsonException:
                 * A possible object cycle was detected which is not supported. This can either be due to a cycle or if the object depth is larger than the maximum allowed depth of 32.
                foreach (var customActivity in customActivities)
                {
                    var themes = await _context.Themes.Where(t => t.ThemeId == customActivity.ThemeId).ToListAsync<Theme>();
                }*/
                //var baseActivity = await _context.BaseActivityInfos.Where(b => b.BaseActivityInfoId == activity.BaseInfoId).ToListAsync<BaseActivityInfo>();

            }
            // Return the activites found
            return activities;
        }

        // GET  /api/Activities/{id}
        // activity with ActivityId == id
        [Authorize()]
        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivity(int id)
        {
            string userEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            // Check if user got R or RW permissions for the project the activity belongs to
            var userproject = await _context.UserProjects.Where(p => p.ProjectId == activity.ProjectId &&
                                                                p.UserId == user.Id && (p.Rights == "RW" || p.Rights == "R")).ToListAsync<UserProject>();
            if (userproject == null)
            {
                return Unauthorized();
            }

            return activity;
        }

        // PUT  /api/Activities/{id}
        // update activity with ActivityId == id
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize()]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutActivity(int id, Activity activity)
        {
            string userEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound();
            }

            if (id != activity.ActivityId)
            {
                return BadRequest();
            }

            // Check if user got W or RW permissions for the project the activity belongs to
            var userproject = await _context.UserProjects.Where(p => p.ProjectId == activity.ProjectId &&
                                                                p.UserId == user.Id && (p.Rights == "RW" || p.Rights == "W")).ToListAsync<UserProject>();
            if (userproject == null)
            {
                return Unauthorized();
            }

            _context.Entry(activity).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActivityExists(id))
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

        // DEL  /api/Activities/{id}
        // delete activity with ActivityId == id
        [Authorize()]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Activity>> DeleteActivity(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            string userEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound();
            }
            // Check if user got W or RW permissions for the project the activity belongs to
            var userproject = await _context.UserProjects.Where(p => p.ProjectId == activity.ProjectId &&
                                                                p.UserId == user.Id && (p.Rights == "RW" || p.Rights == "W")).ToListAsync<UserProject>();
            if (userproject == null)
            {
                return Unauthorized();
            }

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return activity;
        }

        // GET  /api/Activities/Project/{projectId}
        // all activitys for project with ProjectId == projectId
        [Authorize()]
        [HttpGet("Project/{projectId}")]
        public async Task<ActionResult<IEnumerable<Activity>>> GetActivitysForProject(int projectId)
        {
            string userEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return NotFound();
            }

            var userproject = await _context.UserProjects.FirstOrDefaultAsync(p => p.ProjectId == projectId &&
                                                                p.UserId == user.Id && (p.Rights == "RW" || p.Rights == "R"));
            if (userproject == null)
            {
                return Unauthorized();
            }

            var activities = await _context.Activities.Where(a => a.ProjectId == userproject.ProjectId).ToListAsync<Activity>();
            return activities;
        }

        // POST /api/Activities/Project/{projectId}
        // create an activitys for user in project with ProjectId == projectId
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize()]
        [HttpPost("Project/{projectId}")]
        public async Task<ActionResult<Activity>> PostActivity(int projectId, Activity activity)
        {

            //_context.Activities.Add(activity);
            //await _context.SaveChangesAsync();
            // TODO: 


            return CreatedAtAction("GetActivity", new { id = activity.ActivityId }, activity);
        }

        private bool ActivityExists(int id)
        {
            return _context.Activities.Any(e => e.ActivityId == id);
        }
    }
}



// GET  /api/Activities                                             all activitys for user
// GET  /api/Activities/{id}                                        activity with ActivityId == id
// PUT  /api/Activities/{id}                                        update activity with ActivityId == id
// DEL  /api/Activities/{id}                                        delete activity with ActivityId == id
// GET  /api/Activities/Project/{projectId}                         all activitys for project with ProjectId == projectId
// POST /api/Activities/Project/{projectId}/customActivity/{id}     create an activitys for user in project with ProjectId == projectId
// POST /api/Activities/Project/{projectId}/baseActivity/{id}       create an activitys for user in project with ProjectId == projectId
