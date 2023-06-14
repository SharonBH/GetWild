-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[GetUserGraph] 
	-- Add the parameters for the stored procedure here
	@FromDate DATE = NULL,
    @ToDate DATE = NULL,
	@StudioId int
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
	SET @FromDate = DATEADD(year, -1, GETUTCDATE())
	--SET @FromDate = (SELECT CAST(MIN(DateSubscribed) AS DATE) FROM dbo.UserSubscription)

	IF @ToDate IS NULL
	SET @ToDate = getutcdate()

while @FromDate <= @ToDate
begin
	insert into #Temp (date) values (@FromDate)
	UPDATE #Temp
	SET ActiveUsers = (SELECT COUNT(1) from UserSubscription us
	inner join AspNetUsers u on us.UserId = u.Id
join [dbo].[AspNetUserRoles] ur on ur.userid = u.id and ur.roleid = 2
--INNER JOIN dbo.Studio s ON s.Id = u.StudioId and s.CompanyId = @CompanyId
	WHERE u.StudioId = @StudioId and DateSubscribed <= @FromDate AND (DateExpire >= @FromDate or Frozen = 1))
	--and  us.active = 1)
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
