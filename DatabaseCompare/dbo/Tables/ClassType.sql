CREATE TABLE [dbo].[ClassType] (
    [Id]          INT             IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (250)  NOT NULL,
    [Description] NVARCHAR (1024) NULL,
    [Picture]     NVARCHAR (250)  NULL,
    [BGColor]     NVARCHAR (50)   CONSTRAINT [DF_ClassType_BGColor] DEFAULT (N'#696969') NOT NULL,
    [IsDeleted]   BIT             CONSTRAINT [DF__ClassType__IsDel__1BFD2C07] DEFAULT ((0)) NOT NULL,
    [StudioId]    INT             NOT NULL,
    CONSTRAINT [PK_ClassType] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClassType_Studio] FOREIGN KEY ([StudioId]) REFERENCES [dbo].[Studio] ([Id])
);

