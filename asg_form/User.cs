﻿
using System.ComponentModel;
using asg_form.Controllers;
using asg_form.Controllers.Team;
using Microsoft.AspNetCore.Identity;

namespace asg_form
{
    public class User : IdentityUser<long>
    {
        public string? UserBase64 { get; set; }
      
      [DefaultValue(0)]
        public long? Integral { get; set; } 
        public DateTime CreationTime { get; set; }

        public form? haveform { get; set; }
        public T_Team myteam { get; set; }

        public bool? isbooking { get; set; }
        public string? chinaname { get; set; }

        public string? officium { get; set; }

        public int point {  get; set; }

        //public bool isadmin { get; set; }
        //public List<string>? Roles { get; set; }
    }

    public class Role : IdentityRole<long>
    {
        public string msg { get; set; }
    }






}
