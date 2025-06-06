namespace InfoSurge.Core.DTOs.User
{
	public class UserDto
	{
		public string Id { get; set; }

		public string UserName { get; set; }

		public string Email { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public List<string> Roles { get; set; } = new List<string>();

		public int Status { get; set; }
	}
}
