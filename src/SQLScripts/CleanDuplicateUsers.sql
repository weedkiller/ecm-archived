WITH Records AS (
SELECT UserId, Email, FirstName, LastName, SiteId,
ROW_NUMBER() OVER(PARTITION BY Email, FirstName, LastName, SiteId ORDER BY UserId) AS RN
FROM Users
WHERE ISNULL(FirstName, '') <> '' AND ISNULL(LastName, '') <> ''
)
DELETE it FROM CustomerGroupCustomer it
JOIN Records r ON r.UserId = it.CustomerId
WHERE r.RN > 1

WITH Records AS (
SELECT UserId, Email, FirstName, LastName, SiteId,
ROW_NUMBER() OVER(PARTITION BY Email, FirstName, LastName, SiteId ORDER BY UserId) AS RN
FROM Users
WHERE ISNULL(FirstName, '') <> '' AND ISNULL(LastName, '') <> ''
)
DELETE it FROM EmailTracking it
JOIN EmailQue eq ON eq.EmailQueId =  it.EmailQueId
JOIN Records r ON r.UserId = eq.CustomerId
WHERE r.RN > 1

WITH Records AS (
SELECT UserId, Email, FirstName, LastName, SiteId,
ROW_NUMBER() OVER(PARTITION BY Email, FirstName, LastName, SiteId ORDER BY UserId) AS RN
FROM Users
WHERE ISNULL(FirstName, '') <> '' AND ISNULL(LastName, '') <> ''
)
DELETE it FROM EmailQue it
JOIN Records r ON r.UserId = it.CustomerId
WHERE r.RN > 1


WITH Records AS (
SELECT UserId, Email, FirstName, LastName, SiteId,
ROW_NUMBER() OVER(PARTITION BY Email, FirstName, LastName, SiteId ORDER BY UserId) AS RN
FROM Users
WHERE ISNULL(FirstName, '') <> '' AND ISNULL(LastName, '') <> ''
)
DELETE it FROM Users it
JOIN Records r ON r.UserId = it.UserId
WHERE r.RN > 1

SELECT COUNT(*) FROM Users