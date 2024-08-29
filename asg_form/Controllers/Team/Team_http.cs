using asg_form.Controllers.Hubs;
using Manganese.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static 所有队伍;

namespace asg_form.Controllers.Team
{
    public class Team_http:ControllerBase
    {

        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;

        private readonly IHubContext<room> hubContext;
        public Team_http(IHubContext<room> hubContext, RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            this.hubContext = hubContext;
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [Route("api/v1/form/my")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> Posthameform(string eventname)
        {
            long id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value.ToInt64();
            var user = await userManager.Users.FirstAsync(a=>a.Id==id);
            var team= user.myteam;
            using(TestDbContext db=new TestDbContext())
            {
               var events= await db.events.FirstAsync(a=>a.name==eventname);
                events.Teams.Add(team);
                await db.SaveChangesAsync();
            }
            return Ok("成功");
        }


        /// <summary>
        /// 获取我的表单
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("api/v2/user/form")]
        [HttpGet]
        public async Task<ActionResult<form>> getmyform()
        {

            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var ouser = userManager.Users.Include(a => a.myteam.role).FirstOrDefault(a => a.Id == id.ToInt64());
            if (ouser.haveform == null)
            {
                return BadRequest(new error_mb { code = 400, message = "你没有绑定表单" });

            }
            ouser.haveform.events.forms = null;
            foreach (var role in ouser.haveform.role)
            {
                role.form = null;
            }
            return ouser.haveform;



        }



        /// <summary>
        /// 提交表单 eventname留空代表创建战队
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="for1">表单信息</param>
        /// <param name="captoken">谷歌人机验证验证码</param>
        /// <returns></returns>
        [Route("api/v3/form/")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> PostAsync(IFormFile imageFile, [FromForm] form_get_new for1)
        {

            using (TestDbContext ctx = new TestDbContext()) {
                string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                var user = await userManager.FindByIdAsync(id);


                if (ctx.Teams.Include(a => a.Events).Any(e => e.team_name == for1.team_name))
                {
                    return BadRequest(new error_mb { code = 400, message = "有重名队伍" });
                }
                else
                {
                    if (imageFile == null || imageFile.Length == 0)
                        return BadRequest("Invalid image file.");
                    // 将文件保存到磁盘
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), $"loge/", $"{imageFile.FileName}");
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }    // 返回成功响应
                         //  base64toimg(for1.logo_base64, $@"{AppDomain.CurrentDomain.BaseDirectory}loge\{for1.events_name}\{for1.team_name}.png");
                  
                    
                 

                    T_Team form1 = new T_Team();
                    form1.logo_uri = $"/loge/{for1.team_name}.png";
                    form1.team_name = for1.team_name;
                    form1.team_password = for1.team_password;
                    form1.team_tel = for1.team_tel;
                    if (for1.events_name == null)
                    {

                    }
                    else
                    {
                        var events = await ctx.events.FirstAsync(ctx => ctx.name == for1.events_name);

                        form1.Events.Add(events);
                    }

                    List<T_Player> role = new List<T_Player>();
                    foreach (role_get a in for1.role_get)
                    {
                        role.Add(new T_Player { role_id = a.role_id, role_lin = a.role_lin, role_name = a.role_name, Common_Roles = a.Common_Roles, Historical_Ranks = a.Historical_Ranks, Id_Card = a.Id_Card, Game_Name = a.Game_Name, Phone_Number = a.Phone_Number, Id_Card_Name = a.Id_Card_Name });
                    }
                    form1.role = role;

                    await ctx.Teams.AddAsync(form1);
                    await ctx.SaveChangesAsync();
                    user.myteam = form1;
                    await userManager.UpdateAsync(user);
                 //   int nownumber = ctx.Forms.Count();
                    //ChatRoomHub chat = new ChatRoomHub();
                    // await chat.formok(nownumber, for1.team_name);
                 
                }

                }


            return "ok!";



        }



        /// <summary>
        /// 获得所有表单信息
        /// </summary>
        /// <param name="page">页数</param>
        /// <param name="page_long">每页长度</param>
        /// <returns></returns>

        [Route("api/v2/form/all")]
        [HttpGet]
        [Authorize]
        public List<team> Getform(short page, short page_long, string sort, string eventsname)
        {
            using (TestDbContext ctx = new TestDbContext())
            {


                int c = ctx.Teams.Count();
                int b = page_long * page;
                if (page_long * page > c)
                {
                    b = c;
                }
                var events = ctx.events.First(ctx => ctx.name == eventsname);
                List<T_Team> forms;
                if (sort == "vote")
                {
                   var team1 = ctx.events.Include(a => a.Teams).Select(a => new {a.name,a.Teams}).First(a=>a.name==eventsname);
                  forms=  team1.Teams.Where(a=>a.is_check==true).OrderByDescending(a => a.piaoshu).ToList();

                }
                else
                {
                    //改为按照id倒序排序
                    //forms = ctx.Forms.Include(a => a.role).Skip(page_long * page - page_long).Take(page_long).ToList();
                    var team1 = ctx.events.Include(a => a.Teams).Select(a => new { a.name, a.Teams }).First(a => a.name == eventsname);
                    forms = team1.Teams.Where(a => a.is_check == true).OrderByDescending(a => a.Id).ToList();

                }
                List<team> teams = new List<team>();


                foreach (T_Team for1 in forms)
                {
                    var team = new team { id = for1.Id, name = for1.team_name, timer = for1.time, piaoshu = for1.piaoshu, logo_uri = for1.logo_uri };
                   
                    teams.Add(team);
                    // a++;
                }
                return teams;
            }
        }


    }
}
