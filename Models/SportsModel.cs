namespace ApiWebApplication.Models
{
    public class SportsModel
    {
        public int SportId { get; set; }
        public string SportName { get; set; }
        public int TournamentId { get; set; }
        public string TournamentName { get; set; }
        public List<Player> players { get; set; }
    }
    public class Player
    {
        public int Score { get; set; }
        public string Name { get; set; }
        public Player(int score, string name)
        {
            Score = score;
            Name = name;
        }
    }
    public class DataVal
    {
        public string SportId { get; set; }
        public string TournamentId { get; set; }
        public string PlayerId { get; set; }
        public string Score { get; set; }
    }
}
