-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE RemoveMarked


AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- remove the users from user list
	EXEC sp_rename 'dbo.GetUsersList', 'GetUsersList_ORG';
	EXEC sp_rename 'dbo.GetUsersListNoMark', 'GetUsersList';

	--remove from reports
	EXEC sp_rename 'dbo.GetUsersNoEnrollmentList', 'GetUsersNoEnrollmentList_ORG';
	EXEC sp_rename 'dbo.GetUsersNoEnrollmentListNoMark', 'GetUsersNoEnrollmentList';
	

    -- remove users from class enrollments
	update trg
	set IsDeleted = 1,
	DateCanceled = '20991231'
	from ClassEnrollment trg
	join UserSubscription us on us.id = trg.SubscriptionId
	join AspNetUsers u on u.id = us.UserId and Marked = 1

END