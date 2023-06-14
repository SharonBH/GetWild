-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[GetUserGraph_org] 
	-- Add the parameters for the stored procedure here
	@FromDate DATE = NULL,
    @ToDate DATE = NULL,
	@CompanyId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--IF @FromDate IS NULL
	--	SET @FromDate = DATEADD(MONTH, -1, GETDATE())
    
IF OBJECT_ID('tempdb.dbo.#Temp', 'U') IS NOT NULL
  DROP TABLE #Temp; 

  CREATE TABLE #Temp (Date DATE, ActiveUsers INT)

 IF @FromDate IS NULL
	SET @FromDate = (SELECT CAST(MIN(DateSubscribed) AS DATE) FROM dbo.UserSubscription)

	IF @ToDate IS NULL
	SET @ToDate = getutcdate()

while @FromDate <= @ToDate
begin
	insert into #Temp (date) values (@FromDate)
	UPDATE #Temp
	SET ActiveUsers = (SELECT COUNT(1) FROM dbo.UserSubscription us
	inner join AspNetUsers u on us.UserId = u.Id
	inner join Studio s on s.Id = u.StudioId and s.CompanyId = @CompanyId
	WHERE DateSubscribed <= @FromDate AND DateExpire >= @FromDate
	AND us.Id NOT IN (SELECT ub.SubscriptionId FROM dbo.UserBalanceTracking ub 
	INNER JOIN (SELECT SubscriptionId, MAX(date) date FROM dbo.UserBalanceTracking GROUP BY SubscriptionId) t ON t.SubscriptionId = ub.SubscriptionId AND t.date = ub.Date
	WHERE Balance = 0 AND ub.Date <= @FromDate))
	WHERE Date = @FromDate
set @FromDate = DATEADD(day,1,@FromDate)
end


SELECT * FROM #Temp

--INSERT INTO #Temp (participants,
--date,
--Active) 
--SELECT 0, date, 0
--FROM #Temp
--WHERE Active = 1 AND date NOT IN (SELECT date FROM #Temp WHERE Active = 0)

--INSERT INTO #Temp (participants,
--date,
--Active) 
--SELECT 0, date, 1
--FROM #Temp
--WHERE Active = 0 AND date NOT IN (SELECT date FROM #Temp WHERE Active = 1)

--SELECT TOP 90 Date, Active, SUM(participants) OVER (PARTITION BY Active ORDER BY date) AS UsersByActivity,
--SUM(participants) OVER (ORDER BY date) TotalUsers
--FROM #Temp
----WHERE date >= @FromDate --AND date <= @ToDate
--ORDER BY date DESC
END
