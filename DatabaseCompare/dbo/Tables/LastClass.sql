CREATE TABLE [dbo].[LastClass] (
    [RowDate] DATE           NOT NULL,
    [UserId]  NVARCHAR (128) NOT NULL,
    [Date]    DATETIME       NOT NULL,
    CONSTRAINT [PK_LastClass] PRIMARY KEY CLUSTERED ([RowDate] ASC, [UserId] ASC),
    CONSTRAINT [FK_LastClass_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);



