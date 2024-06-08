using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HomeManagementApp.Models
{
    public class RankingViewModel
    {
        public List<RankingEntry> PointsRanking { get; set; }
        public List<RankingEntry> TasksRanking { get; set; }
        public List<RankingEntry> RewardsRanking { get; set; }
    }

    public class RankingEntry
    {
        [Display(Name ="Użytkownik")]
        public string? UserName { get; set; }
        public int Value { get; set; }
    }
}
