using asg_form.Controllers.Team;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO.Compression;
using System.Security.Claims;
using static asg_form.Controllers.login;

namespace asg_form.Controllers
{
    public class excel : ControllerBase
    {

        private readonly RoleManager<Role> roleManager;
        private readonly UserManager<User> userManager;
        public excel(
            RoleManager<Role> roleManager, UserManager<User> userManager)
        {

            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        /// <summary>
        /// 通过战队名搜索一个战队的详细信息
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>





        public static void ExportToExcel_noadmin(List<T_Team> data, string fileName)
        {
            using (var package = new ExcelPackage())
            {
                foreach (var form in data)
                {
                    var worksheet = package.Workbook.Worksheets.Add(form.team_name);
                    worksheet.Cells[1, 1].Value = "队伍logo";
                    worksheet.Cells[1, 2].Value = "队伍ID";
                    worksheet.Cells[1, 3].Value = "队伍名称";
                    worksheet.Cells[1, 4].Value = "队长联系方式";
                    worksheet.Cells[1, 5].Value = "队伍票数";
                    int a = 1;
                    while (a <= 5)
                    {
                        worksheet.Cells[1, a].Style.Font.Bold = true;
                        worksheet.Cells[1, a].Style.Font.Color.SetColor(Color.White);
                        worksheet.Cells[1, a].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, a].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                        a++;
                    }
                    var pic = worksheet.Drawings.AddPicture(form.team_name, $@"{AppDomain.CurrentDomain.BaseDirectory}loge\{form.team_name}.png");
                    pic.SetSize(50, 50);
                    pic.SetPosition(26, 0);
                    worksheet.Cells[2, 2].Value = form.Id;
                    worksheet.Cells[2, 3].Value = form.team_name;
                    worksheet.Cells[2, 4].Value = form.team_tel;
                    worksheet.Cells[2, 5].Value = form.piaoshu;
                    int b = 1;
                    while (b <= 6)
                    {
                        worksheet.Cells[5, b].Style.Font.Bold = true;
                        worksheet.Cells[5, b].Style.Font.Color.SetColor(Color.White);
                        worksheet.Cells[5, b].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[5, b].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                        b++;
                    }

                    worksheet.Cells[5, 1].Value = "队员ID";
                    worksheet.Cells[5, 2].Value = "队员名称";
                    worksheet.Cells[5, 3].Value = "队伍第五人格名称";
                    worksheet.Cells[5, 4].Value = "队员阵营";
                    worksheet.Cells[5, 5].Value = "队员第五人格ID";
                    worksheet.Cells[5, 6].Value = "队员历史段位";
                    for (int i = 0; i < form.role.Count; i++)
                    {
                        var role = form.role[i];
                        var dow = i + 6;
                        worksheet.Cells[dow, 1].Value = role.Id;
                        worksheet.Cells[dow, 2].Value = role.role_name;
                        worksheet.Cells[dow, 3].Value = role.Game_Name;
                        worksheet.Cells[dow, 4].Value = role.role_lin;
                        worksheet.Cells[dow, 5].Value = role.role_id;
                        worksheet.Cells[dow, 6].Value = role.Historical_Ranks;
                    }
                }
                FileInfo excelFile = new FileInfo(fileName);
                package.SaveAs(excelFile);
            }
        }












        public static void ExportToExcel(List<T_Team> data, string fileName)
        {
            using (var package = new ExcelPackage())
            {
                foreach (var form in data)
                {
                    var worksheet = package.Workbook.Worksheets.Add(form.team_name);
                    worksheet.Cells[1, 1].Value = "队伍logo";
                    worksheet.Cells[1, 2].Value = "队伍ID";
                    worksheet.Cells[1, 3].Value = "队伍名称";
                    worksheet.Cells[1, 4].Value = "队长联系方式";
                    worksheet.Cells[1, 5].Value = "队伍密码";
                    worksheet.Cells[1, 6].Value = "队伍票数";


                    int a = 1;
                    while (a <= 6)
                    {
                        worksheet.Cells[1, a].Style.Font.Bold = true;
                        worksheet.Cells[1, a].Style.Font.Color.SetColor(Color.White);
                        worksheet.Cells[1, a].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[1, a].Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                        a++;
                    }
                    var pic = worksheet.Drawings.AddPicture(form.team_name, $@"{AppDomain.CurrentDomain.BaseDirectory}loge\{form.team_name}.png");
                    pic.SetSize(50, 50);
                    pic.SetPosition(26, 0);
                    worksheet.Cells[2, 2].Value = form.Id;
                    worksheet.Cells[2, 3].Value = form.team_name;
                    worksheet.Cells[2, 4].Value = form.team_tel;
                    worksheet.Cells[2, 5].Value = form.team_password;
                    worksheet.Cells[2, 6].Value = form.piaoshu;


                    worksheet.Cells[5, 1].Value = "队员ID";
                    worksheet.Cells[5, 2].Value = "队员名称";
                    worksheet.Cells[5, 3].Value = "队伍第五人格名称";
                    worksheet.Cells[5, 4].Value = "队员阵营";
                    worksheet.Cells[5, 5].Value = "队员第五人格ID";
                    worksheet.Cells[5, 6].Value = "队员身份证号码";
                    worksheet.Cells[5, 7].Value = "队员常用角色";
                    worksheet.Cells[5, 8].Value = "队员历史段位";
                    worksheet.Cells[5, 9].Value = "队员真实姓名";
                    worksheet.Cells[5, 10].Value = "队员姓名";
                    int b = 1;
                    while (b <= 10)
                    {
                        worksheet.Cells[5, b].Style.Font.Bold = true;
                        worksheet.Cells[5, b].Style.Font.Color.SetColor(Color.White);
                        worksheet.Cells[5, b].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        worksheet.Cells[5, b].Style.Fill.BackgroundColor.SetColor(Color.LightGreen);
                        b++;
                    }
                    for (int i = 0; i < form.role.Count; i++)
                    {
                        var role = form.role[i];
                        var dow = i + 6;
                        worksheet.Cells[dow, 1].Value = role.Id;
                        worksheet.Cells[dow, 2].Value = role.role_name;
                        worksheet.Cells[dow, 3].Value = role.Game_Name;
                        worksheet.Cells[dow, 4].Value = role.role_lin;
                        worksheet.Cells[dow, 5].Value = role.role_id;
                        worksheet.Cells[dow, 6].Value = role.Id_Card;
                        worksheet.Cells[dow, 7].Value = role.Common_Roles;
                        worksheet.Cells[dow, 8].Value = role.Historical_Ranks;
                        worksheet.Cells[dow, 9].Value = role.Id_Card_Name;
                        worksheet.Cells[dow, 10].Value = role.Phone_Number;
                    }
                }
                FileInfo excelFile = new FileInfo(fileName);
                package.SaveAs(excelFile);
            }
        }


