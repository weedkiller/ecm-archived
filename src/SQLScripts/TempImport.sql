
 SELECT 2 AS UserTypeId, 0 AS CustomerTypeId, FirstName, LastName, Email, Remark, Password, Address, Street, PostalCode, Phone, Mobile,
  Address AS ShippingAddress, Street AS ShippingStreet, PostalCode AS ShippingPostalCode, Phone AS ShippingPhone,
  CASE Gender WHEN 'Homme' THEN 1 ELSE 2 END AS TitleId,
  (SELECT TOP(1) CityId FROM City WHERE CityName LIKE '%' + ACustomer.City + '%' AND City.PostalCode = ACustomer.PostalCode) AS CityId,
  (SELECT TOP(1) CountryId FROM Country WHERE ACustomer.Country LIKE '%' + CountryName + '%') AS CountryId,
  (SELECT TOP(1) CityId FROM City WHERE CityName LIKE '%' + ACustomer.City + '%' AND City.PostalCode = ACustomer.PostalCode) AS ShippingCityId,
  (SELECT TOP(1) CountryId FROM Country WHERE ACustomer.Country LIKE '%' + CountryName + '%') AS ShippingCountryId,
  1 AS Active, 1 AS IsReceiveEmailInfo, CASE Gender WHEN 'Homme' THEN 0 ELSE 1 END As Gender,
  CAST(GETDATE() AS DATE) AS InsertDate,  CAST(GETDATE() AS DATE) AS UpdateDate,  CAST(GETDATE() AS DATE) AS RegisteredDate, CAST(DATEADD(YEAR, 100, GETDATE()) AS DATE) AS ExpiredDate,  CAST(GETDATE() AS DATE) AS LastLoggedOn,
  0 As SiteId, 1 AS ModifyUserId
  FROM ACustomer

  SELECT * FROM Query ORDER BY FirstName, LastName