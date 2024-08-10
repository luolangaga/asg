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
                a = storeinfo;
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
                var a= sb.T_Store.ToList().GroupBy(a => a.Type);
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
                var a = new Store_record(null,null);
                IQueryable<StoreinfoDB> b;
                if (showVerification)
                {
                     b = sb.T_Storeinfo;
                }
                else
                {
                    b = sb.T_Storeinfo.Where(a=>a.isVerification==false);
                }
               if (search_id == null)
                {
                  return Ok(await b.Paginate(pageindex, pagesize).ToListAsync())  ;
                }
                else
                {
                    return Ok(await b.Where(a=>a.buyerid==search_id).Paginate(pageindex, pagesize).ToListAsync());
                }
            }
        }
        public record Store_record(long? allstort,List<StoreinfoDB>? Storeinfos);
        [Route("api/v1/Store/Buy")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<object>> BuyStore(long storeid)
        {
            string id = this.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var user = await userManager.FindByIdAsync(id);

            if (user.officium != "Commentator")
            {
                return BadRequest(new error_mb { code = 400, message = $"你是{user.officium},你不是解说，无法操作" });

            }

            using (TestDbContext sb = new TestDbContext())
            {
                var stort= await sb.T_Store.FindAsync(storeid);
                try
                {
                    user.Integral = cut_value((long)user.Integral,stort.Price);
                    await userManager.UpdateAsync(user);
                    await sb.T_Storeinfo.AddAsync(new StoreinfoDB { buyerid = id.ToInt64(), Store = stort });
                    await sb.SaveChangesAsync();
                    return Ok("购买成功，请前往背包查看");
                }
                catch
                {
                    return BadRequest(new error_mb { code = 400, message = $"你的金钱无法满足你完成以下操作" });

                }
            }
        }
    }
}
