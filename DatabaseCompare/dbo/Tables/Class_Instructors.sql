CREATE TABLE [dbo].[Class_Instructors] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [ClassId]      INT            NOT NULL,
    [InstructorId] NVARCHAR (128) NOT NULL,
    [Verified]     BIT            CONSTRAINT [DF_Class_Instructors_Verified] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Class_Instructors] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Class_Instructors_AspNetUsers] FOREIGN KEY ([InstructorId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_Class_Instructors_Class] FOREIGN KEY ([ClassId]) REFERENCES [dbo].[Class] ([Id])
);

