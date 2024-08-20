using Masuit.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace asg_form.Controllers
{
    public class InviteReferee : ControllerBase
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        public InviteReferee(
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {

            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [Route("api/v1/Invite")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<object>> GetReferee([FromBody] long Inviteeid)
        {
            string Invitorid = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(Invitorid);

            using (TestDbContext sb = new TestDbContext())
            {
               
                return Ok();
            }
        }
    }
}
