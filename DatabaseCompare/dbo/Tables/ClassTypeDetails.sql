CREATE TABLE [dbo].[ClassTypeDetails] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (128) NOT NULL,
    [Description] NVARCHAR (MAX) NOT NULL,
    [ClassTypeId] INT            NOT NULL,
    [IsDeleted]   BIT            CONSTRAINT [DF_ClassTypeDetails_IsDeleted] DEFAULT ((0)) NOT NULL,
    [Picture]     NVARCHAR (250) NULL,
    CONSTRAINT [PK_ClassTypeDetails] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClassTypeDetails_ClassType] FOREIGN KEY ([ClassTypeId]) REFERENCES [dbo].[ClassType] ([Id])
);





