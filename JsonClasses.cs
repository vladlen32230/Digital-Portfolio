namespace DigitalPortfolioProject
{
    public class SearchInfo
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Photo { get; set; } = null!;
        public Guid Id { get; set; }
        public Guid Author_id { get; set; }
    }

    public class UserInfo
    {
        public string Name { get; set; } = null!;
        public string Id { get; set; } = null!;
    }

    public class PortfolioInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Author_name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Reference { get; set; } = null!;
        public string Project { get; set; } = null!;
        public string Photo { get; set; } = null!;
        public int Rating { get; set; }
        public string Author_rate { get; set; } = null!;
        public List<CommentaryInfo> Commentaries { get; set; } = null!;
    }

    public class ProfileInfo
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string First_skill { get; set; } = null!;
        public string Second_skill { get; set; } = null!;
        public string Third_skill { get; set; } = null!;
        public string Telegram { get; set; } = null!;
        public string Vk { get; set; } = null!;
        public string Photo { get; set; } = null!;
        public Guid Id { get; set; }
        public List<PortfolioInfo> Portfolio_infos { get; set; } = null!;
    }

    public class CommentaryInfo
    {
        public Guid Id { get; set; }
        public Guid User_id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Photo { get; set; } = null!;
        public bool Is_owner { get; set; }
    }
}
