using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WebApplication_MT4North.Infrastructure;
using WebApplication_MT4North.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using WebApplication_MT4North.Resources;
using System.Linq;
using System.Collections.Generic;
using WebApplication_MT4North.Models;

namespace WebApplication_MT4North.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        //private readonly IUserService _userService;
        private readonly IJwtAuthManager _jwtAuthManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(
            ILogger<AccountController> logger,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IJwtAuthManager jwtAuthManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtAuthManager = jwtAuthManager;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return BadRequest( new ErrorResult()
                {
                    Message = "Email already in use",
                    Errors = new List<string>() {"Email already in use"}
                });
            }

            var username = request.Email;
            var newUser = new ApplicationUser()
            {
                Email = request.Email,
                UserName = username,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Gender = request.Gender
            };
            var userCreated = await _userManager.CreateAsync(newUser, request.Password);
            if (!userCreated.Succeeded)
            {
                return BadRequest(new ErrorResult
                {
                    Message = "Error creating user with email: " + request.Email + " and username: " + request.UserName,
                    Errors = userCreated.Errors.Select(x => x.Description).ToList()
            });
            }

            var roleResult = await _userManager.AddToRoleAsync(newUser, "BasicUser");
            var roles = await _userManager.GetRolesAsync(newUser);
            if (userCreated.Succeeded && roleResult.Succeeded)
            {
                return Ok(new RegisterResult
                {
                    UserName = request.UserName,
                    Role = roles,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                });
            }

            return BadRequest(new ErrorResult
            {
                Message = "Error creating user with email: "+request.Email+" and username: "+request.UserName,
                Errors = roleResult.Errors.Select(x => x.Description).ToList()
            });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser == null)
            {
                return BadRequest(new ErrorResult()
                {
                    Message = "BadRequest. Cant find user with email: " + request.Email,
                    Errors = new List<string>()
                });
            }

            var isCorrect = await _userManager.CheckPasswordAsync(existingUser, request.Password);
            if (!isCorrect)
            {
                return Unauthorized(new ErrorResult
                {
                    Message = "Unauthorized. Wrong email or password " + request.Email,
                    Errors = new List<string>()
                });

            }

            var roles = await _userManager.GetRolesAsync(existingUser);
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name,  request.Email));
            claims.Add(new Claim(ClaimTypes.Email, request.Email));
            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwtResult = _jwtAuthManager.GenerateTokens(request.Email, claims.ToArray<Claim>(), DateTime.Now);
            _logger.LogInformation($"User [{request.Email}] logged in the system.");
            var img = (existingUser.Gender == "Kvinna") ? ("") : ("");
            return Ok(new LoginResult
            {
                // TODO: Only return AccessToken and RefreshToken
                /*UserName = existingUser.UserName,
                Email = existingUser.Email,
                FirstName = existingUser.FirstName,
                LastName = existingUser.LastName,
                Gender = existingUser.Gender,
                CompanyName = existingUser.CompanyName,
                Country = existingUser.Country,
                ProfilePicture = existingUser.ProfilePicture,*/
                Roles = roles.ToList<string>(),
                //UsertYPE & uSERrOLE ? TODO:
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public ActionResult Logout()
        {
            // TODO: ???
            // optionally "revoke" JWT token on the server side --> add the current token to a block-list
            // https://github.com/auth0/node-jsonwebtoken/issues/375

            var userName = User.Identity?.Name;
            _jwtAuthManager.RemoveRefreshTokenByUserName(userName);
            _logger.LogInformation($"User [{userName}] logged out the system.");
            return Ok();
        }

        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                // Fetch current user
                string userEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
                var user = await _userManager.FindByEmailAsync(userEmail);
                //
                var userName = User.Identity?.Name;
                _logger.LogInformation($"User [{userName}] is trying to refresh JWT token.");

                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return Unauthorized();
                }
                var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
                var jwtResult = _jwtAuthManager.Refresh(request.RefreshToken, accessToken, DateTime.Now);
                _logger.LogInformation($"User [{userName}] has refreshed JWT token.");

                var roles = await _userManager.GetRolesAsync(user);
                return Ok(new LoginResult
                {
                    //UserName = userName,
                    Roles = roles.ToList<string>(),
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken.TokenString
                });
            }
            catch (SecurityTokenException e)
            {
                return Unauthorized(e.Message); // return 401 so that the client side can redirect the user to login page
            }
        }

        // GET: api/Account/User
        [HttpGet("user")]
        [Authorize]
        public async Task<ActionResult<ApplicationUser>> GetCurrentUserAsync()
        {
            string userEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            var roles = await _userManager.GetRolesAsync(user);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPut("user")]
        [Authorize]
        public async Task<ActionResult> UpdateCurrentUserAsync([FromBody] UserRequest request)
        {
            string userEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            var roles = await _userManager.GetRolesAsync(user);
            
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                //TODO: Can a user A change email to the same email as another user? ...
                //var result = await _userManager.ChangeEmailAsync(user, request.Email);
                user.Email = request.Email;
                user.UserName = request.Email;
            }
            if (!string.IsNullOrWhiteSpace(request.FirstName))
            {
                user.FirstName = request.FirstName;
            }
            if (!string.IsNullOrWhiteSpace(request.LastName))
            {
                user.LastName = request.LastName;
            }
            if (!string.IsNullOrWhiteSpace(request.Gender))
            {
                user.Gender = request.Gender;
            }
            //
            var updateResult = await _userManager.UpdateAsync(user);

            if (updateResult.Succeeded)
            {
                return Ok(user);
            }

            var errors = updateResult.Errors.Select(x => x.Description).ToList();
            return BadRequest(new ErrorResult
            {
                Message = "Error updating user with email: " + user.Email + " using request: " + request.ToString(),
                Errors = errors
            });
        }

        [HttpPut("user/{userEmail}")]
        [Authorize(Roles = "AdminUser")]
        public async Task<ActionResult> UpdateUserAsync(string userEmail, [FromBody] UserRequest request)
        {
            var user = await _userManager.FindByEmailAsync(userEmail);

            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                user.Email = request.Email;
                user.UserName = request.Email;
            }
            if (!string.IsNullOrWhiteSpace(request.Email))
            {
                user.Email = request.Email;
            }
            if (!string.IsNullOrWhiteSpace(request.FirstName))
            {
                user.FirstName = request.FirstName;
            }
            if (!string.IsNullOrWhiteSpace(request.LastName))
            {
                user.LastName = request.LastName;
            }
            if (!string.IsNullOrWhiteSpace(request.Gender))
            {
                user.Gender = request.Gender;
            }
            //
            var updateResult = await _userManager.UpdateAsync(user);

            if (updateResult.Succeeded)
            {
                return Ok(updateResult);
            }

            var errors = updateResult.Errors.Select(x => x.Description).ToList();
            return BadRequest(new ErrorResult
            {
                Message = "Error updating user with email: " + user.Email + " using request: " + request.ToString(),
                Errors = errors
            });
        }

        [HttpPut("user/password")]
        [Authorize]
        public async Task<ActionResult> UpdateUserPassword([FromBody] UpdatePasswordRequest request)
        {
            string userEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            var user = await _userManager.FindByEmailAsync(userEmail);
            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword); 
            if (result.Succeeded)
            {
                return Ok(new StatusResult
                {
                    Message = "Password updated"
                });
            }

            var errors = result.Errors.Select(x => x.Description).ToList();
            return BadRequest(new ErrorResult
            {
                Message = "Error updating password",
                Errors = errors
            });
        }

        [HttpDelete("user")]
        public async Task<ActionResult> DeleteUserAsync()
        {
            string userEmail = ((ClaimsIdentity)User.Identity).Claims.Where(c => c.Type == ClaimTypes.Email).FirstOrDefault().Value;
            if (string.IsNullOrWhiteSpace(userEmail))
            {
                return BadRequest(new ErrorResult
                {
                    Message = "Email cant be empty",
                    Errors = new List<string>()
                });
            }
            var user = await _userManager.FindByEmailAsync(userEmail);
            if (user == null)
            {
                //return BadRequest("Can't find user to delete");
                return BadRequest(new ErrorResult
                {
                    Message = "Can't find user with email: " + userEmail + " to delete",
                    Errors = new List<string>()
                });
            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if (deleteResult.Succeeded)
            {
                return Ok(new StatusResult
                {
                    Message = "User " + userEmail + " deleted"
                });
            }
            else
            {
                var errors = deleteResult.Errors.Select(x => x.Description).ToList();
                return BadRequest(new ErrorResult
                {
                    Message = "Error deleting user with email: " + userEmail,
                    Errors = errors
                });
            }
        }

        [HttpDelete("user/{userEmail}")]
        [Authorize(Roles = "AdminUser")]
        public async Task<ActionResult> DeleteUserByEmailAsync(string userEmail)
        {
            var email = userEmail;
            if(string.IsNullOrWhiteSpace(email))
            {
                return BadRequest(new ErrorResult
                {
                    Message = "Email cant be empty",
                    Errors = new List<string>()
                });
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                //return BadRequest("Can't find user to delete");
                return BadRequest(new ErrorResult
                {
                    Message = "Can't find user with email: " + userEmail + " to delete",
                    Errors = new List<string>()
                });
            }
            var deleteResult = await _userManager.DeleteAsync(user);
            if (deleteResult.Succeeded)
            {
                return Ok(new StatusResult
                {
                    Message = "User " + userEmail + " deleted"
                });
            } else
            {
                var errors = deleteResult.Errors.Select(x => x.Description).ToList();
                return BadRequest(new ErrorResult
                {
                    Message = "Error deleting user with email: " + userEmail,
                    Errors = errors
                });
            }
        }

        [HttpGet("roles")]
        [Authorize]
        //[AllowAnonymous]
        public async Task<IActionResult> GetRoles()
        {
            return Ok(new RolesResult
            {
                Roles = _roleManager.Roles.Select(x => x.Name).ToList()
            });
        }

        [HttpPost("roles/{roleName}")]
        [Authorize(Roles = "AdminUser")]
        //[AllowAnonymous]
        public async Task<IActionResult> CreateRole(string roleName/*[FromBody] RoleRequest request*/)
        {
            // FIXME: Can it happen?! :/
            if (string.IsNullOrWhiteSpace(roleName/*request.RoleName*/))
            {
                return BadRequest("Role name should be provided.");
            }

            var newRole = new IdentityRole
            {
                Name = roleName //request.RoleName
            };
            var roleResult = await _roleManager.CreateAsync(newRole);

            if (roleResult.Succeeded)
            {
                //return Ok();
                return Ok(new StatusResult
                {
                    Message = "Role " + roleName/*request.RoleName*/ + " created"
                });
            }
            return Problem(roleResult.Errors.First().Description, null, 500);
        }

        [HttpDelete("roles/{roleName}")]
        [Authorize(Roles = "AdminUser")]
        //[AllowAnonymous]
        public async Task<IActionResult> DeleteRole(string roleName/*[FromBody] RoleRequest request*/)
        {
            // FIXME: Can it happen?! :/
            if (string.IsNullOrWhiteSpace(/*request.RoleName*/roleName))
            {
                return BadRequest("Role name should be provided.");
            }

            var role = _roleManager.Roles.SingleOrDefault(r => r.NormalizedName == /*request.RoleName*/roleName.ToUpper());
            var roleResult = await _roleManager.DeleteAsync(role);

            if (roleResult.Succeeded)
            {
                //return Ok();
                return Ok(new StatusResult
                {
                    Message = "Role " + roleName/*request.RoleName*/ + " deleted"
                });
            }

            return Problem(roleResult.Errors.First().Description, null, 500);
        }

        [HttpPost("roles/{userEmail}/{roleName}")]
        [Authorize(Roles = "AdminUser")]
        //[AllowAnonymous]
        public async Task<IActionResult> AddUserToRole(string userEmail, string roleName)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.UserName == userEmail);
            var result = await _userManager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                //return Ok();
                return Ok(new StatusResult
                {
                    Message = "User " + userEmail + " added to role " + roleName
                });
            }

            return Problem(result.Errors.First().Description, null, 500);
        }

        [HttpDelete("roles/{userEmail}/{roleName}")]
        [Authorize(Roles = "AdminUser")]
        //[AllowAnonymous]
        public async Task<IActionResult> RemoveUserFromRole(string userEmail, string roleName)
        {
            var user = _userManager.Users.SingleOrDefault(u => u.UserName == userEmail);
            var result = await _userManager.RemoveFromRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                //return Ok();
                return Ok(new StatusResult
                {
                    Message = "User " + userEmail + " removed to role "+roleName
                });
            }

            return Problem(result.Errors.First().Description, null, 500);
        }

        /*
        [HttpPost("impersonation")]
        [Authorize(Roles = UserRoles.Admin)]
        public ActionResult Impersonate([FromBody] ImpersonationRequest request)
        {
            var userName = User.Identity?.Name;
            _logger.LogInformation($"User [{userName}] is trying to impersonate [{request.UserName}].");

            var impersonatedRole = _userService.GetUserRole(request.UserName);
            if (string.IsNullOrWhiteSpace(impersonatedRole))
            {
                _logger.LogInformation($"User [{userName}] failed to impersonate [{request.UserName}] due to the target user not found.");
                return BadRequest($"The target user [{request.UserName}] is not found.");
            }
            if (impersonatedRole == UserRoles.Admin)
            {
                _logger.LogInformation($"User [{userName}] is not allowed to impersonate another Admin.");
                return BadRequest("This action is not supported.");
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.Name,request.UserName),
                new Claim(ClaimTypes.Role, impersonatedRole),
                new Claim("OriginalUserName", userName ?? string.Empty)
            };

            var jwtResult = _jwtAuthManager.GenerateTokens(request.UserName, claims, DateTime.Now);
            _logger.LogInformation($"User [{request.UserName}] is impersonating [{request.UserName}] in the system.");
            return Ok(new LoginResult
            {
                UserName = request.UserName,
                //Role = impersonatedRole,
                OriginalUserName = userName,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            });
        }*/

        /*
        [HttpPost("stop-impersonation")]
        public ActionResult StopImpersonation()
        {
            var userName = User.Identity?.Name;
            var originalUserName = User.FindFirst("OriginalUserName")?.Value;
            if (string.IsNullOrWhiteSpace(originalUserName))
            {
                return BadRequest("You are not impersonating anyone.");
            }
            _logger.LogInformation($"User [{originalUserName}] is trying to stop impersonate [{userName}].");

            var role = _userService.GetUserRole(originalUserName);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,originalUserName),
                new Claim(ClaimTypes.Role, role)
            };

            var jwtResult = _jwtAuthManager.GenerateTokens(originalUserName, claims, DateTime.Now);
            _logger.LogInformation($"User [{originalUserName}] has stopped impersonation.");
            return Ok(new LoginResult
            {
                UserName = originalUserName,
                //Role = role,
                OriginalUserName = null,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            });
        }*/
    }
}

