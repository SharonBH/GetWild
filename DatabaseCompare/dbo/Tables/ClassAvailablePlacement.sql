CREATE TABLE [dbo].[ClassAvailablePlacement] (
    [Id]                   INT            IDENTITY (1, 1) NOT NULL,
    [ClassId]              INT            NOT NULL,
    [StudioPlacementId]    INT            NOT NULL,
    [StudioPlacementName]  NVARCHAR (250) NOT NULL,
    [ClassPlacementNumber] TINYINT        NOT NULL,
    [IsInUse]              BIT            CONSTRAINT [DF_ClassAvailablePlacements_IsInUse] DEFAULT ((0)) NOT NULL,
    [IsDeleted]            BIT            CONSTRAINT [DF_ClassAvailablePlacements_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ClassAvailablePlacements] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClassAvailablePlacement_StudioPlacements] FOREIGN KEY ([StudioPlacementId]) REFERENCES [dbo].[StudioPlacements] ([Id]),
    CONSTRAINT [FK_ClassAvailablePlacements_Class] FOREIGN KEY ([ClassId]) REFERENCES [dbo].[Class] ([Id])
);



