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
                // fetch sub-activity and project
                var project = await _context.Projects.Where(p => p.ProjectId == activity.ProjectId).ToListAsync<Project>();
                if (activity.BaseActivityInfoId != null)
                {
                    var baseactivity = await _context.BaseActivityInfos.FirstOrDefaultAsync(b => b.BaseActivityInfoId == activity.BaseActivityInfoId);
                    var theme = await _context.Themes.FirstOrDefaultAsync(t => t.ThemeId == activity.BaseActivityInfo.ThemeId);
                }
                if (activity.CustomActivityInfoId != null)
                {
                    var customactivity = await _context.CustomActivityInfos.FirstOrDefaultAsync(b => b.CustomActivityInfoId == activity.CustomActivityInfoId);
                    var theme = await _context.Themes.FirstOrDefaultAsync(t => t.ThemeId == activity.CustomActivityInfo.ThemeId);
                }

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
            if (userproject.Count == 0)
            {
                return Unauthorized();
            }

            // fetch base or custom -activity and project
            var project = await _context.Projects.Where(p => p.ProjectId == activity.ProjectId).ToListAsync<Project>();
            if (activity.BaseActivityInfoId != null)
            {
                var baseactivity = await _context.BaseActivityInfos.FirstOrDefaultAsync(b => b.BaseActivityInfoId == activity.BaseActivityInfoId);
                var theme = await _context.Themes.FirstOrDefaultAsync(t => t.ThemeId == activity.BaseActivityInfo.ThemeId);
            }
            if (activity.CustomActivityInfoId != null)
            {
                var customactivity = await _context.CustomActivityInfos.FirstOrDefaultAsync(b => b.CustomActivityInfoId == activity.CustomActivityInfoId);
                var theme = await _context.Themes.FirstOrDefaultAsync(t => t.ThemeId == activity.CustomActivityInfo.ThemeId);
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

                Ok(activity);
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

        // POST /api/Activities/
        // create an activitys for user in project with ProjectId == projectId
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize()]
        [HttpPost("")]
        public async Task<ActionResult<Activity>> PostActivity(Activity activity)
        {
            // Check input data
            if (activity.BaseActivityInfoId != null && activity.CustomActivityInfoId != null)
            {
                return BadRequest(); // cant have both BaseActivity AND CustomActivity
            }

            if (activity.BaseActivityInfoId == null && activity.CustomActivityInfoId == null)
            {
                return BadRequest(); // We must have a BaseActivity OR CustomActivity
            }

            if (activity.BaseActivityInfo != null && activity.BaseActivityInfoId != activity.BaseActivityInfo.BaseActivityInfoId)
            {
                return BadRequest(); // BaseInfoActivityInfo id's dont match
            }

            if (activity.ProjectId == null /*|| activity.ProjectId != activity.Project.ProjectId*/)
            {
                return BadRequest(); // Projects is null or id's dont match
            }

            // Check that the base activity exixts if attached
            if (activity.BaseActivityInfo != null)
            {
                if (!BaseActivityInfoExists((int)activity.BaseActivityInfoId))
                {
                    return NotFound(); // We didnt find the base activity
                }
            }

            // Check that the project exists
            if (!ProjectExists((int)activity.ProjectId))
            {
                return NotFound(); // We didnt find the base activity
            }


            // Make sure we dont have any notes
            if (activity.Notes.Count != 0)
            {
                return BadRequest();
            }

            // Check permissions
            // Check if the caller got the RW rights! Otherwise return Forbidden
            string callerEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var caller = await _userManager.FindByEmailAsync(callerEmail);
            var callerUserProject = await _context.UserProjects.FirstOrDefaultAsync<UserProject>(p => p.ProjectId == activity.ProjectId && p.UserId == caller.Id && (p.Rights == "RW" || p.Rights == "W")); // TODO Enum non R-read RW-readwrite
            if (callerUserProject == null)
            {
                // The caller doesnt have WRITE rights to this project
                return Forbid();
            }

            if (activity.BaseActivityInfoId != null)
            {
                activity.BaseActivityInfo = null;
            }

            // If we got a CustomActivityInfo attached to the activity. Add it to the database
            if (activity.CustomActivityInfo != null)
            {
                activity.CustomActivityInfo.Theme = null;
                _context.CustomActivityInfos.Add(activity.CustomActivityInfo);
                await _context.SaveChangesAsync();

                activity.CustomActivityInfoId = activity.CustomActivityInfo.CustomActivityInfoId;
            }
            // Add the activity to the database
            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            // fetch sub-activity and theme
            var project = await _context.Projects.Where(p => p.ProjectId == activity.ProjectId).ToListAsync<Project>();
            if (activity.BaseActivityInfoId != null)
            {
                var baseactivity = await _context.BaseActivityInfos.FirstOrDefaultAsync(b => b.BaseActivityInfoId == activity.BaseActivityInfoId);
                var theme = await _context.Themes.FirstOrDefaultAsync(t => t.ThemeId == activity.BaseActivityInfo.ThemeId);
            }
            if (activity.CustomActivityInfoId != null)
            {
                var customactivity = await _context.CustomActivityInfos.FirstOrDefaultAsync(b => b.CustomActivityInfoId == activity.CustomActivityInfoId);
                var theme = await _context.Themes.FirstOrDefaultAsync(t => t.ThemeId == activity.CustomActivityInfo.ThemeId);
            }

            return CreatedAtAction("GetActivity", new { id = activity.ActivityId }, activity);
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

            // Remove custom activity info if we got one
            if (activity.CustomActivityInfoId != null)
            {
                _context.CustomActivityInfos.Remove(activity.CustomActivityInfo);
            }
            if (activity.Notes.Count > 0)
            {
                _context.Notes.RemoveRange(activity.Notes); 
            }
            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return Ok(activity);
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

            // fetch the project from the database
            var project = await _context.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectId);
            var activities = await _context.Activities.Where(a => a.ProjectId == userproject.ProjectId).ToListAsync<Activity>();
            foreach(var activity in activities)
            {
                // fetch base or custom -activity and theme
                if (activity.BaseActivityInfoId != null)
                {
                    var baseactivity = await _context.BaseActivityInfos.FirstOrDefaultAsync(b => b.BaseActivityInfoId == activity.BaseActivityInfoId);
                    var theme = await _context.Themes.FirstOrDefaultAsync(t => t.ThemeId == activity.BaseActivityInfo.ThemeId);
                }
                if (activity.CustomActivityInfoId != null)
                {
                    var customactivity = await _context.CustomActivityInfos.FirstOrDefaultAsync(b => b.CustomActivityInfoId == activity.CustomActivityInfoId);
                    var theme = await _context.Themes.FirstOrDefaultAsync(t => t.ThemeId == activity.CustomActivityInfo.ThemeId);
                }
            }
            return activities;
        }

        /*
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
        }*/

        private bool ActivityExists(int id)
        {
            return _context.Activities.Any(e => e.ActivityId == id);
        }

        private bool BaseActivityInfoExists(int id)
        {
            return _context.BaseActivityInfos.Any(e => e.BaseActivityInfoId == id);
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.ProjectId == id);
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