/*using System;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication_MT4North.Infrastructure;
//using WebApplication_MT4North.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace WebApplication_MT4North.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtAuthManager _jwtAuthManager;

        public AccountController(ILogger<AccountController> logger, UserManager<IdentityUser> userManager , RoleManager<IdentityRole> roleManager, IJwtAuthManager jwtAuthManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtAuthManager = jwtAuthManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = _userManager.Users.SingleOrDefault(u => u.UserName == request.UserName);
            if (user is null)
            {
                // User not found in db
                return NotFound("User not found");
            }
            var userSigninResult = await _userManager.CheckPasswordAsync(user, request.Password);
            if (userSigninResult)
            {
                return Unauthorized();
            }

            var role = "BasicUser";// _userManager.GetUserRole(request.UserName);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name,request.UserName),
                new Claim(ClaimTypes.Role, role)
            };

            var jwtResult = _jwtAuthManager.GenerateTokens(request.UserName, claims, DateTime.Now);
            _logger.LogInformation($"User [{request.UserName}] logged in the system.");
            return Ok(new LoginResult
            {
                UserName = request.UserName,
                Role = role,
                AccessToken = jwtResult.AccessToken,
                RefreshToken = jwtResult.RefreshToken.TokenString
            });
        }

        [HttpGet("user")]
        [Authorize]
        public ActionResult GetCurrentUser()
        {
            return Ok(new LoginResult
            {
                UserName = User.Identity?.Name,
                Role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
                OriginalUserName = User.FindFirst("OriginalUserName")?.Value
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public ActionResult Logout()
        {
            // optionally "revoke" JWT token on the server side --> add the current token to a block-list
            // https://github.com/auth0/node-jsonwebtoken/issues/375

            var userName = User.Identity?.Name;
            _jwtAuthManager.RemoveRefreshTokenByUserName(userName);
            _logger.LogInformation($"User [{userName}] logged out the system.");
            return Ok();
        }

        [HttpPost("refresh-token")]
        [Authorize]
        public async Task<ActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var userName = User.Identity?.Name;
                _logger.LogInformation($"User [{userName}] is trying to refresh JWT token.");

                if (string.IsNullOrWhiteSpace(request.RefreshToken))
                {
                    return Unauthorized();
                }

                var accessToken = await HttpContext.GetTokenAsync("Bearer", "access_token");
                var jwtResult = _jwtAuthManager.Refresh(request.RefreshToken, accessToken, DateTime.Now);
                _logger.LogInformation($"User [{userName}] has refreshed JWT token.");
                return Ok(new LoginResult
                {
                    UserName = userName,
                    Role = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
                    AccessToken = jwtResult.AccessToken,
                    RefreshToken = jwtResult.RefreshToken.TokenString
                });
            }
            catch (SecurityTokenException e)
            {
                return Unauthorized(e.Message); // return 401 so that the client side can redirect the user to login page
            }
        }

    }
}
*/