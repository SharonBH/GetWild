CREATE PROCEDURE [dbo].[GetCommentsForClass] 
	-- Add the parameters for the stored procedure here
	@ClassId int = 0 --,
	--@StudioId int = 0
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
;WITH cte AS
(
	select --ce.*, c.date, u2.FullName CommentBy, CreateDate, Comment,
	 ec.Id, ec.EnrollmentId, ec.UserId, ec.ClassId, Comment, u2.FullName CommentBy, CreateDate, ec.IsDeleted, cast(case when ur.RoleId = 1 then 1 else 0 end as bit) IsAdmin,
	ROW_NUMBER() OVER (PARTITION BY us.UserId ORDER BY c.date desc) AS rn
	from ClassEnrollment ce
	join UserSubscription us on us.id = ce.SubscriptionId
	join Class c on c.id = ce.ClassId and c.IsDeleted = 0
	left join [dbo].[EnrollmentComments] ec on ec.EnrollmentId = ce.Id
	left join AspNetUsers u2 on u2.id = ec.UserCreated --and u2.StudioId = @StudioId
	left join AspNetUserRoles ur on ur.UserId = u2.Id and RoleId = 1
	where us.Userid in (select us.UserId from ClassEnrollment ce join UserSubscription us on us.id = ce.SubscriptionId where ce.ClassId = @ClassId and ce.IsDeleted = 0)
	and ce.IsDeleted = 0 --and ec.Id is not null
	--and c.Date < GETUTCDATE()
)


SELECT *
FROM cte
WHERE rn < 3 --and Id is not null

end