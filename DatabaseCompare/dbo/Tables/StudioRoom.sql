CREATE TABLE [dbo].[StudioRoom] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (250) NOT NULL,
    [MaxParticipants] INT            NOT NULL,
    [Picture]         NVARCHAR (250) NULL,
    [StudioId]        INT            NOT NULL,
    [IsDeleted]       BIT            CONSTRAINT [DF__StudioRoo__IsDel__0F975522] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_StudioRoom] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_StudioRoom_Studio] FOREIGN KEY ([StudioId]) REFERENCES [dbo].[Studio] ([Id])
);

