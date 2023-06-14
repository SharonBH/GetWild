CREATE TABLE [dbo].[ClassDailySlot] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [StartTime]   TIME (5)       NOT NULL,
    [EndTime]     TIME (5)       NOT NULL,
    [Duration]    INT            NOT NULL,
    [Description] NVARCHAR (256) NULL,
    [IsDeleted]   BIT            DEFAULT ((0)) NOT NULL,
    [StudioId]    INT            NOT NULL,
    CONSTRAINT [PK_ClassDailySlot] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClassDailySlot_Studio] FOREIGN KEY ([StudioId]) REFERENCES [dbo].[Studio] ([Id])
);



