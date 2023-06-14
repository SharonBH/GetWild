CREATE TABLE [dbo].[StudioExpenses] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (512) NOT NULL,
    [Description] NVARCHAR (MAX) NULL,
    [Amount]      FLOAT (53)     NOT NULL,
    [Date]        DATETIME       NOT NULL,
    [StudioId]    INT            NOT NULL,
    [IsDeleted]   BIT            CONSTRAINT [DF_StudioExpenses_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_StudioExpenses] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_StudioExpenses_Studio] FOREIGN KEY ([StudioId]) REFERENCES [dbo].[Studio] ([Id])
);

