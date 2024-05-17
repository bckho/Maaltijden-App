using MaaltijdenApp_Core.Models;
using MaaltijdenApp_Core.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace MaaltijdenApp_WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class MealPackagesController : ControllerBase
    {
        private readonly IMealPackageRepository _mealPackageRepository;
        private readonly UserManager<IdentityUser> _userManager;

        public MealPackagesController(IMealPackageRepository mealPackageRepository, UserManager<IdentityUser> userManager)
        {
            _mealPackageRepository = mealPackageRepository;
            _userManager = userManager;
        }

        [HttpGet]
        [Route("Get")]
        public ActionResult<List<MealPackage>> Get()
        {
            try
            {
                var results = _mealPackageRepository.GetAllFuturePlanned();
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<MealPackage>> CreateReservation(string id)
        {
            //var token = Request.Headers.Authorization;
            //var handler = new JwtSecurityTokenHandler();
            //var jwtSecurityToken = handler.ReadJwtToken(token);

            //var userClaims = User.FindFirst(jwtSecurityToken.Subject);

            //var user = _userManager.GetUserAsync(userClaims.Value);

            //if (user == null) return Unauthorized();

            

            return Ok();
        }

        
    }
}
