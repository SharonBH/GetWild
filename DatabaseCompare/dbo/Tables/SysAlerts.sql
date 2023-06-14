CREATE TABLE [dbo].[SysAlerts] (
    [Id]      INT            IDENTITY (1, 1) NOT NULL,
    [UserId]  NVARCHAR (128) NOT NULL,
    [TypeId]  INT            NULL,
    [Message] NVARCHAR (MAX) NOT NULL,
    [Date]    DATETIME       NOT NULL,
    [IsRead]  BIT            DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_SysAlerts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SysAlerts_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SysAlerts_MSGTypes] FOREIGN KEY ([TypeId]) REFERENCES [dbo].[MSGTypes] ([Id])
);



