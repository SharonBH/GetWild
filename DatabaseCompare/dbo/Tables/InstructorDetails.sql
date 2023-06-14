CREATE TABLE [dbo].[InstructorDetails] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [InstructorId] NVARCHAR (128) NOT NULL,
    [ColorCode]    NCHAR (10)     DEFAULT ('696969') NOT NULL,
    [Rate]         FLOAT (53)     NOT NULL,
    [DailyRate]    FLOAT (53)     DEFAULT ((0)) NOT NULL,
    [DateUpdated]  DATETIME       NOT NULL,
    [IsDeleted]    BIT            CONSTRAINT [DF_InstructorDetails_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_InstructorRates] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_InstructorRates_AspNetUsers] FOREIGN KEY ([InstructorId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);





