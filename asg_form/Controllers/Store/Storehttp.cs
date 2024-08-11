using Masuit.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace asg_form.Controllers.Store
{
    public class Storehttp : ControllerBase
    {
        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        public Storehttp(
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {

            this.roleManager = roleManager;
            this.userManager = userManager;
        }



        [Route("api/v1/admin/Store")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<object>> AddStore([FromBody]StoreDB storeinfo)
        {
            if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });
            }
            using (TestDbContext sb = new TestDbContext()) { 
            sb.T_Store.Add(storeinfo);
             await sb.SaveChangesAsync();
                return Ok(storeinfo);
            }
        }
        [Route("api/v1/admin/Store")]
        [HttpDelete]
        [Authorize]
        public async Task<ActionResult<object>> DelStore(long id)
        {
            if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });
            }
            using (TestDbContext sb = new TestDbContext())
            {
               sb.T_Store.Remove(sb.T_Store.Find(id));
                await sb.SaveChangesAsync();
                return Ok("ok");
            }
        }
        [Route("api/v1/admin/Store")]
        [HttpPut]
        [Authorize]
        public async Task<ActionResult<object>> putStore([FromBody] StoreDB storeinfo)
        {
            if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });
            }
            using (TestDbContext sb = new TestDbContext())
            {
                var a= await sb.T_Store.FindAsync(storeinfo.id);
                a.Name=storeinfo.Name;
                a.description=storeinfo.description;
                a.information=storeinfo.information;
                a.Price=storeinfo.Price;
                await sb.SaveChangesAsync();
                return Ok(storeinfo);
            }
        }
        public long cut_value(long value,long money)
        {
            long _value = value;
            value = value - money;
            if (value < 0)
            {
                throw new ArgumentException("你已经没钱啦！");

            }
            return value;
        }
        [Route("api/v1/Store")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<object>> GetStore()
        {
          
            using (TestDbContext sb = new TestDbContext())
            {
                var a= sb.T_Store.ToList();
               return Ok(a);
            }
        }

        [Route("api/v1/Store/Verification")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<object>> Verification(long storeinfoid)
        {

            if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });
            }
            using (TestDbContext sb = new TestDbContext())
            {
                var a = sb.T_Storeinfo.Find(storeinfoid);
                a.isVerification = true;
                await sb.SaveChangesAsync();
                return Ok(a);
            }
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="search"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="showVerification">是否展示以及核销过的</param>
        /// <returns></returns>
        [Route("api/v1/admin/Storeinfo")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<object>> GetStoreinfo(bool showVerification,long? search_id,int pageindex=0,int pagesize=10)
        {
            if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });
            }
            using (TestDbContext sb = new TestDbContext())
            {
                var a = new all_record();
                IQueryable<StoreinfoDB> b;
                if (showVerification)
                {
                     b = sb.T_Storeinfo.Include(a=>a.Store);
                }
                else
                {
                    b = sb.T_Storeinfo.Include(a => a.Store).Where(a => a.isVerification == false);
                }
               if (search_id == null)
                {
                    a.cout = b.Count();
                    a.msg = await b.Paginate(pageindex, pagesize).Select(a => new { a.id, a.buyerid, a.Store.Price, a.Store.description, a.isVerification, a.Store.information, a.Store.Name }).ToListAsync();
                 
                }
                else
                {
                    a.cout = b.Where(a => a.buyerid == search_id).Count();
                    a.msg = await b.Where(a => a.buyerid == search_id).Paginate(pageindex, pagesize).Select(a => new { a.id, a.buyerid, a.Store.Price, a.Store.description, a.isVerification, a.Store.information, a.Store.Name }).ToListAsync();
                }
               
                return Ok(a);
            }
        }
        public record buyreq_record(bool iserror, string msg);


        public record all_record()
        {
            public long? cout { get; set; } 
        public   object msg { get; set; }
        }
        [Route("api/v1/Store/Buy")]
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<object>> BuyStore([FromBody]long[] storeid)
        {
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);

            if (user.officium != "Commentator")
            {
                return BadRequest(new error_mb { code = 400, message = $"你是{user.officium},你不是解说，无法操作" });

            }

            using (TestDbContext sb = new TestDbContext())
            {
                List<buyreq_record> bureq = new List<buyreq_record>();
                foreach (var item in storeid)
                {
                    var stort = await sb.T_Store.FindAsync(item);
                    try
                    {
                        user.Integral = cut_value((long)user.Integral, stort.Price);
                        await userManager.UpdateAsync(user);
                        await sb.T_Storeinfo.AddAsync(new StoreinfoDB { buyerid = id.ToInt64(), Store = stort });
                        await sb.SaveChangesAsync();
                        bureq.Add(new buyreq_record(false, $"购买{stort.Name}成功"));
                       
                    }
                    catch
                    {
                        bureq.Add(new buyreq_record(true, $"购买失败，因为余额不足"));


                    }
                }

             return Ok(bureq);
            }
        }
    }
}
