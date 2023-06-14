CREATE TABLE [dbo].[UserSubscription] (
    [Id]                 INT            IDENTITY (1, 1) NOT NULL,
    [UserId]             NVARCHAR (128) NOT NULL,
    [DateSubscribed]     DATETIME       CONSTRAINT [DF__UserSubsc__DateS__4EA8A765] DEFAULT (getdate()) NOT NULL,
    [NumClasses]         INT            NOT NULL,
    [DateExpire]         DATETIME       NULL,
    [CurrentBalance]     INT            NOT NULL,
    [ClassesDone]        INT            CONSTRAINT [DF__UserSubsc__Class__4F9CCB9E] DEFAULT ((0)) NOT NULL,
    [AmountPaid]         FLOAT (53)     NULL,
    [UserCreated]        NVARCHAR (128) NULL,
    [Active]             BIT            NOT NULL,
    [SubscriptionTypeId] INT            NOT NULL,
    [Frozen]             BIT            CONSTRAINT [DF_UserSubscription_Frozen] DEFAULT ((0)) NOT NULL,
    [DateExpireOriginal] DATETIME       NULL,
    [IsFirst]            BIT            CONSTRAINT [DF_UserSubscription_IsFirst] DEFAULT ((0)) NOT NULL,
    [LateCacelation]     INT            CONSTRAINT [DF__UserSubsc__LateC__1F4E99FE] DEFAULT ((0)) NOT NULL,
    [IsLate]             BIT            CONSTRAINT [DF_UserSubscription_IsLate] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_UserSubscription] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_UserSubscription_AspNetUsers] FOREIGN KEY ([UserId]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_UserSubscription_AspNetUsers1] FOREIGN KEY ([UserCreated]) REFERENCES [dbo].[AspNetUsers] ([Id]),
    CONSTRAINT [FK_UserSubscription_SubscriptionType] FOREIGN KEY ([SubscriptionTypeId]) REFERENCES [dbo].[SubscriptionType] ([Id])
);
















GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE TRIGGER Trig_UpdateUserActive 
   ON  dbo.UserSubscription
   AFTER INSERT,UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    
	UPDATE trg
	SET trg.Active = Inserted.Active
	FROM dbo.AspNetUsers trg
	INNER JOIN Inserted ON Inserted.UserId = trg.Id

END
