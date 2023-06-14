CREATE TABLE [dbo].[UserDailyTick] (
    [UserId] NVARCHAR (128) NOT NULL,
    [Date]   DATETIME       NOT NULL,
    [Note]   NVARCHAR (512) NULL,
    CONSTRAINT [PK_UserDailyTick] PRIMARY KEY CLUSTERED ([UserId] ASC, [Date] ASC),
    CONSTRAINT [FK_UserDailyTick_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]) ON DELETE CASCADE
);

