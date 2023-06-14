-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UnFreezeSubscriptions]
	-- Add the parameters for the stored procedure here

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	IF OBJECT_ID('tempdb.dbo.#frozen', 'U') IS NOT NULL DROP TABLE #frozen;
	-- unfreeze subscription on Date to finish
	SELECT SubscriptionId, f.Date, f.FreezeToDate, f.Id INTO #frozen
	FROM dbo.FrozenSubscription f
	INNER JOIN dbo.UserSubscription us ON us.Id = f.SubscriptionId
	WHERE IsDeleted = 0 AND FreezeToDate <= GETDATE() AND us.Frozen = 1

	WHILE exists (SELECT TOP 1 1 FROM #frozen)
		BEGIN
			BEGIN TRAN
			DECLARE @days INT = 0
			DECLARE @subId INT = 0
			DECLARE @Id INT = 0
			SELECT TOP 1 @subId = SubscriptionId,  @days = DATEDIFF(DAY, Date,FreezeToDate), @Id = Id
			FROM #frozen

			UPDATE dbo.FrozenSubscription
			SET IsDeleted = 1, DateFinished = GETDATE()
			WHERE Id = @Id

			UPDATE dbo.UserSubscription
			SET DateExpire = DATEADD(DAY,@days,DateExpire), Frozen = 0
			WHERE Id = @subId

			DELETE FROM #frozen WHERE Id = @Id
			COMMIT
		END

	DROP TABLE #frozen
END
