using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace asg_form.Controllers
{
    public class T_gift
    {
        public long id { get; set; }
        public int probability { get; set; }
        public string gift_name {  get; set; }
        public string gitf_introduce {  get; set; }
        public string winner_name {  get; set; }
        public bool is_open {  get; set; }
        //奖品内容
        public string gift_introduce { get; set; }
    }


    public class gif : ControllerBase
    {
        [Authorize]
        [Route("api/v1/admin/gift")]
        [HttpPost]
        public object post_gitf()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<gif>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<gif>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<gif>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<gif>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
