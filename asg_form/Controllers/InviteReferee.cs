using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    }

    [Route("api/v1/Invite")]
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<object>> GetReferee([FromBody] long Inviteeid)
    {
        string Invitorid = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var user = await userManager.FindByIdAsync(Invitorid);


    }
}
