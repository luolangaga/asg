﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using static 所有队伍;
using Microsoft.AspNetCore.Authorization;
using static asg_form.blog;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using NPOI.OpenXmlFormats.Spreadsheet;
using Castle.Components.DictionaryAdapter;

namespace asg_form.Controllers
{
    public class schedule:ControllerBase
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        public schedule(
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {

            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [Authorize]
        [Route("api/v1/admin/game")]
        [HttpPut]
        public async Task<ActionResult<string>> gameput(long gameid,[FromBody] req_team_game req)
        {

            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);

            bool a = await userManager.IsInRoleAsync(user, "admin");
            if (a)
            {
                TestDbContext testDb = new TestDbContext();

                var game = await testDb.team_Games.FirstOrDefaultAsync(a => a.id == gameid);
                game.team1_name = req.team1_name;
                game.team2_name = req.team2_name;
                game.opentime = req.opentime;
                game.commentary = req.commentary;
                game.bilibiliuri = req.bilibiliuri;
                game.referee = req.referee;
                game.belong = req.belong;
                game.tag=req.tag;
                await testDb.SaveChangesAsync();
                return "ok";
             


            }
            return BadRequest(new error_mb { code = 400, message = "无权访问" });

        }






        /// <summary>
        /// 发布一个竞猜比赛
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/v1/admin/game")]
        [HttpPost]
        public async Task<ActionResult<string>> gamepost([FromBody] req_team_game req)
        {

            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);

            bool a = await userManager.IsInRoleAsync(user, "admin");
            if (a)
            {
                TestDbContext testDb = new TestDbContext();
                testDb.team_Games.Add(new team_game { 
                    team1_name = req.team1_name,
                    team2_name = req.team2_name,
                    opentime = req.opentime,
                     team1_piaoshu = 0, 
                    team2_piaoshu = 0 ,
                commentary=req.commentary,
                referee=req.referee,
                belong=req.belong,
                tag=req.tag
                });
                await testDb.SaveChangesAsync();
                return "ok";
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }

        }


        /// <summary>
        /// 发布胜利者
        /// </summary>
        /// <param name="teamid"></param>
        /// <param name="winteam"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/v1/admin/game/win")]
        [HttpPost]
        public async Task<ActionResult<string>> gamepost(long teamid,string winteam)
        {

            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);

            bool a = await userManager.IsInRoleAsync(user, "admin");
            if (a)
            {
                TestDbContext testDb = new TestDbContext();
                team_game game=testDb.team_Games.Include(a=>a.logs).First(a=>a.id==teamid);
                game.winteam = winteam;
               foreach(var log in game.logs)
                {
                    if (log.chickteam == winteam)
                    {
                        log.win = true;
                    }else
                    {
                        log.win = false;
                    }
                }
                await testDb.SaveChangesAsync();
                return "ok";
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }

        }


        /// <summary>
        /// 删除竞猜比赛
        /// </summary>
        /// <param name="gameid"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/v1/admin/game")]
        [HttpDelete]
        public async Task<ActionResult<string>> gamepush(int gameid)
        {
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);

            bool a = await userManager.IsInRoleAsync(user, "admin");
            if (a)
            {
                TestDbContext testDb = new TestDbContext();
                team_game game = testDb.team_Games.Include(a => a.logs).First(a => a.id == gameid);
                testDb.team_Games.Remove(game);
                await testDb.SaveChangesAsync();
                return "ok";
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }

        }

        /// <summary>
        /// 投票
        /// </summary>
        /// <param name="gameid"></param>
        /// <param name="teamid"></param>
        /// <returns></returns>
            [Authorize]
        [Route("api/v1/game/pushgame")]
        [HttpPut]
        public async Task<ActionResult<string>> gamepush(int gameid,int teamid)
        {

            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            TestDbContext test =new TestDbContext();
          var team =  test.team_Games.Include(a=>a.logs).Single(a=>a.id==gameid);
            bool isckick = team.logs.Any(a => a.userid == id);
            if (isckick)
            {
                return BadRequest(new error_mb { code = 400, message = "不要重复投票" });

            }
            else
            {
                if (teamid == 1)
                {
                    team.team1_piaoshu++;
                    team.logs.Add(new schedule_log { userid= id ,chickteam=team.team1_name,team=team,win= null });
                }
                else if(teamid == 2)
                {
                    team.team2_piaoshu++;
                    team.logs.Add(new schedule_log { userid = id, chickteam = team.team2_name, team = team,win=null });

                }
                else
                {
                    return BadRequest(new error_mb { code = 400, message = "队伍id不合法" });

                }
                await test.SaveChangesAsync();
            }
            return "ok";
        }

        /// <summary>
        /// 获得所有比赛
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/game/")]
        [HttpGet]
        public async Task<ActionResult<List<team_game>>> gameall(int page,int page_long,string belong="all")
        {
           
            TestDbContext test = new TestDbContext();

            int c = test.Forms.Count();
            int b = page_long * page;
            if (page_long * page > c)
            {
                b = c;
            }
            List<team_game> team = new List<team_game>();
            if(belong=="all")
                {
              team = test.team_Games.OrderByDescending(a => a.opentime).Skip(page_long * page - page_long).Take(page_long).ToList();

            }
            else
            {
              team = test.team_Games.Where(a => a.belong == belong).OrderByDescending(a => a.opentime).Skip(page_long * page - page_long).Take(page_long).ToList();

            }

            return team;

        }

        /// <summary>
        /// 获取我的竞猜
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("api/v1/game/mylog")]
        [HttpGet]
        public async Task<ActionResult<List<schedule_log>>> mylog()
        {
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            TestDbContext test = new TestDbContext();
            var team = test.schlogs.Include(a=>a.team).Where(a=>a.userid==id).ToList();
            foreach(var team_game in team)
            {
                team_game.team.logs = null;
            }
            return team;


        }

    



        public class schedule_log
        {
            public long Id { get; set; }
            public string userid { get; set; }
            public team_game team { get; set; }
            public string chickteam { get; set; }
            public bool? win { get; set; }
        }


        public class team_game
        { 
            public long id { get; set; }

            public string team1_name { get; set; }
            public int team1_piaoshu { get; set; }
            public string team2_name { get; set; }
            public int team2_piaoshu { get; set; }
            public DateTime opentime { get; set; }
            /// <summary>
            /// 解说的名字，用逗号隔开！！！！！！！
            /// </summary>
            public string commentary { get; set; }
            /// <summary>
            /// 裁判的名字
            /// </summary>
            public string referee { get; set; }
            /// <summary>
            /// bilibili录屏路径
            /// </summary>
            public Uri? bilibiliuri { get; set; }
         public string? winteam { get; set; }
            public string? tag {  get; set; }
            public string? belong { get; set; }
            public List<schedule_log> logs { get; set; } = new List<schedule_log>();

        }



        public class req_team_game
        {
            public string team1_name { get; set; }
            public string team2_name { get; set; }
            public DateTime opentime { get; set; }

            /// <summary>
            /// 解说的名字，用逗号隔开！！！！！！！
            /// </summary>
            public string commentary { get; set; }
            /// <summary>
            /// 裁判的名字
            /// </summary>
            public string referee { get; set; }
            public string? belong { get; set; }
            public Uri? bilibiliuri { get; set;}
            public string tag {  get; set; }

        }



    }



        }

