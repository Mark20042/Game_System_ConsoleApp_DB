partial class Program
{
    class GameRecord
    {
        public int GameId { get; set; }
        public string SeasonName { get; set; }
        public string TeamAName { get; set; }
        public string TeamBName { get; set; }
        public int ScoreA { get; set; }
        public int ScoreB { get; set; }
        public DateTime StartTime { get; set; }
    }
}