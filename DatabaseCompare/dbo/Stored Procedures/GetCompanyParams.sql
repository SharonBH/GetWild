-- =============================================
-- Author:		Sharon Ben Haim
-- Create date: 21/05/2013
-- Description:	get param and flatten the data
-- =============================================
CREATE PROCEDURE [dbo].[GetCompanyParams]
	-- Add the parameters for the stored procedure here
	@CompanyId nvarchar(10) = '0'

AS
BEGIN

	DECLARE @cols AS NVARCHAR(MAX),
    @query  AS NVARCHAR(MAX)


	select @cols = STUFF((SELECT distinct ',' + QUOTENAME(ParamName) 
                    from [CompanyParam] 
					FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'')

			set @query = 'SELECT ' + @cols + ' 
             from 
             (
                select ParamName, ParamValue
                from [CompanyParam] where [CompanyID] =  cast(''' + @CompanyId + ''' as int)
            ) x
            pivot 
            (
                min(ParamValue)
                for ParamName in (' + @cols + ') 
            ) p '
--print (@query)
execute(@query)
return

END
