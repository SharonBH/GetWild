CREATE TABLE [dbo].[UserProcessing] (
    [Id]     INT             IDENTITY (1, 1) NOT NULL,
    [UserId] NVARCHAR (128)  NOT NULL,
    [Date]   DATETIME        NOT NULL,
    [Note]   NVARCHAR (1024) NULL,
    CONSTRAINT [PK_UserProcessing] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserProcessing_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);

