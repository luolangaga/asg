using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static asg_form.Controllers.excel;
using System.Security.Claims;
using static asg_form.Controllers.login;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using RestSharp;
using static asg_form.blog;
using static asg_form.Controllers.schedule;
using static NPOI.HSSF.Util.HSSFColor;
using NLog;
using System.Security.Authentication;
using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using asg_form.Controllers.Hubs;
using NPOI.OpenXmlFormats.Spreadsheet;
using Mirai.Net.Data.Shared;
using MimeKit;
using MailKit.Net.Smtp;
using Mirai.Net.Utils.Scaffolds;
using Mirai.Net.Sessions.Http.Managers;
using asg_form.Controllers.Store;
using NPOI.SS.Formula.Functions;
using static asg_form.Controllers.Store.Storehttp;

namespace asg_form.Controllers
{

    public class admin : ControllerBase
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        private readonly IHubContext<room> hubContext;
        public admin(
            RoleManager<Role> roleManager, UserManager<User> userManager, IHubContext<room> hubContext)
        {

            this.roleManager = roleManager;
            this.userManager = userManager;
            this.hubContext = hubContext;
        }
        [Route("api/v1/admin/allperson_c")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<int>> getalladmin_c()
        {
            int a = await userManager.Users.CountAsync();
            return Ok(a);
        }
        [Route("api/v1/admin/allteam_c")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<int>> getteam_c()
        {
            TestDbContext testDb = new TestDbContext();
            int a = testDb.Forms.Count();
            if (a >= 100)
            {

            }
            return Ok(a);
        }

        [Route("api/v1/admin/statistics")]
        [HttpGet]
        [Authorize]
        [ResponseCache(Duration = 600)]
        public async Task<ActionResult<object>> all_total()
        {
            TestDbContext testDb = new TestDbContext();
            int form_t = testDb.Forms.Count();
            int user_t = userManager.Users.Count();
            int sh_t = testDb.team_Games.Count();
            int team_log_t = testDb.schlogs.Count();
            int role_t = testDb.Roles.Count();
            return new { form_t = form_t, user_t = user_t, sh_t = sh_t, sh_log_t = team_log_t, role_t = role_t };
        }

        [Route("api/v1/admin/updata_img")]
        [HttpPost]
        public async Task<ActionResult<object>> update_img(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return BadRequest("Invalid image file.");
            // 将文件保存到磁盘
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "loge/", $"friend-{imageFile.FileName}");
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }    // 返回成功响应
            return Ok("Image file uploaded successfully.");

        }
        [Route("api/v1/admin/dbgu")]
        [HttpPost]
        public async Task<ActionResult<object>> dbgu()
        {
           using(TestDbContext db=new TestDbContext())
            {
                await db.Database.MigrateAsync();
            }
            return Ok("successfully.");

        }

        [Route("api/v1/admin/Privacy_Policy")]
        [HttpPost]
        [Authorize]

        public async Task<ActionResult<object>> Privacy_Policy([FromBody] string rule_markdown)
        {

            if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });
            }


            System.IO.File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + $"doc/rule/隐私政策.md", rule_markdown);
            return Ok("添加成功！");

        }


        [Route("api/v1/admin/post_qqbotmsg")]
        [HttpPost]
        [Authorize]

        public async Task<ActionResult<object>> post_qqbotmsg([FromBody] string msg,string qqgrope,bool is_atall)
        {

            if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });
            }
if(is_atall){
                var messageChain = new MessageChainBuilder()
                                 .AtAll()
                                .Plain(msg)
                                .Build();
                await MessageManager.SendGroupMessageAsync(qqgrope, messageChain);

            }
