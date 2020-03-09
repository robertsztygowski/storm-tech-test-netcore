namespace Todo.Models.TodoItems
{
    public class UserSummaryViewmodel
    {
        public string UserName { get; set; }
        public string Email { get; set;  }
        public string ThumbnailUrl { get; set; }
        public bool HasGravatarProfile { get; set; }

        public UserSummaryViewmodel(string userName, string email)
        {
            UserName = userName;
            Email = email;
        }
    }
}