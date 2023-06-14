CREATE TABLE [dbo].[SmsSender] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [Date]       DATETIME       NOT NULL,
    [SendDate]   DATETIME       NOT NULL,
    [Sender]     NVARCHAR (50)  NOT NULL,
    [Recipients] NVARCHAR (MAX) NOT NULL,
    [RefId]      NVARCHAR (50)  NULL,
    [Source]     NVARCHAR (50)  NULL,
    [Sent]       BIT            NOT NULL,
    [Message]    NVARCHAR (MAX) NOT NULL,
    [UserId]     NVARCHAR (128) NOT NULL,
    CONSTRAINT [PK_SmsSender] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SmsSender_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

