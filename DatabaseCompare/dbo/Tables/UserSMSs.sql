CREATE TABLE [dbo].[UserSMSs] (
    [Id]       INT             IDENTITY (1, 1) NOT NULL,
    [UserId]   NVARCHAR (128)  NOT NULL,
    [Date]     DATETIME        NOT NULL,
    [TypeId]   INT             NOT NULL,
    [SMS]      NVARCHAR (1024) NOT NULL,
    [RefId]    INT             NULL,
    [Response] NVARCHAR (1024) NULL,
    CONSTRAINT [PK_UserSMSs] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserSMSs_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserSMSs_MSGTypes] FOREIGN KEY ([TypeId]) REFERENCES [dbo].[MSGTypes] ([Id])
);

