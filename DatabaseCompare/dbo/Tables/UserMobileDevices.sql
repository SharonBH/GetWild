CREATE TABLE [dbo].[UserMobileDevices] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [UserId]             NVARCHAR (128) NOT NULL,
    [DeviceId]           NVARCHAR (256) NOT NULL,
    [PushNotificationId] NVARCHAR (128) NULL,
    [DeviceAdsId]        NVARCHAR (128) NULL,
    [DeviceType]         NVARCHAR (128) NULL,
    [DeviceOS]           NVARCHAR (128) NULL,
    [DateAdded]          DATETIME       CONSTRAINT [DF_User_MobileDevices_DateAdded] DEFAULT (getdate()) NOT NULL,
    [IsDeleted]          BIT            CONSTRAINT [DF_UserMobileDevices_IsDeleted] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_User_MobileDevices] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_User_MobileDevices_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id])
);

