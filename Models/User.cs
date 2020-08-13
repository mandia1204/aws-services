namespace Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string[] Roles { get; set; }
        public string Default { get; set; }
    }
}
