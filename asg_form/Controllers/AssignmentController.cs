using AngleSharp.Text;
using asg_form.Controllers.Hubs;
using asg_form.Controllers.Store;
using Manganese.Array;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NPOI.HPSF;
using System.Security.Claims;

namespace asg_form.Controllers
{
    public class TaskDB
    {
        public long id { get; set; }
        public string chinaname { get; set; }
        public long userId { get; set; }
        public string taskName { get; set; }
        public string taskDescription {  get; set; }
        public string status { get; set; }
        public long money { get; set; }
    }
    public class TaskCreate
    {
        public string Chinaname { get; set; }
        public long UserId { get; set; }
        public string TaskName { get; set; }
        public string TaskDescription { get; set; }
        public long Money { get; set; }
    }
    public class AssignmentController : ControllerBase
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        public AssignmentController(
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }
        [Route("api/v1/admin/Task")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> PubTask([FromBody] TaskCreate taskinfo)
        {
            if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                return Ok(new error_mb { code = 401, message = "无权访问" });
            }
            using (TestDbContext sub = new TestDbContext())
            {
                var task = new TaskDB
                {
                    chinaname = taskinfo.Chinaname,
                    userId = taskinfo.UserId,
                    taskName = taskinfo.TaskName,
                    taskDescription = taskinfo.TaskDescription,
                    money = taskinfo.Money,
                    status = "0",
                };
                sub.T_Task.Add(task);
                await sub.SaveChangesAsync();
                return Ok(taskinfo);
            }
        }


        [Route("api/v1/admin/Task")]
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult<object>> DelTask([FromQuery]long id)
        {
            if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                return Ok(new error_mb { code = 401, message = "无权访问" });
            }
            using (TestDbContext sub = new TestDbContext())
            {
                sub.T_Task.Remove(sub.T_Task.Find(id));
                await sub.SaveChangesAsync();
                return Ok("ok");
            }
        }
        public class Click_done
        {
            public long userId { get; set; }
            public long id { get; set; }
        }

        [Route("api/v1/Task")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<object>> CekTask([FromBody] Click_done msg)
        {
            string userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(userId);
      
            using (TestDbContext sub = new TestDbContext())
            {
                var task = sub.T_Task.Find(msg.id);
                if(user.Id != task.userId)
                {
                    return Ok(new error_mb { code = 401, message = "不是自己的任务" });
                }
                task.status = "1";
                await sub.SaveChangesAsync();
                return Ok(task);
            }
        }

        [Route("api/v1/admin/Task/Done")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<object>> FinishTask([FromBody] Click_done msg)
        {
            string userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(userId);

            if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                return Ok(new error_mb { code = 401, message = "无权访问" });
            }
            using (TestDbContext sub = new TestDbContext())
            {
                var task = sub.T_Task.Find(msg.id);
                task.status = "2";
                user.Integral += task.money;
                await userManager.UpdateAsync(user);
                await sub.SaveChangesAsync();
                return Ok(task);
            }
        }

        [Route("api/v1/Tasks")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<TaskDB>>> GetTasks([FromQuery] string userid = null)
        {
            TestDbContext test = new TestDbContext();

            var query = test.T_Task.AsQueryable();

            if (!string.IsNullOrEmpty(userid))
            {
                long idNum = long.Parse(userid);
                query = query.Where(n => n.userId == idNum);
            }

            return query.OrderByDescending(a => a.userId).ToList();
        }
    }
}
