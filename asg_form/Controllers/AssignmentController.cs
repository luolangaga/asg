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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Web;
using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using NPOI.SS.Formula.Functions;
using Microsoft.EntityFrameworkCore;

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

        public string createTime { get; set; }
        
        public string lastOperateTime { get; set; }
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
            if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "nbadmin"))
            {
                return Ok(new error_mb { code = 401, message = "无权访问" });
            }
            var dateString = DateTime.Now;
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
                    createTime = dateString.ToString(),
                    lastOperateTime = dateString.ToString()
                };
                sub.T_Task.Add(task);
                await sub.SaveChangesAsync();
                return Ok(new error_mb { code = 200, message = "新建任务成功" });
            }

        }


        [Route("api/v1/admin/Task")]
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult<object>> DelTask([FromQuery] long id)
        {
            if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "nbadmin"))
            {
                return Ok(new error_mb { code = 401, message = "无权访问" });
            }
            using (TestDbContext sub = new TestDbContext())
            {
                sub.T_Task.Remove(sub.T_Task.Find(id));
                await sub.SaveChangesAsync();
                return Ok(new error_mb { code = 200, message = "成功删除" });
            }
        }

        [Route("api/v1/Task")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<object>> CekTask([FromQuery] long taskid)
        {  
            using (TestDbContext sub = new TestDbContext())
            {
                var task = sub.T_Task.Find(taskid);
                var dateString = DateTime.Now;
                task.status = "1";
                task.lastOperateTime = dateString.ToString();
                await sub.SaveChangesAsync();
                return Ok(new error_mb { code = 200, message = "成功提交修改" });
            }
        }
        public class statusChange
        {
            public long taskid {  get; set; }
            public string status { get; set; }

        }
        [Route("api/v1/admin/Task/Done")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<object>> FinishTask([FromBody] statusChange msg)
        {
            string userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(userId);

            if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "nbadmin"))
            {
                return Ok(new error_mb { code = 401, message = "无权访问" });
            }
            var dateString = DateTime.Now;
            using (TestDbContext sub = new TestDbContext())
            {
                var task = sub.T_Task.Find(msg.taskid);
                long isPassed = long.Parse(msg.status);
                if(isPassed == 2)
                {
                    task.status = "2";
                    user.Integral += task.money;
                    user.Integral = user.Integral > 200 ? 200 : user.Integral;
                }
                if (isPassed == 3)
                {
                    task.status = "3";
                }
                task.lastOperateTime = dateString.ToString();
                await userManager.UpdateAsync(user);
                await sub.SaveChangesAsync();
                return Ok(new error_mb { code = 200, message = "成功修改" });
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

            //return Ok("用户不存在");
            return query.OrderByDescending(a => a.userId).ToList();
        }

        [Route("api/v1/admin/FindTasks")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<object>> FindTasks([FromQuery] string chinaname = null,string status = null,short page = 1,short limit = 10)
        {
            //string encodedChinaname = HttpUtility.UrlEncode(chinaname);

            if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "nbadmin"))
            {
                return Ok(new error_mb { code = 401, message = "无权访问" });
            }
            using (TestDbContext sub = new TestDbContext())
            {
                //List <TaskDB> taskDBs = new List<TaskDB>();

                var query = sub.T_Task.AsQueryable();

                if (!string.IsNullOrEmpty(chinaname))
                {
                    query = query.Where(n => n.chinaname.Contains(chinaname));
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(n => n.status == status);
                }

                var TotalRecords = await query.CountAsync();

                var Tasks = await query
                    .OrderByDescending(a => a.status)
                    .Skip((page - 1) * limit)
                    .Take(limit)
                    .ToListAsync();

                var result = new
                {
                    rows = Tasks ,
                    total = TotalRecords,
                };
                return Ok(result);
            }
        }

        [Route("api/v1/Find_nbadmin")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<User>>> Find_nbadmin()
        {
            using (TestDbContext fd = new TestDbContext())
            {              
                var usersWithNbadminRole = this.User.FindAll(ClaimTypes.Role)
                             .Where(a => a.Value == "nbadmin")
                             .ToList();

                return Ok(usersWithNbadminRole);
            }
        }

    }
}