else{
                var messageChain = new MessageChainBuilder()
                                          .Plain(msg)
                                          .Build();
                await MessageManager.SendGroupMessageAsync(qqgrope, messageChain);

            }


            return Ok("成功！");

        }





        [Route("api/v1/admin/allschedle_c")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<int>> getschedle_c()
        {
            TestDbContext testDb = new TestDbContext();
            int a = testDb.team_Games.Count();
            return Ok(a);
        }

        /// <summary>
        /// 获取所有用户-支持分页
        /// </summary>
        /// <param name="page"></param>
        /// <param name="page_long"></param>
        /// <returns></returns>
        [Route("api/v1/admin/allperson")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<post_user>>> getalladmin(short page, short page_long = 10)
        {



            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {

                int a = userManager.Users.Count();
                int b = page_long * page;
                if (page_long * page > a)
                {
                    b = a;
                }
                var users = userManager.Users.Skip(page_long * page - page_long).Take(page_long).ToList();
                List<post_user> user = new List<post_user>();
                foreach (var auser in users)
                {
                    bool isadmin = await userManager.IsInRoleAsync(auser, "admin");
                    var roles = await userManager.GetRolesAsync(auser);
                    user.Add(new post_user { id = auser.Id, chinaname = auser.chinaname, name = auser.UserName, isadmin = isadmin, email = auser.Email, Roles = (List<string>)roles, officium = auser.officium });

                }
                return user;


            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }




        }






        [Route("api/v1/admin/userfind/{userid}")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<object>> getuser(long userid)
        {

            if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });
            }
            return await userManager.Users.Select(a => new { a.Id, a.Email, a.chinaname, a.UserName, a.Integral, a.officium }).FirstAsync(a=>a.Id==userid);
        }



        /// <summary>
        /// 获取所有用户-支持分页(整合api:allperson_c)
        /// </summary>
        /// <param name="page"></param>
        /// <param name="page_long"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [Route("api/v2/admin/allperson")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<post_user_v2>> getalladmin_v2(string? keyword, short pageindex = 1, short pagesize = 10)
        {
            bool isAdmin = this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin");
            if (isAdmin)
            {
                var a = new all_record();

                if (keyword == null)
                {
                    a.cout = userManager.Users.Count();
                    a.msg = await userManager.Users.Paginate(pageindex, pagesize).Select(a => new { a.Id, a.Email, a.chinaname, a.UserName, a.Integral, a.officium}).ToListAsync();
           
                }
                else
                {

                    a.cout = userManager.Users.Where(a => a.chinaname == keyword || a.UserName == keyword || a.Email == keyword).Count();
                    a.msg = await userManager.Users.Where(a => a.chinaname == keyword || a.UserName == keyword || a.Email == keyword).Paginate(pageindex, pagesize).Select(a => new { a.Id, a.Email, a.chinaname, a.UserName, a.Integral, a.officium}).ToListAsync();
                }
                return Ok(a);
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }
        }

        public class post_user_v2
        {


            public int Count { get; set; }
            public List<post_user> user { get; set; } = new List<post_user>();


        }

        /// <summary>
        /// 设置管理员,需要superadmin
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [Route("api/v1/admin/setadmin")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> setadmin(string userid)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                var ouser = await userManager.FindByIdAsync(userid);

                await userManager.AddToRoleAsync(ouser, "admin");

                return Ok(new { message = "用户成功设置为管理员" });
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }

        }


        //管理员设置用户的职位
        [Route("api/v1/admin/setop")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> setrole(string userid, string opname)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                var ouser = await userManager.FindByIdAsync(userid);
               
                ouser.officium = opname;
                await userManager.UpdateAsync(ouser);


                return "成功！";
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }

        }
        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="email">收件人邮箱</param>
        /// <param name="title">标题</param>
        /// <param name="content">发送内容</param>
        /// <returns></returns>
        public static bool SendEmail(string email1, string title, string content)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("ASG管理员", "admin@idvasg.cn"));
            message.To.Add(new MailboxAddress("用户", email1));
            message.Subject = title;
            message.Body = new TextPart("html")
            {
                Text = content
            };
            var client = new SmtpClient();
            try
            {
                client.SslProtocols = System.Security.Authentication.SslProtocols.Tls12;
                client.Connect("smtp.zeptomail.com.cn", 587, false);
                client.Authenticate("emailapikey", "eiwqDPhYvz0JfAQUxXs1c7O73eRiDb3M8/Gf5RApUPFGGubJSXsdBgtmpwu3IVEtfn3yErFsaKxyy8T14VUn85QSbSlYs6Cq+CaF7ISNMHtAL/6LeVmGwh9Qhwk1b6IDW6AK/kk2B53nNw==");
                client.Send(message);
                client.Disconnect(true);
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
            }
            return true;


        }




        /// <summary>
        /// 管理员直接添加一个用户
        /// </summary>
        /// <param name="newuser"></param>
        /// <param name="captoken"></param>
        /// <returns></returns>
        [Route("api/v1/admin/enroll")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<newuser_get>> Post([FromBody] newuser_get newuser, string captoken)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {

                User user = await this.userManager.FindByEmailAsync(newuser.EMail);
                if (user == null)
                {
                    user = new User { UserName = newuser.UserName, Email = newuser.EMail, chinaname = newuser.chinaname, EmailConfirmed = true };
                    var r = await userManager.CreateAsync(user, newuser.Password);

                    if (!r.Succeeded)
                    {
                        return BadRequest(r.Errors);
                    }
                    /*   new Email()
                       {
                           SmtpServer = "smtphz.qiye.163.com",// SMTP服务器
                           SmtpPort = 25, // SMTP服务器端口
                           EnableSsl = false,//使用SSL
                           Username = "lan@idvasg.cn",// 邮箱用户名
                           Password = "aNcdGsEYVghrNsE7",// 邮箱密码
                           Tos = newuser.EMail,//收件人
                           Subject = "欢迎加入ASG赛事！",//邮件标题
                           Body = $"欢迎加入ASG赛事，当你看到这封邮件时说明你已经注册成功，感谢你支持ASG赛事！",//邮件内容
                       }.SendAsync(s =>
                       {

                       });// 异步发送邮件
                    */
                    return newuser;
                }
                return BadRequest(new error_mb { code = 400, message = "此邮件已被使用" });


            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }




        }




        /// <summary>
        /// 删除用户,需要superadmin
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        [Route("api/v1/admin/deluser")]
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult<string>> deluser(string userid)
        {

            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                var setuser = await userManager.FindByIdAsync(userid);

                await userManager.DeleteAsync(setuser);
                logger.Warn($"管理员删除了用户{setuser.UserName}！");
                return "成功！";
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }

        }



        /// <summary>
        /// 设置职位,需要superadmin
        /// </summary>
        /// <param name="userid">用户id</param>
        /// <param name="officium">职位名称</param>
        /// <returns></returns>
        [Route("api/v1/admin/officium")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> setofficium(string userid, string officium)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                var ouser = await userManager.FindByIdAsync(userid);

                ouser.officium = officium;
                await userManager.UpdateAsync(ouser);
                logger.Warn($"设置了{ouser.UserName}的职位为{officium}");
                SendEmail(ouser.Email, "ASG赛事组", $@"<div>
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
                               你的职位已经被设置为{officium}。
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
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }

        }

        /// <summary>
        /// 给所有form两两随机组队
        /// </summary>
        /// <param name="formname"></param>
        /// <returns></returns>
        [Route("api/v1/admin/team/")]
        [HttpPost]
        public async Task<ActionResult<string>> team([FromBody] int[] formid, string game_tag)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                TestDbContext ctx = new TestDbContext();

                var form = ctx.Forms.Include(a => a.events).OrderBy(a => Guid.NewGuid()).Where(a => formid.Any(b => b == a.Id)).ToList();

                string teamname1 = "";
                string teamname2 = "";
                for (int i = 0; i < form.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        teamname1 = form[i].team_name;

                    }
                    else
                    {
                        teamname2 = form[i].team_name;
                        ctx.team_Games.Add(new team_game
                        {
                            team1_name = teamname1,
                            team2_name = teamname2,
                            opentime = DateTime.Now,
                            team1_piaoshu = 0,
                            team2_piaoshu = 0,
                            commentary = "待公布",
                            referee = "待公布",
                            belong = form[1].events.name,
                            tag = game_tag
                        });
                        // await Task.Delay(6000);
                    }
                }

                await ctx.SaveChangesAsync();
                logger.Info($"管理员已经随机分组");
                return "OK";

            }
            return BadRequest(new error_mb { code = 400, message = "无权访问" });


        }

        [Route("api/v1/admin/SendEmail/")]
        [HttpPost]
        public async Task<ActionResult<string>> Sendemail(string To, string Title, string msg)
        {
            SendEmail(To, Title, msg);
            return Ok();
        }

        /// <summary>
        /// 删除表单
        /// </summary>
        /// <param name="formid">表单id</param>
        /// <param name="password">表单密码</param>
        /// <returns></returns>
        [Route("api/v1/admin/form/")]
        [HttpDelete]
        public async Task<ActionResult<string>> delform(int formid)
        {

            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                TestDbContext ctx = new TestDbContext();
                var form = await ctx.Teams.Include(a => a.role).FirstOrDefaultAsync(a => a.Id == formid);
                var users = await userManager.Users.Include(a => a.myteam).Where(a => a.myteam == form).ToListAsync();
                try
                {
                    foreach (var user in users)
                    {
                        user.haveform = null;
                        await userManager.UpdateAsync(user);
                    }
                }
                catch
                {
                }

                ctx.Teams.Remove(form); ;
                await ctx.SaveChangesAsync();
                logger.Warn($"管理员删除了表单{formid},参赛选手：{string.Join(',', form.role.Select(a => a.role_name))}");
                return Ok("删除成功！");
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }






        }



        //管理员设置用户的职位
        [Route("api/v1/admin/Friend")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> Add_Friend(T_Friend friend)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                using (TestDbContext ctx = new TestDbContext())
                {
                    ctx.T_Friends.Add(friend);
                    await ctx.SaveChangesAsync();
                }


                return "成功！";
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }

        }

        //管理员设置用户的职位
        [Route("api/v1/admin/Friend")]
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<string>> Put_Friend(T_Friend friend, int friend_id)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                using (TestDbContext ctx = new TestDbContext())
                {
                    var friend_p = ctx.T_Friends.First(a => a.id == friend_id);
                    friend_p.comMsg = friend.comMsg;
                    friend_p.comTime = friend.comTime;
                    friend_p.account = friend.account;
                    friend_p.orgName = friend.orgName;
                    friend_p.headName = friend.headName;
                    friend_p.degree = friend.degree;
                    friend_p.comType = friend.comType;
                    friend_p.headTel = friend.headTel;
                    await ctx.SaveChangesAsync();
                }


                return "成功！";
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }

        }


        [Route("api/v1/admin/Friend")]
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult<string>> Del_Friend(long friend_id)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                using (TestDbContext ctx = new TestDbContext())
                {
                    var friend = await ctx.T_Friends.FirstAsync(a => a.id == friend_id);
                    ctx.Remove(friend);
                    await ctx.SaveChangesAsync();
                }


                return "成功！";
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }

        }

        [Route("api/v1/admin/Friend")]
        [HttpGet]
        public async Task<ActionResult<string>> Get_Friend(short page, short page_long)
        {


            using (TestDbContext ctx = new TestDbContext())
            {
                int Total = ctx.T_Friends.Count();
                int b = page_long * page;
                if (page_long * page > Total)
                {
                    b = Total;
                }

                var friend = await ctx.T_Friends.OrderByDescending(a => a.degree).Skip(page_long * page - page_long).Take(page_long).ToListAsync();
                object body = new { friend, Total };
                return Ok(body);
            }





        }



        private readonly Logger logger = LogManager.GetCurrentClassLogger();






    }
}
