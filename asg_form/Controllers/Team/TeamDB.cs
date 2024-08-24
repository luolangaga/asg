namespace asg_form.Controllers.Team
{
    public class T_Team
    {

        public List<T_events> Events { get; set; }=new List<T_events>();
        public long Id { get; set; }
        public int piaoshu { get; set; }
        public DateTime time { get; set; } = DateTime.Now;
        public string team_name { get; set; }
        public string team_password { get; set; }
        public string team_tel { get; set; }
        public string logo_uri { get; set; }
        public T_events events { get; set; }

        //  public string? belong { get; set; }
        public List<T_Player> role { get; set; } = new List<T_Player>();
    }
    public class T_Player
    {
        public long Id { get; set; }

        public T_Team Team { get; set; }//属于哪个队伍

        public string role_id { get; set; } = "无";
        public string role_name { get; set; } = "无";//阵容
        public string? Game_Name { get; set; } = "未知";
        public string role_lin { get; set; }
        public string? Id_Card { get; set; } = "未知";
        public string? Common_Roles { get; set; } = "未知";
        public string? Phone_Number { get; set; } = "未知";
        public string? Id_Card_Name { get; set; } = "未知";
        public int? Historical_Ranks { get; set; } = 0;
    }
}
