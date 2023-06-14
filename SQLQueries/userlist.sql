SELECT  u.FirstName + ' ' + u.LastName FullName,
                            u.PhoneNumber,
                            u.Gender,
                            u.JoinDate,
                            u.ProfileIMG,
                            u.Id, 
							us.DateSubscribed, 
							us.DateExpire, 
							us.Active,
							us.NumClasses, 
							us.CurrentBalance , 
							LastClass.Last, 
							NextClass.Next
FROM dbo.AspNetUsers u
INNER JOIN dbo.AspNetUserRoles r ON r.UserId = u.Id AND r.RoleId = '2'
LEFT JOIN 
(SELECT * FROM 
dbo.UserSubscription 
WHERE Active = 1) us ON us.UserId = u.Id
LEFT JOIN (SELECT SubscriptionId, MAX(c.Date) Last FROM dbo.ClassEnrollment ce inner join Class c on ce.ClassId = c.Id WHERE c.Date < GETDATE() GROUP BY SubscriptionId) LastClass ON LastClass.SubscriptionId = us.Id
LEFT JOIN (SELECT SubscriptionId, MIN(c.Date) Next FROM dbo.ClassEnrollment ce inner join Class c on ce.ClassId = c.Id WHERE c.Date > GETDATE() GROUP BY SubscriptionId) NextClass ON NextClass.SubscriptionId = us.Id


--SELECT * FROM dbo.ClassEnrollment WHERE SubscriptionId = 22
