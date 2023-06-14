CREATE TABLE [dbo].[ClassWaitList] (
    [Id]                   INT      IDENTITY (1, 1) NOT NULL,
    [ClassId]              INT      NOT NULL,
    [SubscriptionId]       INT      NOT NULL,
    [DateEnrolled]         DATETIME CONSTRAINT [DF_ClassWaitList_DateEnrolled] DEFAULT (getdate()) NOT NULL,
    [IsDeleted]            BIT      CONSTRAINT [DF_ClassWaitList_IsDeleted] DEFAULT ((0)) NOT NULL,
    [DateCanceled]         DATETIME NULL,
    [IsSmsSent]            BIT      CONSTRAINT [DF_ClassWaitList_IsSmsSent] DEFAULT ((0)) NOT NULL,
    [DateSmsSent]          DATETIME NULL,
    [IsBroadcastSmsSent]   BIT      CONSTRAINT [DF_ClassWaitList_IsSmsSent1] DEFAULT ((0)) NOT NULL,
    [DateBroadcastSmsSent] DATETIME NULL,
    CONSTRAINT [PK_ClassWaitList] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClassWaitList_Class] FOREIGN KEY ([ClassId]) REFERENCES [dbo].[Class] ([Id]),
    CONSTRAINT [FK_ClassWaitList_UserSubscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[UserSubscription] ([Id]) ON DELETE CASCADE
);


GO
CREATE TRIGGER [dbo].[ClassWaitListUpdate]
       ON [dbo].[ClassWaitList]
AFTER INSERT, UPDATE
AS
BEGIN
       SET NOCOUNT ON;
 
       DECLARE @ClassId INT
  if UPDATE(IsDeleted)
	begin
       SELECT @ClassId = INSERTED.ClassId       
       FROM INSERTED
 
       update Class
       set WaitingList = (select count(1) from ClassWaitList where ClassId = @ClassId and IsDeleted = 0)
	   where Id = @ClassId
	END
END