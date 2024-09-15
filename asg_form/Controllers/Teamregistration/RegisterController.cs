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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Utilities.Encoders;

namespace asg_form.Controllers.Teamregistration
{
    public class ComformDB
    {
        public string chinaname { get; set; }
        public int user_id { get; set; }
        public int sex { get; set; }
        public string introduction { get; set; }
        public string game_id { get; set; }
        public string history_rank { get; set; }
        public string contact_number { get; set; }
        public string create_time { get; set; }
        //public string web_social_name { get; set; }
        public string approval_person { get; set; }
        public string approval_time { get; set; }
        public string status { get; set; }
    }
    public class userMsg
    {
        public string chinaname { get; set; }
        public int userId { get; set; }
        public int sex { get; set; }
        public string introduction { get; set; }
        public string gameId { get; set; }
        public string historyRank { get; set; }
        public string contactNumber { get; set; }
        public string id { get; set; }
    }
    public class RegisterController : ControllerBase 
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        public RegisterController(
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [Route("/api/v1/userRegister")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<object>> UserRgst([FromBody] userMsg msg)
        {
            string userId = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            using (TestDbContext sub = new TestDbContext())
            {
                if (sub.T_Comform.Find(userId) != null)
                {
                    return Ok(new error_mb { code = 400, message = "你已经提交过表单了，请不要重复提交" });
                }
                var dateString = DateTime.Now;

                if (msg.id == null)
                {
                    var rgst = new ComformDB
                    {
                    chinaname = msg.chinaname,
                    user_id = msg.userId,
                    sex = msg.sex,
                    introduction = msg.introduction,
                    game_id = msg.gameId,
                    history_rank = msg.historyRank,
                    contact_number = msg.contactNumber,
                    create_time = dateString.ToString(),
                    status = "1",
                    approval_person = "未审核",
                    approval_time = "未审核"                    
                    };
                    sub.T_Comform.Add(rgst);
                    await sub.SaveChangesAsync();
                    return Ok(new error_mb { code = 200, message = "提交成功" });
                }
                
                var query =  sub.T_Comform.Find(msg.id);               
                query.chinaname = msg.chinaname;                 
                query.user_id = msg.userId; 
                query.sex = msg.sex;                                   
                query.introduction = msg.introduction;               
                query.game_id = msg.gameId;               
                query.history_rank = msg.historyRank;                            
                query.contact_number = msg.contactNumber;
                query.create_time = dateString.ToString();
                await sub.SaveChangesAsync();
                return Ok(new error_mb { code = 200, message = "成功修改" });
            }

        }

        public class approveStatus
        {
            public string status {  get; set; }
            public int id { get; set; }
            public string approvalPerson { get; set; }

        }
        [Route("/api/v1/admin/approval")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<object>> Approve([FromBody] approveStatus msg)
        {
            if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "nbadmin")&& !this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                return Ok(new error_mb { code = 401, message = "无权访问" });
            }
            var dateString = DateTime.Now;
            using (TestDbContext sub = new TestDbContext())
            {
                var form = sub.T_Comform.Find(msg.id);
                if (form == null)
                {
                    return Ok(new error_mb { code = 400, message = "不存在这个报名" });
                }
                form.status = msg.status;
                form.approval_time = dateString.ToString();
                form.approval_person = msg.approvalPerson;
                var result = new
                {
                    code = 200,
                    message = "",
                    chinaName = form.chinaname,
                    userId = form.user_id,
                    sex = form.sex,
                    introduction = form.introduction,
                    gameId = form.game_id,
                    historyRank = form.history_rank,
                    createTime = form.create_time,
                    approvalPerson = form.approval_person,
                    approvalTime = form.approval_time,
                    status = form.status
                };
                return Ok(result);
            }
        }
        [Route("/api/v1/admin/findRegister")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<object>> GetLists([FromQuery] string chinaname = null, string status = null, short page = 1, short limit = 10)
        {
            if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "nbadmin")&& !this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                return Ok(new error_mb { code = 401, message = "无权访问" });
            }
            using (TestDbContext sub = new TestDbContext())
            {

                var query = sub.T_Comform.AsQueryable();

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
                    rows = Tasks,
                    total = TotalRecords,
                };
                return Ok(result);
            }
        }

        [Route("/api/v1/userMsg")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<object>> Mymsgs([FromQuery] int userid)
        {
            using (TestDbContext sub = new TestDbContext())
            {
                var query = sub.T_Comform.AsQueryable();
                query = query.Where(n => n.user_id == userid);              
                return query.OrderByDescending(a => a.user_id).ToList();
            }
        }
    }
}
