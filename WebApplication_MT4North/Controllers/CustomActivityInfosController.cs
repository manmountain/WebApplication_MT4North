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
    public class CustomActivityInfosController : ControllerBase
    {
        private readonly MT4NorthContext _context;
        private readonly ILogger<CustomActivityInfosController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public CustomActivityInfosController(
            ILogger<CustomActivityInfosController> logger,
            MT4NorthContext context,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        // GET: api/CustomActivityInfos/5
        /// <summary>
        /// Get the custom activity with CustomActivityId == id
        /// </summary>
        /// <remarks></remarks>
        /// <returns>
        /// The custom activity with CustomActivityId == id
        /// </returns>
        /// <response code="200">OK</response>
        /// <response code="204">No content</response>
        /// <response code="400">BadRequest</response>
        /// <response code="401">Forbidden</response>
        /// <response code="403">Unautherized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status204NoContent)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
        [Authorize()]
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomActivityInfo>> GetCustomActivityInfo(int id)
        {
            var customActivityInfo = await _context.CustomActivityInfos.FindAsync(id);
            if (customActivityInfo == null)
            {
                return NotFound();
            }

            string userEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return Unauthorized();
            }

            var activity = await _context.Activities.FirstAsync(a => a.CustomActivityInfoId == customActivityInfo.CustomActivityInfoId);
            if (activity == null)
            {
                return NotFound();
            }

            // Check if user got R or RW permissions for the project the activity belongs to
            var userproject = _context.UserProjects.Where(p => p.ProjectId == activity.ProjectId &&
                                                               p.UserId == user.Id && (p.Rights == UserProjectPermissions.READWRITE || p.Rights == UserProjectPermissions.READ)).FirstOrDefault();
            if (userproject == null)
            {
                return Forbid();
            }

            return Ok(customActivityInfo);
        }

        // PUT: api/CustomActivityInfos/5
        /// <summary>
        /// Update custom activity with CustomActivityId == id
        /// </summary>
        /// <remarks></remarks>
        /// <returns>
        /// The updated custom activity with CustomActivityId == id
        /// </returns>
        /// <response code="200">OK</response>
        /// <response code="204">No content</response>
        /// <response code="400">BadRequest</response>
        /// <response code="401">Forbidden</response>
        /// <response code="403">Unautherized</response>
        /// <response code="404">Not Found</response>
        /// <response code="500">Internal Server Error</response>
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status200OK)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status204NoContent)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status403Forbidden)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status404NotFound)]
        [ProducesResponseType(Microsoft.AspNetCore.Http.StatusCodes.Status500InternalServerError)]
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

            string userEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                return Unauthorized();
            }

            var activity = await _context.Activities.FirstAsync(a => a.CustomActivityInfoId == customActivityInfo.CustomActivityInfoId);
            if (activity == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(activity.ProjectId);
            if (project == null)
            {
                return NotFound();
            }

            // Check if user got W or RW permissions for the project the activity belongs to
            var userproject = _context.UserProjects.Where(p => p.ProjectId == project.ProjectId &&
                                                               p.UserId == user.Id && (p.Rights == UserProjectPermissions.READWRITE || p.Rights == UserProjectPermissions.WRITE)).FirstOrDefault();
            if (userproject == null)
            {
                return Forbid();
            }

            _context.Entry(customActivityInfo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(customActivityInfo);
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
        }

        private bool CustomActivityInfoExists(int id)
        {
            return _context.CustomActivityInfos.Any(e => e.CustomActivityInfoId == id);
        }
    }
}
