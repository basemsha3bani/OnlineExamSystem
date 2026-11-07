DECLARE @examId INT = 2;

INSERT INTO ExamQuestions (ExamId, QuestionId)
SELECT @examId, Q.Id
FROM (
    SELECT q.Id, q.DifficultyLevelId,
           ROW_NUMBER() OVER (PARTITION BY q.DifficultyLevelId ORDER BY NEWID()) as rn
    FROM Questions q
    WHERE q.DifficultyLevelId IN (SELECT DifficultyLevelId FROM ExamTypesDetails WHERE ExamTypeId = @examType)
) Q
JOIN ExamTypesDetails etd ON etd.DifficultyLevelId = Q.DifficultyLevelId AND etd.ExamTypeId = @examType
WHERE Q.rn <= etd.NumberOfQuestions;﻿
