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

            var userProjects = await _context.UserProjects.Where(p => p.User.UserName == user.UserName && p.Status == UserProjectStatus.Accepted).ToListAsync<UserProject>();




            var activities = await _context.Activities.ToListAsync();
            foreach(var activity in activities)
            {
                _context.CustomActivityInfos.Where(c => c.ActivityId == activity.ActivityId);
            }
            return activities;
        }

        // GET  /api/Activities/{id}
        // activity with ActivityId == id
        [Authorize()]
        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> GetActivity(int id)
        {
            var activity = await _context.Activities.FindAsync(id);

            if (activity == null)
            {
                return NotFound();
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
            if (id != activity.ActivityId)
            {
                return BadRequest();
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

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return activity;
        }

        // GET  /api/Activities/Project/{projectId}
        // all activitys for project with ProjectId == projectId
        [Authorize()]
        [HttpGet("Project/{projectId}")]
        public async Task<ActionResult<Activity>> GetActivitysForProject(int projectId)
        {
            var id = 0;
            var activity = await _context.Activities.FindAsync(id);

            if (activity == null)
            {
                return NotFound();
            }

            return activity;
        }

        // POST /api/Activities/Project/{projectId}
        // create an activitys for user in project with ProjectId == projectId
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [Authorize()]
        [HttpPost("Project/{projectId}")]
        public async Task<ActionResult<Activity>> PostActivity(Activity activity)
        {

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetActivity", new { id = activity.ActivityId }, activity);
        }

        private bool ActivityExists(int id)
        {
            return _context.Activities.Any(e => e.ActivityId == id);
        }
    }
}



// GET  /api/Activities                                       all activitys for user
// GET  /api/Activities/{id}                                  activity with ActivityId == id
// PUT  /api/Activities/{id}                                  update activity with ActivityId == id
// DEL  /api/Activities/{id}                                  delete activity with ActivityId == id
// GET  /api/Activities/Project/{projectId}                   all activitys for project with ProjectId == projectId
// POST /api/Activities/Project/{projectId}                   create an activitys for user in project with ProjectId == projectId