namespace Builder.Domain.Models
{
    public class Author
    {
        public string Name { get; set; }
        public string Avatar { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }
        public string Email { get; set; }
        public IEnumerable<AuthorSocialMedia> SocialMedia { get; set; }
    }
}
