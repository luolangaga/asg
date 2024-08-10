using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace asg_form.Controllers;

public class T_config
{
   public int Id { set; get; }
   public string Title{ set; get; }
   public string Substance{ set; get;}
   public string? msg{ set; get; }


}



[ApiController]
[Route("api/[controller]")]
public class config : ControllerBase
{
    [Route("api/v1/admin/config")]
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<object>> config_post([FromBody] T_config config)
    {

        if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
        {
            return BadRequest(new error_mb { code = 400, message = "无权访问" });
        }
using(TestDbContext db=new TestDbContext()){
            var config__ = db.T_config.FirstOrDefault(a => a.Id == config.Id);
    if (config__==null)
    {
                db.T_config.Add(config);
                await db.SaveChangesAsync();
            }
    else
    {
                var config_ = db.T_config.FirstOrDefault(a => a.Id == config.Id);

                config_.msg=config.msg;
                config_.Substance = config.Substance;
                config_.Title = config.Title;
                await db.SaveChangesAsync();
            }
           
}
        
     
         return Ok("添加成功！");

    }




      


    [Route("api/v1/admin/config/byTitle")]
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<object>> config_get_title([FromBody] string title)
    {

        if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
        {
            return BadRequest(new error_mb { code = 400, message = "无权访问" });
        }
        using (TestDbContext db = new TestDbContext())
        {
            var config = db.T_config.FirstOrDefault(a => a.Title == title);
           return Ok(config.Substance);
        }

    }

    [Route("api/v1/admin/config")]
    [HttpDelete]
    [Authorize]
    public async Task<ActionResult<object>> config_get_title(int Id)
    {

        if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
        {
            return BadRequest(new error_mb { code = 400, message = "无权访问" });
        }
        using (TestDbContext db = new TestDbContext())
        {
            var config = db.T_config.FirstOrDefault(a => a.Id==Id);
            db.Remove(config);
            await db.SaveChangesAsync();
            return Ok("成功！");
        }

    }





    [Route("api/v1/admin/config/all")]
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<object>> config_get_all(short page, short page_long = 10)
    {
       
        if (!this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
        {
            return BadRequest(new error_mb { code = 400, message = "无权访问" });
        }
        using (TestDbContext db = new TestDbContext())
        {
            int a = db.T_config.Count();
            int b = page_long * page;
            if (page_long * page > a)
            {
                b = a;
            }
            object config = db.T_config.Skip(page_long * page - page_long).Take(page_long).ToList();
            return Ok(config);
        }

    }



}
