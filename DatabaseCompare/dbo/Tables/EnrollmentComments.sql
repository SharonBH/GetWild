CREATE TABLE [dbo].[EnrollmentComments] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [EnrollmentId] INT            NOT NULL,
    [UserId]       NVARCHAR (128) NOT NULL,
    [ClassId]      INT            NOT NULL,
    [Comment]      NVARCHAR (MAX) NULL,
    [UserCreated]  NVARCHAR (128) NOT NULL,
    [CreateDate]   DATETIME       NOT NULL,
    [IsDeleted]    BIT            CONSTRAINT [DF_EnrollmentComments_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_EnrollmentComments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EnrollmentComments_AspNetUsers] FOREIGN KEY ([UserCreated]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_EnrollmentComments_AspNetUsers1] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_EnrollmentComments_Class] FOREIGN KEY ([ClassId]) REFERENCES [dbo].[Class] ([Id]),
    CONSTRAINT [FK_EnrollmentComments_ClassEnrollment] FOREIGN KEY ([EnrollmentId]) REFERENCES [dbo].[ClassEnrollment] ([Id])
);

