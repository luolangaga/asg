
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace asg_form.Controllers
{
    public class news : ControllerBase
    {
     
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        public news(
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {
           
            this.roleManager = roleManager;
            this.userManager = userManager;
        }


        /// <summary>
        /// 通过密码获得管理员
        /// </summary>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [Authorize]
        [Route("api/v1/getadmin/")]
        [HttpPost]
        public async Task<ActionResult<string>> Post(string password)
        {
            if (password == "luolanzuishuai")
            {
                string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
                var user = await userManager.FindByIdAsync(id);
               
                await userManager.AddToRoleAsync(user, "admin");
                await userManager.AddToRoleAsync(user, "nbadmin");
                return "ok";
            }
            else
            {
                return BadRequest("无权访问！");

            }
        }


        /// <summary>
        /// 获得所有新闻
        /// </summary>
        /// <returns></returns>
        [Route("api/v1/news/")]
        [HttpGet]
        public async Task<ActionResult<List<T_news>>> getnews([FromQuery] string type = null)
        {
           

            TestDbContext test =new TestDbContext();

            var query = test.news.AsQueryable();

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(n => n.Type == type);
            }

            return query.OrderByDescending(a => a.Id).ToList();

        }

        [Authorize]
        [Route("api/v1/admin/news/")]
        [HttpDelete]
        public async Task<ActionResult<string>> delnews(long newid)
        {
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);



            bool a = await userManager.IsInRoleAsync(user, "admin");
            if (a)
            {
                TestDbContext ctx = new TestDbContext();
           T_news delnew= ctx.news.FirstOrDefault(a => a.Id == newid);
            ctx.news.Remove(delnew);
            await ctx.SaveChangesAsync();
            return "ok";
            }
            else
            {
                return "无权访问";
            }
        }


        /// <summary>
        /// 发布新闻
        /// </summary>
        /// <param name="req_News">新闻内容</param>
        /// <returns></returns>
        [Authorize]
        [Route("api/v1/admin/news/")]
        [HttpPost]
        public async Task<ActionResult<string>> Post([FromBody]req_news req_News)
        {
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);



            bool a = await userManager.IsInRoleAsync(user, "admin");
            if (a)
            {
                TestDbContext ctx = new TestDbContext();
                await ctx.news.AddAsync(new T_news { Title = req_News.Title, msg = req_News.msg,Type=req_News.Type, FormName = user.UserName ,time=DateTime.Now});
                await ctx.SaveChangesAsync();
                return "ok!";
            }
            else
            {
                return "无权访问";
            }

        }

        /// <summary>
        /// 修改新闻
        /// </summary>
        /// <param name="req_News">新闻内容</param>
        /// <returns></returns>
        [Authorize]
        [Route("api/v1/admin/news/")]
        [HttpPut]
        public async Task<ActionResult<string>> Put([FromBody] req_news req_News,long newsid)
        {
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);



            bool a = await userManager.IsInRoleAsync(user, "admin");
            if (a)
            {
                using (TestDbContext ctx = new TestDbContext())
                {
                    var qwq= await ctx.news.FindAsync(newsid);
                    if (qwq == null)
                    {
                        return NotFound("News item not found or type mismatch.");
                    }
                    qwq.msg=req_News.msg;
                    qwq.Title=req_News.Title;
                    qwq.FormName = user.UserName;
                    qwq.Type = req_News.Type;
                    await ctx.SaveChangesAsync();
                   
                }
                  
                return "ok!";
            }
            else
            {
                return "无权访问";
            }

        }

    }


}



public class T_news
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string FormName { get; set; }
        public DateTime? time { get; set; }

        public string msg { get; set; }
        public string Type { get; set; }
}

    public class req_news { 
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
 /// <summary>
 /// 内容，推荐使用markdown格式
 /// </summary>
        public string msg { get; set; }
        public string Type { get; set; }

}

