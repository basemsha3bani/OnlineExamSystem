using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ServicesClasseslibrary.Examiner
{
    public class AttemptSnapshot
    {
        public List<AttemptSection> Sections { get; set; } = new List<AttemptSection>();
        [JsonIgnore]
        public IEnumerable<AttemptQuestion> Questions => Sections.SelectMany(s => s.Questions);
    }
    public class AttemptSection
    {
        public string Name { get; set; }
        public List<AttemptQuestion> Questions { get; set; } = new List<AttemptQuestion>();
    }
    public class AttemptQuestion
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int? SelectedOptionId { get; set; }
        public List<AttemptOption> Options { get; set; } = new List<AttemptOption>();
    }
    public class AttemptOption
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
    public static class AttemptScoring
    {
        public static int CountCorrect(AttemptSnapshot snapshot) => snapshot.Questions.Count(q =>
            q.Options.Any(o => o.Id == q.SelectedOptionId && o.IsCorrect));
        public static decimal Ratio(int correct, int total)
        {
            if (total <= 0) throw new System.InvalidOperationException("An attempt must contain questions.");
            return (decimal)correct / total;
        }
    }
}
