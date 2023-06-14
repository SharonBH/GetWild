-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetWeeklyFooter]
	-- Add the parameters for the stored procedure here
	@StartDate datetime,
	@StudioId int = 0,
	@daysSince int = 14
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @endDate datetime = dateadd(day, 7, @StartDate)

	--select @StartDate
    -- Insert statements for procedure here
	select cast(c.Date as date) [Date],
	sum(case when ce.IsDeleted = 0 then 1 else 0 end) TotalParticipants,
	sum(case when ce.IsDeleted = 0 and IsVerified = 0 then 1 else 0 end) MissedParticipants,
	sum(case when ce.IsDeleted = 1 and isLatecancel = 1 then 1 else 0 end) LateCancel,
	sum(case when ce.IsDeleted = 0 and ec.Id is not null then 1 else 0 end) Comments,
	sum(case when ce.IsDeleted = 0 and DATEDIFF(day, lc.Date, c.Date) >= @daysSince then 1 else 0 end) Activated,
	sum(case when ce.IsDeleted = 0 and r.RoleId = 5 then 1 else 0 end) TrailParticipants 
	from class c
	join ClassEnrollment ce on ce.ClassId = c.Id
	left join EnrollmentComments ec on ce.Id = ec.EnrollmentId
	join UserSubscription us on us.Id = ce.SubscriptionId
	join AspNetUsers u on u.Id = us.UserId
	left join LastClass lc on lc.UserId = u.Id and lc.RowDate = cast(c.Date as date)
	join AspNetUserRoles r on r.UserId = u.Id
	join StudioRoom sr on sr.Id = c.StudioRoomId and sr.StudioId = @StudioId
	where cast(c.Date as date) >= @StartDate and cast(c.Date as date) < @endDate
	--and ce.IsDeleted = 0 
	and c.IsDeleted = 0
	group by cast(c.Date as date)
	order by cast(c.Date as date)
END