-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetNextWaitingListEnrollments]
	-- Add the parameters for the stored procedure here
	@Broadcast bit = 0,
	@MinsInterval int = 10,
	@BroadcastThresholdMins int = 30
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

if (@Broadcast = 1)
	begin
		select wl.[Id]
      ,[ClassId]
      ,[SubscriptionId]
      ,[DateEnrolled]
      ,wl.[IsDeleted]
      ,[DateCanceled]
      ,[IsSmsSent]
      ,[DateSmsSent]
		from [dbo].[ClassWaitList] wl
		join class c on c.Id = wl.classid
		--left join ClassEnrollment ce on c.Id = ce.ClassId and ce.SubscriptionId = wl.SubscriptionId
		 where 
		 c.Date <= DATEADD(MINUTE,@BroadcastThresholdMins, getdate())
		 and c.IsDeleted = 0
		 and c.IsFull = 1 and c.[WaitingList] > 0
		 and wl.IsDeleted = 0 
	end

else
	begin
		select [Id]
      ,[ClassId]
      ,[SubscriptionId]
      ,[DateEnrolled]
      ,[IsDeleted]
      ,[DateCanceled]
      ,[IsSmsSent]
      ,[DateSmsSent] from (select 
		ROW_NUMBER() OVER (PARTITION BY wl.Classid ORDER BY DateEnrolled) rn
		,wl.*
		from [dbo].[ClassWaitList] wl
		join class c on c.Id = wl.classid
		 where 
		 c.Date > getdate() and c.IsDeleted = 0
		 and c.IsFull = 1 and c.[WaitingList] > 0
		 and wl.IsDeleted = 0 and (wl.IsSmsSent = 0 or DATEDIFF(MINUTE, DateSmsSent, getdate()) < @MinsInterval)
		 ) A
		 where rn = 1
	end

END
