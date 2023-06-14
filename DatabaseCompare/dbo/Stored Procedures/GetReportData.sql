-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetReportData]
	-- Add the parameters for the stored procedure here
	@classId INT = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT u.FirstName + ' ' + u.LastName Name, r.participate
FROM dbo.AspNetUsers u
LEFT JOIN
(SELECT us.userid, 1 participate --CASE when ce.Id IS NOT NULL THEN 1 ELSE 0 END participate
	FROM dbo.UserSubscription us 
inner JOIN dbo.ClassEnrollment ce ON ce.SubscriptionId = us.Id
WHERE ce.ClassId = @classId AND ce.IsDeleted = 0) r ON r.UserId = u.Id
ORDER BY u.Id
END
