-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[CalcClassRating]
	-- Add the parameters for the stored procedure here
	@classId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;


		update Class
		set Rating = (select sum(Rating) / count(1) n from ClassEnrollment where ClassId = @classId and Rating is not null)
		where Id = @classId
		

END
