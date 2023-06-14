CREATE TABLE [dbo].[StudioPlacements] (
    [Id]        INT            IDENTITY (1, 1) NOT NULL,
    [Name]      NVARCHAR (250) NOT NULL,
    [StudioId]  INT            NOT NULL,
    [Places]    INT            CONSTRAINT [DF_StudioPlacements_Places] DEFAULT ((0)) NOT NULL,
    [IsDeleted] BIT            NOT NULL,
    [TypeId]    INT            CONSTRAINT [DF_StudioPlacements_TypeId] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ClassPlacements] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_StudioPlacements_Studio] FOREIGN KEY ([StudioId]) REFERENCES [dbo].[Studio] ([Id])
);







