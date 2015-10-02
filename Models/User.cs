namespace Team.Models
{
	public class User
	{
		public int UserId { get; set; }
		
		public string Identifier { get; set; }
		
		public string Name { get; set; }
		
		public string Email { get; set; }
		
		public AccountStatus Status { get; set; }
		
		public bool IsSurveyor { get; set; }
		
		public string Password { get; set; }
	}

    public enum AccountStatus
    {
        Active = 1,

        Blocked = 2,

        Deleted = 9,

        Invited = 3,

        Unverified = 4
    }
}