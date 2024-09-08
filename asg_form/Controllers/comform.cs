using Manganese.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace asg_form.Controllers
{
    public class comform : ControllerBase
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
      
        public comform(
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {

            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [Route("api/v1/comform")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> getschedle_c([FromBody]req_com_form req)
        {
            int id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value.ToInt32();
            //  var user = await userManager.Users.FirstAsync(a=>a.Id==id);
            var dateString = DateTime.Now;
            TestDbContext testDb = new TestDbContext();
            var result = new com_form
            {
                comSex = req.sex,
                Com_Cocial_media = req.Com_Cocial_media,
                Com_qq = req.Com_qq,
                UserId=id,
                Status = 0,
                introduction = req.introduction,
                idv_id=req.idv_id,
                createTime = dateString.ToString(),
            };
        
           testDb.com_Forms.Add(result);
           await testDb.SaveChangesAsync();
            return Ok("成功！");
        }
        [Route("api/v1/admin/comform")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<string>> getschedle_c(short page, short limit)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                using (TestDbContext sub = new TestDbContext())
                {
                    var query = sub.com_Forms.AsQueryable();

                    var TotalRecords = await query.CountAsync();

                    var signData = await query
                    .OrderByDescending(a => a.Status)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToListAsync();

                    var result = new
                    {
                        rows = signData,
                        total = TotalRecords,
                    };
                return Ok(result);
                }
                
            }
            return Ok(new error_mb { code = 400, message = "没有管理员，无法获取" });
        }
        [Route("api/v1/admin/user")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<User>> getschedle_c(long userid)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                 return await userManager.FindByIdAsync(userid.ToString());
            }
            return BadRequest(new error_mb { code = 400, message = "没有管理员，无法获取" });
        }
        [Route("api/v1/admin/comform_ok")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<string>> ok_comform(long comform_id)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
               
                TestDbContext testDb = new TestDbContext();

                var comform = await testDb.com_Forms.FirstAsync(a=>a.Id == comform_id);
                comform.Status = 1;
               await testDb.SaveChangesAsync();
                var ouser = await userManager.FindByIdAsync(comform.UserId.ToString());
                ouser.officium = "Commentator";
  
                await userManager.UpdateAsync(ouser);
                  admin.SendEmail(ouser.Email, "ASG赛事组", $@"<div>
    <includetail>
        <table style=""font-family: Segoe UI, SegoeUIWF, Arial, sans-serif; font-size: 12px; color: #333333; border-spacing: 0px; border-collapse: collapse; padding: 0px; width: 580px; direction: ltr"">
            <tbody>
            <tr>
                <td style=""font-size: 10px; padding: 0px 0px 7px 0px; text-align: right"">
                    {ouser.chinaname} ，欢迎加入ASG赛事组。
                </td>
            </tr>
            <tr style=""background-color: #0078D4"">
                <td style=""padding: 0px"">
                    <table style=""font-family: Segoe UI, SegoeUIWF, Arial, sans-serif; border-spacing: 0px; border-collapse: collapse; width: 100%"">
                        <tbody>
                        <tr>
                            <td style=""font-size: 38px; color: #FFFFFF; padding: 12px 22px 4px 22px"" colspan=""3"">
                                欢迎
                            </td>
                        </tr>
                        <tr>
                            <td style=""font-size: 20px; color: #FFFFFF; padding: 0px 22px 18px 22px"" colspan=""3"">
                                 欢迎{ouser.chinaname}加入ASG赛事组。
                            </td>
                        </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
            <tr>
                <td style=""padding: 30px 20px; border-bottom-style: solid; border-bottom-color: #0078D4; border-bottom-width: 4px"">
                    <table style=""font-family: Segoe UI, SegoeUIWF, Arial, sans-serif; font-size: 12px; color: #333333; border-spacing: 0px; border-collapse: collapse; width: 100%"">
                        <tbody>
                        <tr>
                            <td style=""font-size: 12px; padding: 0px 0px 5px 0px"">
                               你的职位已经被设置为Commentator。
                                <ul style=""font-size: 14px"">
                                    <li style=""padding-top: 10px"">
                                        对此次执行有疑问请联系我们的QQ：2667210109。
                                    </li>
                                    <li>
                                        请不要回复此邮件。如果你需要帮助，请联系我们。
                                    </li>
                                    <li>
                                        请加入对应职位的群聊。
                                    </li>
                                </ul>
                            </td>
                        </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
            <tr>
                <td style=""padding: 0px 0px 10px 0px; color: #B2B2B2; font-size: 12px"">
                    版权所有 ASG赛事官网
                </td>
            </tr>
            </tbody>
        </table>
    </includetail>
</div>
");
                return "成功！";

            }
            return BadRequest(new error_mb { code = 400, message = "没有管理员，无法设置" });
        }
        [Route("api/v1/admin/comform_no")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<string>> no_comform(long comform_id)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {

                TestDbContext testDb = new TestDbContext();

                var comform = await testDb.com_Forms.FirstAsync(a => a.Id == comform_id);
                comform.Status = 2;
                var ouser =await userManager.FindByIdAsync(comform.UserId.ToString());
               await testDb.SaveChangesAsync();
                admin.SendEmail(ouser.Email, "ASG赛事组", $@"很抱歉，你的解说申请未通过");
                return "成功！";

            }
            return BadRequest(new error_mb { code = 400, message = "没有管理员，无法设置" });
        }
        public class req_com_form
        {
          
            public string sex { get; set; }
            public string? Com_Cocial_media { get; set; }
            public string idv_id { get; set; }
            public string introduction { get; set; }
            public string Com_qq { get; set; }

        }
        public class com_form
        {
            public long Id { get; set; }
            public int UserId { get; set; }
            public string comSex { get; set; }
            public string? Com_Cocial_media { get; set; }
            public string idv_id { get; set; }
            public string introduction { get; set; }
            public string Com_qq { get; set; }
            public int Status { get; set; }

            public string createTime { get; set; }
        }
    }
     

        }

