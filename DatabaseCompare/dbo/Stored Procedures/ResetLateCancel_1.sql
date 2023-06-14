CREATE PROCEDURE [dbo].[ResetLateCancel]
	-- Add the parameters for the stored procedure here
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	if (DATEPART(day, GETUTCDATE()) = 1)
	begin
		update UserSubscription
		set LateCacelation = 0, 
		isLate = 0
		where LateCacelation > 0
	end 

END