using System.Text.Json;
using Domain.Exceptions;
using Domain.DTOs;
using Domain.Entities;
using System;

namespace Domain.Aggregates
{
    // The snapshot retains the exact sections, questions, options and answer key at start time.
    public class ExamAttempt : Domain.Abstractions.IAggregateRoot
    {
        public int Id { get; private set; }
        public int ExamId { get; private set; }
        public int UserId { get; private set; }
        public string ExamTitle { get; private set; }
        public string SnapshotJson { get; private set; }
        public string Status { get; private set; } = "InProgress";
        public DateTime StartedAt { get; private set; }
        public DateTime? SubmittedAt { get; private set; }
        public DateTime? EvaluatedAt { get; private set; }
        public int TotalQuestions { get; private set; }
        public int? CorrectQuestions { get; private set; }
        public decimal? Score { get; private set; }

        public byte[] Version { get; private set; }

        private ExamAttempt() { }
        public static ExamAttempt Define(int Id = default, int ExamId = default, int UserId = default, string ExamTitle = default, string SnapshotJson = default, string Status = default, DateTime StartedAt = default, DateTime? SubmittedAt = default, DateTime? EvaluatedAt = default, int TotalQuestions = default, int? CorrectQuestions = default, decimal? Score = default, byte[] Version = default)
        {
            var value = new ExamAttempt();
            value.ChangeId(Id);
            value.ChangeExamId(ExamId);
            value.ChangeUserId(UserId);
            value.ChangeExamTitle(ExamTitle);
            value.ChangeSnapshotJson(SnapshotJson);
            value.ChangeStatus(Status ?? "InProgress");
            value.ChangeStartedAt(StartedAt);
            value.ChangeSubmittedAt(SubmittedAt);
            value.ChangeEvaluatedAt(EvaluatedAt);
            value.ChangeTotalQuestions(TotalQuestions);
            value.ChangeCorrectQuestions(CorrectQuestions);
            value.ChangeScore(Score);
            value.ChangeVersion(Version);
            return value;
        }
        public void ChangeId(int value) => Id = EntityRules.NonNegative(value, "Id");
        public void ChangeExamId(int value) => ExamId = EntityRules.NonNegative(value, "ExamId");
        public void ChangeUserId(int value) => UserId = EntityRules.NonNegative(value, "UserId");
        public void ChangeExamTitle(string value) => ExamTitle = value;
        public void ChangeSnapshotJson(string value) => SnapshotJson = value;
        public void ChangeStatus(string value) => Status = value;
        public void ChangeStartedAt(DateTime value) => StartedAt = value;
        public void ChangeSubmittedAt(DateTime? value) => SubmittedAt = value;
        public void ChangeEvaluatedAt(DateTime? value) => EvaluatedAt = value;
        public void ChangeTotalQuestions(int value) => TotalQuestions = value;
        public void ChangeCorrectQuestions(int? value) => CorrectQuestions = value;
        public void ChangeScore(decimal? value) => Score = value;
        public void ChangeVersion(byte[] value) => Version = value;

        public static ExamAttempt Restore(ExamAttemptDTO model)
        {
            EntityRules.Exists(model, "Attempt");
            return Define(Id: model.Id, ExamId: model.ExamId, UserId: model.UserId, ExamTitle: model.ExamTitle,
            SnapshotJson: model.SnapshotJson, Status: model.Status, StartedAt: model.StartedAt, SubmittedAt: model.SubmittedAt,
            EvaluatedAt: model.EvaluatedAt, TotalQuestions: model.TotalQuestions, CorrectQuestions: model.CorrectQuestions,
            Score: model.Score, Version: model.Version);
        }

        public ExamAttemptDTO ToDTO() => ExamAttemptDTO.Define(Id: Id, ExamId: ExamId, UserId: UserId, ExamTitle: ExamTitle,
        SnapshotJson: SnapshotJson, Status: Status, StartedAt: StartedAt, SubmittedAt: SubmittedAt, EvaluatedAt: EvaluatedAt,
        TotalQuestions: TotalQuestions, CorrectQuestions: CorrectQuestions, Score: Score, Version: Version);

        public AttemptSnapshotDTO ReadSnapshot() => JsonSerializer.Deserialize<AttemptSnapshotDTO>(SnapshotJson)
        ?? throw new DomainException("Attempt snapshot is required.");

        public static AttemptSnapshotDTO Read(ExamAttemptDTO model) => Restore(model).ReadSnapshot();

        public static ExamAttempt Start(int examId, int userId, IEnumerable<ExamSectionsDTO> sections,
        Dictionary<int, List<QuestionsDTO>> selectedBySection)
        {
            EntityRules.Positive(examId, "Exam"); EntityRules.Positive(userId, "User");
            var configured = sections.ToList();
            if (configured.Count == 0) throw new DomainException("The selected exam does not exist.");
            var snapshot = AttemptSnapshotDTO.Define();
            foreach (var section in configured)
            {
                if (!selectedBySection.TryGetValue(section.Id, out var selected) || selected.Count == 0)
                throw new DomainException("Each exam section must contain configured questions.");
                var saved = AttemptSectionDTO.Define(Name: section.SectionName);
                foreach (var question in selected)
                {
                    Questions.CheckAnswers(question.QuestionAnswersDTO);
                    saved.Questions.Add(AttemptQuestionDTO.Define(Id: question.Id, Text: question.QuestionText,
                    Options: question.QuestionAnswersDTO.Select(a => AttemptOptionDTO.Define(Id: a.Id, Text: a.AnswerText, IsCorrect: a.IsCorrect)).ToList()));
                }
                snapshot.Sections.Add(saved);
            }
            return Define(ExamId: examId, UserId: userId, ExamTitle: configured[0].exam.Title,
            StartedAt: DateTime.UtcNow, TotalQuestions: snapshot.Questions.Count(), SnapshotJson: JsonSerializer.Serialize(snapshot));
        }

        public bool SaveAnswers(int sectionIndex, Dictionary<int, int?> answers, bool submit)
        {
            if (Status != "InProgress") return false;
            var snapshot = ReadSnapshot();
            if (sectionIndex < 0 || sectionIndex >= snapshot.Sections.Count) throw new DomainException("Invalid section.");
            var questions = snapshot.Sections[sectionIndex].Questions;
            if (answers.Keys.Any(id => questions.All(q => q.Id != id))) throw new DomainException("Invalid question.");
            foreach (var question in questions)
            if (answers.TryGetValue(question.Id, out var selected) && selected.HasValue && question.Options.All(o => o.Id != selected.Value))
            throw new DomainException("Invalid answer.");
            foreach (var question in questions)
            {
                answers.TryGetValue(question.Id, out var selected);
                question.ChangeSelectedOptionId(selected);
            }
            SnapshotJson = JsonSerializer.Serialize(snapshot);
            if (submit) { Status = "Submitted"; SubmittedAt = DateTime.UtcNow; }
            return true;
        }

        public void Evaluate()
        {
            if (Status != "Submitted") throw new DomainException("Only submitted attempts can be scored.");
            var snapshot = ReadSnapshot();
            if (snapshot.Questions.Count() != TotalQuestions) throw new DomainException("Attempt question count is inconsistent.");
            var correct = AttemptScoringDTO.CountCorrect(snapshot);
            var score = AttemptScoringDTO.Ratio(correct, TotalQuestions);
            CorrectQuestions = correct; Score = score;
            EvaluatedAt = DateTime.UtcNow; Status = "Scored";
        }

    }
}
