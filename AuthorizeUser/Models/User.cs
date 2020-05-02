namespace AuthorizeUser.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string[] Roles { get; set; }
    }
}
