﻿using asg_form.Migrations;
using Manganese.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Mirai.Net.Sessions.Http.Managers;
using Newtonsoft.Json;
using System.Security.Claims;

namespace asg_form.Controllers
{
    public class Com : ControllerBase
    {

        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        public Com(
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {

            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        /// <summary>
        /// 获取我的场次
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/com/my")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> com_my()
        {
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);
            if (user.officium == "Commentator")
            {
                TestDbContext testDb = new TestDbContext();
                string chinaname = user.chinaname;
                var teamgame = testDb.team_Games.Where(a => a.commentary.IndexOf(chinaname) >= 0).Select(a => new { a.id, a.team1_name, a.team2_name, a.bilibiliuri, a.commentary, a.referee ,a.opentime,a.team1_piaoshu,a.team2_piaoshu}).OrderByDescending(a => a.opentime).ToList();

                return JsonConvert.SerializeObject(teamgame);
            }
            return BadRequest(new error_mb { code = 400, message = $"你是{user.officium},你不是解说，无法操作" });
        }
        /// <summary>
        /// 选定场次
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/com/")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> com_post(long gameid)
        {
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);
            if (user.officium == "Commentator")
            {
                if (user.Integral == null)
                {
                    user.Integral = 0;
                    await userManager.UpdateAsync(user);
                }
                TestDbContext testDb = new TestDbContext();
                string chinaname = user.chinaname;
                var teamgame = await testDb.team_Games.FirstAsync(a => a.id == gameid);
                var com = JsonConvert.DeserializeObject<List<com_json>>(teamgame.commentary);
                com.Add(new com_json { id = id.ToInt32(), chinaname = chinaname });
                teamgame.commentary = JsonConvert.SerializeObject(com);
                await testDb.SaveChangesAsync();
                user.Integral = user.Integral+10;
                await userManager.UpdateAsync(user);
                try
                {
                    await MessageManager.SendGroupMessageAsync("870248618", $"解说:\r\n{chinaname}\r\n选择了比赛:\r\n{teamgame.team1_name} VS {teamgame.team2_name}");
                }
                catch
                {

                }
                return "成功";
            }
            return BadRequest(new error_mb { code = 400, message = $"你是{user.officium},你不是解说，无法操作" });
        }

        public class com_json
        {
            /// <summary>
            /// 
            /// </summary>
            public int id { get; set; }
            /// <summary>
            /// 老恐龙
            /// </summary>
            public string chinaname { get; set; }
        }


 


        /// <summary>
        /// 取消选班
        /// </summary>
        /// <param name="gameid"></param>
        /// <returns></returns>
        [Route("api/v1/com/")]
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult<string>> com_del(long gameid)
        {
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);
            if (user.Integral == null)
            {
                user.Integral = 0;
                await userManager.UpdateAsync(user);
            }
            if (user.officium == "Commentator")
            {
                TestDbContext testDb = new TestDbContext();
                string chinaname = user.chinaname;
                var teamgame = await testDb.team_Games.FirstAsync(a => a.id == gameid);
                var com = JsonConvert.DeserializeObject<List<com_json>>(teamgame.commentary);
                com.Remove(com.First(a => a.id == id.ToInt32()));
                try{
                    user.Integral = cut_value((long)user.Integral);
                    await userManager.UpdateAsync(user);
                }
                catch{
                    return BadRequest(new error_mb { code = 400, message = $"你的金钱无法满足你完成以下操作" });

                }
                teamgame.commentary = JsonConvert.SerializeObject(com);
                await testDb.SaveChangesAsync();
                return "成功";
            }
            return BadRequest(new error_mb { code = 400, message = $"你是{user.officium},你不是解说，无法操作" });
        }
        [Route("api/v1/com/search_ok_regist")]
        [HttpPost]
        [Authorize]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<string>> Search()
        {
            TestDbContext testDb = new TestDbContext();
            var team = await testDb.team_Games.Select(a => new { a.id, a.commentary, a.opentime, a.team1_name, a.team2_name, a.belong }).ToListAsync();
            var team1 = team.Where(a => a.commentary.Split(",").Length <= 1);
            return JsonConvert.SerializeObject(team1);
        }


        [Route("api/v1/com/Integral/ranking")]
        [HttpPost]
        [Authorize]
        [ResponseCache(Duration = 60)]
        public async Task<ActionResult<object>> ranking()
        {
       object user= await userManager.Users.OrderByDescending(a => a.Integral).Select(a=>new{a.Id,a.chinaname,a.Integral}).Take(10).ToListAsync();
            return user;
        }

        public long cut_value(long value)
        {
            long _value = value;
            value = value - 10;
            if (value < 0)
            {
                throw new ArgumentException("你已经没钱啦！");
               
            }
            return value;
        }

        [Route("api/v1/com/")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<int>> com_cout(long gameid)
        {
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);
            if (user.officium == "Commentator")
            {
                var chinaname = user.chinaname;
                TestDbContext testDb = new TestDbContext();
                int a = await testDb.team_Games.CountAsync(a => a.commentary.IndexOf(id) >= 0);
                return a;
            }
            return BadRequest(new error_mb { code = 400, message = $"你是{user.officium},你不是解说，无法操作" });
        }

    }
}


