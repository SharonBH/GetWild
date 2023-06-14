CREATE TABLE [dbo].[ClassEnrollment] (
    [Id]                        INT        IDENTITY (1, 1) NOT NULL,
    [ClassId]                   INT        NOT NULL,
    [SubscriptionId]            INT        NOT NULL,
    [DateEnrolled]              DATETIME   CONSTRAINT [DF__tmp_rg_xx__DateE__24285DB4] DEFAULT (getdate()) NOT NULL,
    [IsDeleted]                 BIT        CONSTRAINT [DF__tmp_rg_xx__IsDel__251C81ED] DEFAULT ((0)) NOT NULL,
    [DateCanceled]              DATETIME   NULL,
    [IsVerified]                BIT        CONSTRAINT [DF__tmp_rg_xx__IsVer__2610A626] DEFAULT ((1)) NOT NULL,
    [IsSmsSent]                 BIT        CONSTRAINT [DF__tmp_rg_xx__IsSms__2704CA5F] DEFAULT ((0)) NOT NULL,
    [Rating]                    FLOAT (53) NULL,
    [ClassAvailablePlacementId] INT        NULL,
    [IsLateCancel]              BIT        CONSTRAINT [DF_ClassEnrollment_IsLateCancel] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_UserClasses] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ClassEnrollment_ClassAvailablePlacement] FOREIGN KEY ([ClassAvailablePlacementId]) REFERENCES [dbo].[ClassAvailablePlacement] ([Id]),
    CONSTRAINT [FK_ClassEnrollment_UserSubscription] FOREIGN KEY ([SubscriptionId]) REFERENCES [dbo].[UserSubscription] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserClasses_Class] FOREIGN KEY ([ClassId]) REFERENCES [dbo].[Class] ([Id])
);






















GO
CREATE TRIGGER [dbo].[ClassEnrollmentUpdate]
       ON [dbo].[ClassEnrollment]
AFTER INSERT, UPDATE
AS
BEGIN
       SET NOCOUNT ON;
 
       DECLARE @ClassId INT
	   declare @ClassAvailablePlacementId int
 
 if UPDATE(IsDeleted)
	begin
       SELECT @ClassId = INSERTED.ClassId, @ClassAvailablePlacementId = INSERTED.ClassAvailablePlacementId
	          
       FROM INSERTED
 
 if (@ClassAvailablePlacementId > 0)
 begin
       update trg
	   set trg.Participants = src.Participants,
	   trg.ExtraParticipants = src.ExtraParticipants
	   from Class trg
       join (select ce.ClassId, sum(case when isNull(p.[StudioPlacementId],999) <> 999 and isNull(p.[StudioPlacementId],1003) <> 1003 and isNull(p.[StudioPlacementId],1007) <> 1007 and ce.IsDeleted = 0 then 1 else 0 end) Participants,
	   sum(case when (p.[StudioPlacementId] = 999 or p.[StudioPlacementId] = 1003 or p.[StudioPlacementId] = 1007) and ce.IsDeleted = 0 then 1 else 0 end) ExtraParticipants
	   from ClassEnrollment ce 
	   left join ClassAvailablePlacement p on ce.ClassAvailablePlacementId = p.Id
	   --where ce.IsDeleted = 0
	   group by ce.ClassId
	   ) src on src.ClassId = trg.Id
	   where Id = @ClassId
	  end
	  else
	  begin
		update Class
       set Participants = (select count(1) from ClassEnrollment where ClassId = @ClassId and IsDeleted = 0)
	   where Id = @ClassId
	  end
	END
end
GO
CREATE NONCLUSTERED INDEX [IXGetUsersList]
    ON [dbo].[ClassEnrollment]([ClassId] ASC, [IsDeleted] ASC)
    INCLUDE([SubscriptionId]);


GO
CREATE NONCLUSTERED INDEX [IX_UserEnroll]
    ON [dbo].[ClassEnrollment]([IsDeleted] ASC)
    INCLUDE([ClassId], [SubscriptionId], [IsVerified]);


GO
CREATE NONCLUSTERED INDEX [IX_EnrollmentIsDeleted]
    ON [dbo].[ClassEnrollment]([SubscriptionId] ASC, [IsDeleted] ASC);


GO
CREATE NONCLUSTERED INDEX [nci_wi_ClassEnrollment_BE8BF4AE1881AAE8FC0823783C04507C]
    ON [dbo].[ClassEnrollment]([ClassAvailablePlacementId] ASC);

