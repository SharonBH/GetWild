-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[TickUser]
	-- Add the parameters for the stored procedure here
	@userId NVARCHAR(128)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	IF NOT EXISTS(select 1 from dbo.UserDailyTick WHERE UserId = @userId and CAST(Date AS DATE) = CAST(GETDATE() AS date)) 
		BEGIN
			INSERT INTO UserDailyTick (UserId, Date) VALUES (@userId, CAST(GETDATE() AS date))
		END
END
