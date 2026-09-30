BEGIN TRANSACTION;
GO

CREATE TABLE [ExaminerAttempts] (
    [Id] int NOT NULL IDENTITY,
    [ExamId] int NOT NULL,
    [UserId] int NOT NULL,
    [ExamTitle] nvarchar(max) NULL,
    [SnapshotJson] nvarchar(max) NULL,
    [Status] nvarchar(20) NULL,
    [StartedAt] datetime2 NOT NULL,
    [SubmittedAt] datetime2 NULL,
    [EvaluatedAt] datetime2 NULL,
    [TotalQuestions] int NOT NULL,
    [CorrectQuestions] int NULL,
    [Score] decimal(18,10) NULL,
    [Version] rowversion NULL,
    CONSTRAINT [PK_ExaminerAttempts] PRIMARY KEY ([Id])
);
GO

CREATE INDEX [IX_ExaminerAttempts_Status] ON [ExaminerAttempts] ([Status]);
GO

CREATE INDEX [IX_ExaminerAttempts_UserId_StartedAt] ON [ExaminerAttempts] ([UserId], [StartedAt]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260930105742_AddExaminerAttempts', N'5.0.5');
GO

COMMIT;
GO

