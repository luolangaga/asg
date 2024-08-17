namespace asg_form.Controllers.Store
{
    public class StoreDB
    {
        public long id { get; set; }
        public string Name { get; set; }
        public long Price { get; set; }

        public string description { get; set; }

        public string information { get; set; }
        public string Type { get; set; }
        public List<StoreinfoDB>? buyer {  get; set; }=new List<StoreinfoDB>();
    }
    public class StoreinfoDB
    {
        public long id { get; set; }
        public long buyerid { get; set; }
        public StoreDB Store { get; set; }

         public bool isVerification {  get; set; }


    }
}
