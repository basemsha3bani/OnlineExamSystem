using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;


namespace Domain.DTOs
{
     public class AttemptSnapshotDTO
    {
        [System.Text.Json.Serialization.JsonInclude]
        public List<AttemptSectionDTO> Sections { get; private set; } = new List<AttemptSectionDTO>();
        [JsonIgnore]
        public IEnumerable<AttemptQuestionDTO> Questions => Sections.SelectMany(s => s.Questions);
    
        [System.Text.Json.Serialization.JsonConstructor]

        private AttemptSnapshotDTO() { }
        public static AttemptSnapshotDTO Define(List<AttemptSectionDTO> Sections = default)
        {
            var value = new AttemptSnapshotDTO();
            value.Sections = Sections ?? new List<AttemptSectionDTO>();
            return value;
        }
        public void ChangeSections( List<AttemptSectionDTO> value) => Sections = value;
}
    public class AttemptSectionDTO
    {
        [System.Text.Json.Serialization.JsonInclude]
        public string Name { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public List<AttemptQuestionDTO> Questions { get; private set; } = new List<AttemptQuestionDTO>();
    
        [System.Text.Json.Serialization.JsonConstructor]

        private AttemptSectionDTO() { }
        public static AttemptSectionDTO Define(string Name = default, List<AttemptQuestionDTO> Questions = default)
        {
            var value = new AttemptSectionDTO();
            value.Name = Name;
            value.Questions = Questions ?? new List<AttemptQuestionDTO>();
            return value;
        }
        public void ChangeName( string value) => Name = value;
        public void ChangeQuestions( List<AttemptQuestionDTO> value) => Questions = value;
}
    public class AttemptQuestionDTO
    {
        [System.Text.Json.Serialization.JsonInclude]
        public int Id { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public string Text { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public int? SelectedOptionId { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public List<AttemptOptionDTO> Options { get; private set; } = new List<AttemptOptionDTO>();
    
        [System.Text.Json.Serialization.JsonConstructor]

        private AttemptQuestionDTO() { }
        public static AttemptQuestionDTO Define(int Id = default, string Text = default, int? SelectedOptionId = default, List<AttemptOptionDTO> Options = default)
        {
            var value = new AttemptQuestionDTO();
            value.Id = Id;
            value.Text = Text;
            value.SelectedOptionId = SelectedOptionId;
            value.Options = Options ?? new List<AttemptOptionDTO>();
            return value;
        }
        public void ChangeId( int value) => Id = value;
        public void ChangeText( string value) => Text = value;
        public void ChangeSelectedOptionId( int? value) => SelectedOptionId = value;
        public void ChangeOptions( List<AttemptOptionDTO> value) => Options = value;
}
    public class AttemptOptionDTO
    {
        [System.Text.Json.Serialization.JsonInclude]
        public int Id { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public string Text { get; private set; }
        [System.Text.Json.Serialization.JsonInclude]
        public bool IsCorrect { get; private set; }
    
        [System.Text.Json.Serialization.JsonConstructor]

        private AttemptOptionDTO() { }
        public static AttemptOptionDTO Define(int Id = default, string Text = default, bool IsCorrect = default)
        {
            var value = new AttemptOptionDTO();
            value.Id = Id;
            value.Text = Text;
            value.IsCorrect = IsCorrect;
            return value;
        }
        public void ChangeId( int value) => Id = value;
        public void ChangeText( string value) => Text = value;
        public void ChangeIsCorrect( bool value) => IsCorrect = value;
}
    public static class AttemptScoringDTO
    {
        public static int CountCorrect(AttemptSnapshotDTO snapshot) => snapshot.Questions.Count(q =>
            q.Options.Any(o => o.Id == q.SelectedOptionId && o.IsCorrect));
        public static decimal Ratio(int correct, int total)
        {
            if (total <= 0) throw new Domain.Exceptions.DomainException("An attempt must contain questions.");
            return (decimal)correct / total;
        }
    }
}