        [Authorize]
        [Route("api/v1/admin/form/{search}")]
        [HttpGet]
        public async Task<ActionResult<List<allteam>>> Get(string search)
        {

            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                TestDbContext ctx = new TestDbContext();
                List<allteam> data = new List<allteam>();
                List<form> teams = ctx.Forms.Include(a => a.role).Where(a => a.team_name.IndexOf(search) >= 0).ToList();
                foreach (var team in teams)
                {
                    var roles = team.role;
                    allteam allteam = new allteam();
                    allteam.Id = team.Id;
                    allteam.Name = team.team_name;
                    foreach (var role in roles)
                    {
                        role.form = null;
                        allteam.role.Add(role);
                    }
                    data.Add(allteam);
                }
                return data;
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });
            }


        }




        [Authorize]
        [Route("api/v1/admin/officium/group")]
        [HttpGet]
        public async Task<ActionResult<object>> Gethaveop()
        {

                using (TestDbContext ctx = new TestDbContext())
                {
                    object data = userManager.Users.Select(a => new { a.Id, a.UserName, a.chinaname, a.Email, a.officium }).Where(a => a.officium != null).GroupBy(a => a.officium).ToList();
                    return data;
                }

            

        }





        [Authorize]
        [Route("api/v1/admin/user/{search}")]
        [HttpGet]
        public async Task<ActionResult<List<post_user>>> searchuser(string search)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "nbadmin"))
            {
                TestDbContext ctx = new TestDbContext();
                List<post_user> data = new List<post_user>();
                List<User> users = userManager.Users.Where(a => a.UserName.IndexOf(search) >= 0).ToList();
                foreach (var user1 in users)
                {
                    post_user post_User = new post_user();
                    post_User.id = user1.Id;
                    post_User.name = user1.UserName;
                    post_User.email = user1.Email;
                    post_User.officium = user1.officium;
                    post_User.chinaname = user1.chinaname;

                    post_User.Base64 = user1.UserBase64;
                    post_User.Roles = (List<string>?)await userManager.GetRolesAsync(user1);
                    data.Add(post_User);
                }
                return data;
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });
            }
        }

        [Authorize]
        [Route("api/v1/admin/user/email/{search}")]
        [HttpGet]
        public async Task<ActionResult<List<post_user>>> searchuser_byemail(string search)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                TestDbContext ctx = new TestDbContext();
                List<post_user> data = new List<post_user>();
                List<User> users = userManager.Users.Where(a => a.Email.IndexOf(search) >= 0).ToList();
                foreach (var user1 in users)
                {
                    post_user post_User = new post_user();
                    post_User.id = user1.Id;
                    post_User.name = user1.UserName;
                    post_User.email = user1.Email;
                    post_User.officium = user1.officium;
                    post_User.chinaname = user1.chinaname;

                    post_User.Base64 = user1.UserBase64;
                    post_User.Roles = (List<string>?)await userManager.GetRolesAsync(user1);
                    data.Add(post_User);
                }
                return data;
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });
            }
        }

        [Authorize]
        [Route("api/v1/admin/user/chinaname/{search}")]
        [HttpGet]
        public async Task<ActionResult<List<post_user>>> searchuser_bychinaname(string search)
        {

            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                TestDbContext ctx = new TestDbContext();
                List<post_user> data = new List<post_user>();
                List<User> users = userManager.Users.Where(a => a.chinaname.IndexOf(search) >= 0).ToList();
                foreach (var user1 in users)
                {
                    post_user post_User = new post_user();
                    post_User.id = user1.Id;
                    post_User.name = user1.UserName;
                    post_User.email = user1.Email;
                    post_User.officium = user1.officium;
                    post_User.chinaname = user1.chinaname;

                    post_User.Base64 = user1.UserBase64;
                    post_User.Roles = (List<string>?)await userManager.GetRolesAsync(user1);
                    data.Add(post_User);
                }
                return data;
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });
            }
        }


        [Route("api/v1/admin/excel")]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<List<post_user>>> excel1(string event_name)
        {
            string guid = Guid.NewGuid().ToString();
            TestDbContext testDb = new TestDbContext();
            List<T_Team> result = testDb.Teams.ToList();
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "nbadmin"))
            {

                ExportToExcel(result, $"{AppDomain.CurrentDomain.BaseDirectory}excel/{guid}.xlsx");
                return Ok(guid);
            }
            else if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                ExportToExcel_noadmin(result, $"{AppDomain.CurrentDomain.BaseDirectory}excel/{guid}.xlsx");
                return Ok(guid);
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无管理员" });
            }
        }



        [Route("api/v1/admin/excel/")]
        [HttpDelete]
        [Authorize]
        public ActionResult<List<string>> delexcel()
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                DirectoryInfo di = new DirectoryInfo($@"{AppDomain.CurrentDomain.BaseDirectory}excel");
                FileInfo[] files = di.GetFiles();
                foreach (var file in files)
                {
                    System.IO.File.Delete(file.FullName);
                }
                return Ok("ok");
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无管理员" });
            }
        }



        [Route("api/v1/admin/excel/all")]
        [HttpGet]
        [Authorize]
        public ActionResult<List<string>> allexcel()
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))
            {
                DirectoryInfo di = new DirectoryInfo($@"{AppDomain.CurrentDomain.BaseDirectory}video");
                FileInfo[] files = di.GetFiles();

                List<string> result = new List<string>();
                foreach (FileInfo file in files)
                {
                    result.Add(file.Name);

                }
                return result;
            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无管理员" });
            }
        }


        /// <summary>
        /// 将指定目录压缩为Zip文件
        /// </summary>
        /// <param name="folderPath">文件夹地址 D:/1/ </param>
        /// <param name="zipPath">zip地址 D:/1.zip </param>
        public static void CompressDirectoryZip(string folderPath, string zipPath)
        {
            DirectoryInfo directoryInfo = new(zipPath);

            if (directoryInfo.Parent != null)
            {
                directoryInfo = directoryInfo.Parent;
            }

            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }

            ZipFile.CreateFromDirectory(folderPath, zipPath, System.IO.Compression.CompressionLevel.Optimal, false);
        }
        [Route("api/v1/admin/form_count")]
        [HttpGet]
        [ResponseCache(Duration = 30)]
        public async Task<ActionResult<string>> form_count()
        {
            Dictionary<string, int> keys = new Dictionary<string, int>();

            using (TestDbContext ctx = new TestDbContext())
            {
                var all_event = await ctx.events.ToListAsync();
                foreach (var eventt in all_event)
                {

                    keys.Add(eventt.name, ctx.Forms.Where(a => a.events == eventt).Count());
                }
            }
            return Ok((object)keys);
        }



        [Route("api/v1/admin/img_zip")]
        [HttpGet]
        [ResponseCache(Duration = 1720)]
        public async Task<ActionResult<string>> img_zip()
        {
            string guid = Guid.NewGuid().ToString();
            CompressDirectoryZip($"{AppDomain.CurrentDomain.BaseDirectory}loge/", $"{AppDomain.CurrentDomain.BaseDirectory}doc/{guid}.zip");
            return $"/doc/{guid}.zip";
        }



        public class team_count
        {
            public string eventname { get; set; }
            public int formnumber { get; set; }

        }

        /// <summary>
        /// 获得所有战队信息
        /// </summary>
        /// <param name="page"></param>
        /// <param name="page_long"></param>
        /// <returns></returns>
        [Authorize]
        [Route("api/v1/admin/form/all")]
        [HttpGet]
        public async Task<ActionResult<List<form>>> Post(string events)
        {
            if (this.User.FindAll(ClaimTypes.Role).Any(a => a.Value == "admin"))

            {

                TestDbContext ctx = new TestDbContext();

                var teams = ctx.Forms.Include(a => a.role).Include(a => a.events).Where(a => a.events.name == events).ToList();
                foreach (var team in teams)
                {
                    team.events.forms = null;
                    foreach (var role in team.role)
                    {
                        role.form = null;
                    }
                }

                return teams;









            }
            else
            {
                return BadRequest(new error_mb { code = 400, message = "无权访问" });

            }


        }







        public class allteam
        {
            /// <summary>
            /// 战队id
            /// </summary>
            public long Id { get; set; }
            /// <summary>
            /// 战队名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 战队的角色
            /// </summary>
            public List<role> role { get; set; } = new List<role>();
        }




    }
}