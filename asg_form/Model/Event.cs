using asg_form.Controllers;

namespace asg_form.Model
{
    public class T_events
    {
        public int Id { get; set; }
        public string? name { get; set; }
        public bool? is_over { get; set; }
        public DateTime? opentime { get; set; }

        public List<form>? forms { get; set; }
        public Uri? events_rule_uri { get; set; }

    }
}
