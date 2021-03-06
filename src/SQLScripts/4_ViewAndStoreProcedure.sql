IF  EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_DiagramPaneCount' , N'SCHEMA',N'dbo', N'VIEW',N'uv_SaleOrderReport', NULL,NULL))
EXEC sys.sp_dropextendedproperty @name=N'MS_DiagramPaneCount' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'uv_SaleOrderReport'

GO
IF  EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_DiagramPane1' , N'SCHEMA',N'dbo', N'VIEW',N'uv_SaleOrderReport', NULL,NULL))
EXEC sys.sp_dropextendedproperty @name=N'MS_DiagramPane1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'uv_SaleOrderReport'

GO
IF  EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_DiagramPane2' , N'SCHEMA',N'dbo', N'VIEW',N'uv_SaleItemsReport', NULL,NULL))
EXEC sys.sp_dropextendedproperty @name=N'MS_DiagramPane2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'uv_SaleItemsReport'

GO
IF  EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_DiagramPane1' , N'SCHEMA',N'dbo', N'VIEW',N'uv_SaleItemsReport', NULL,NULL))
EXEC sys.sp_dropextendedproperty @name=N'MS_DiagramPane1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'uv_SaleItemsReport'

GO
/****** Object:  StoredProcedure [dbo].[UpdateUserType]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateUserType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateUserType]
GO
/****** Object:  StoredProcedure [dbo].[UpdateReadMessage]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateReadMessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateReadMessage]
GO
/****** Object:  StoredProcedure [dbo].[UpdatePromotion]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdatePromotion]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdatePromotion]
GO
/****** Object:  StoredProcedure [dbo].[UpdateFlagMessage]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateFlagMessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateFlagMessage]
GO
/****** Object:  StoredProcedure [dbo].[UpdateConnectWithFacebook]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateConnectWithFacebook]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[UpdateConnectWithFacebook]
GO
/****** Object:  StoredProcedure [dbo].[SelectWebContentsList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectWebContentsList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectWebContentsList]
GO
/****** Object:  StoredProcedure [dbo].[SelectUserTypesList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectUserTypesList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectUserTypesList]
GO
/****** Object:  StoredProcedure [dbo].[SelectUserType]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectUserType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectUserType]
GO
/****** Object:  StoredProcedure [dbo].[SelectUsersList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectUsersList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectUsersList]
GO
/****** Object:  StoredProcedure [dbo].[SelectUser]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectUser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectUser]
GO
/****** Object:  StoredProcedure [dbo].[SelectTaxesList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectTaxesList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectTaxesList]
GO
/****** Object:  StoredProcedure [dbo].[SelectTax]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectTax]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectTax]
GO
/****** Object:  StoredProcedure [dbo].[SelectSuppliersList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectSuppliersList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectSuppliersList]
GO
/****** Object:  StoredProcedure [dbo].[SelectSupplier]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectSupplier]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectSupplier]
GO
/****** Object:  StoredProcedure [dbo].[SelectStayPackagesList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectStayPackagesList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectStayPackagesList]
GO
/****** Object:  StoredProcedure [dbo].[SelectStayPackage]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectStayPackage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectStayPackage]
GO
/****** Object:  StoredProcedure [dbo].[SelectSitesList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectSitesList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectSitesList]
GO
/****** Object:  StoredProcedure [dbo].[SelectSiteReviewsList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectSiteReviewsList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectSiteReviewsList]
GO
/****** Object:  StoredProcedure [dbo].[SelectSiteById]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectSiteById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectSiteById]
GO
/****** Object:  StoredProcedure [dbo].[SelectSite]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectSite]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectSite]
GO
/****** Object:  StoredProcedure [dbo].[SelectSaleItemsReport]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectSaleItemsReport]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectSaleItemsReport]
GO
/****** Object:  StoredProcedure [dbo].[SelectResellersList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectResellersList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectResellersList]
GO
/****** Object:  StoredProcedure [dbo].[SelectRandomItems]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectRandomItems]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectRandomItems]
GO
/****** Object:  StoredProcedure [dbo].[SelectModifiersList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectModifiersList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectModifiersList]
GO
/****** Object:  StoredProcedure [dbo].[SelectModifier]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectModifier]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectModifier]
GO
/****** Object:  StoredProcedure [dbo].[SelectMailingListsList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectMailingListsList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectMailingListsList]
GO
/****** Object:  StoredProcedure [dbo].[SelectLatestItems]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectLatestItems]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectLatestItems]
GO
/****** Object:  StoredProcedure [dbo].[SelectLatestDLGItems]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectLatestDLGItems]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectLatestDLGItems]
GO
/****** Object:  StoredProcedure [dbo].[SelectItemTypesList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectItemTypesList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectItemTypesList]
GO
/****** Object:  StoredProcedure [dbo].[SelectItemType]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectItemType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectItemType]
GO
/****** Object:  StoredProcedure [dbo].[SelectItemsList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectItemsList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectItemsList]
GO
/****** Object:  StoredProcedure [dbo].[SelectItemReviewsList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectItemReviewsList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectItemReviewsList]
GO
/****** Object:  StoredProcedure [dbo].[SelectItemCategory]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectItemCategory]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectItemCategory]
GO
/****** Object:  StoredProcedure [dbo].[SelectItemCategoriesList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectItemCategoriesList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectItemCategoriesList]
GO
/****** Object:  StoredProcedure [dbo].[SelectItemById]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectItemById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectItemById]
GO
/****** Object:  StoredProcedure [dbo].[SelectItem]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectItem]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectItem]
GO
/****** Object:  StoredProcedure [dbo].[SelectInterestsList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectInterestsList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectInterestsList]
GO
/****** Object:  StoredProcedure [dbo].[SelectInterest]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectInterest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectInterest]
GO
/****** Object:  StoredProcedure [dbo].[SelectImpressumsList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectImpressumsList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectImpressumsList]
GO
/****** Object:  StoredProcedure [dbo].[SelectImpressum]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectImpressum]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectImpressum]
GO
/****** Object:  StoredProcedure [dbo].[SelectGreenFeesList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectGreenFeesList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectGreenFeesList]
GO
/****** Object:  StoredProcedure [dbo].[SelectGreenFeeCategoriesList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectGreenFeeCategoriesList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectGreenFeeCategoriesList]
GO
/****** Object:  StoredProcedure [dbo].[SelectGolfLessonsList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectGolfLessonsList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectGolfLessonsList]
GO
/****** Object:  StoredProcedure [dbo].[SelectGolfLessonCategoriesList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectGolfLessonCategoriesList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectGolfLessonCategoriesList]
GO
/****** Object:  StoredProcedure [dbo].[SelectGolfBrandsList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectGolfBrandsList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectGolfBrandsList]
GO
/****** Object:  StoredProcedure [dbo].[SelectEmailTemplateList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectEmailTemplateList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectEmailTemplateList]
GO
/****** Object:  StoredProcedure [dbo].[SelectEmailingsList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectEmailingsList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectEmailingsList]
GO
/****** Object:  StoredProcedure [dbo].[SelectDrivingRangesList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectDrivingRangesList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectDrivingRangesList]
GO
/****** Object:  StoredProcedure [dbo].[SelectDrivingRange]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectDrivingRange]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectDrivingRange]
GO
/****** Object:  StoredProcedure [dbo].[SelectCustomersList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCustomersList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectCustomersList]
GO
/****** Object:  StoredProcedure [dbo].[SelectCustomerGroupsList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCustomerGroupsList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectCustomerGroupsList]
GO
/****** Object:  StoredProcedure [dbo].[SelectCustomerGroup]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCustomerGroup]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectCustomerGroup]
GO
/****** Object:  StoredProcedure [dbo].[SelectCourseTypesList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCourseTypesList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectCourseTypesList]
GO
/****** Object:  StoredProcedure [dbo].[SelectCourseType]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCourseType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectCourseType]
GO
/****** Object:  StoredProcedure [dbo].[SelectCoursesList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCoursesList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectCoursesList]
GO
/****** Object:  StoredProcedure [dbo].[SelectCourse]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCourse]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectCourse]
GO
/****** Object:  StoredProcedure [dbo].[SelectCouponsList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCouponsList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectCouponsList]
GO
/****** Object:  StoredProcedure [dbo].[SelectCouponGroupsList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCouponGroupsList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectCouponGroupsList]
GO
/****** Object:  StoredProcedure [dbo].[SelectCoupon]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCoupon]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectCoupon]
GO
/****** Object:  StoredProcedure [dbo].[SelectCountriesList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCountriesList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectCountriesList]
GO
/****** Object:  StoredProcedure [dbo].[SelectCategory]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCategory]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectCategory]
GO
/****** Object:  StoredProcedure [dbo].[SelectCategoriesList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCategoriesList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectCategoriesList]
GO
/****** Object:  StoredProcedure [dbo].[SelectBrandsList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectBrandsList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SelectBrandsList]
GO
/****** Object:  StoredProcedure [dbo].[SearchGreenFee]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SearchGreenFee]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SearchGreenFee]
GO
/****** Object:  StoredProcedure [dbo].[SaveStyleImageDlgCardAdmin]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaveStyleImageDlgCardAdmin]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SaveStyleImageDlgCardAdmin]
GO
/****** Object:  StoredProcedure [dbo].[SaveSiteImage]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaveSiteImage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SaveSiteImage]
GO
/****** Object:  StoredProcedure [dbo].[SavePrimaryImageDlgCardAdmin]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SavePrimaryImageDlgCardAdmin]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SavePrimaryImageDlgCardAdmin]
GO
/****** Object:  StoredProcedure [dbo].[SavePriceDlgCardItemAdmin]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SavePriceDlgCardItemAdmin]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SavePriceDlgCardItemAdmin]
GO
/****** Object:  StoredProcedure [dbo].[SaveItemImage]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaveItemImage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SaveItemImage]
GO
/****** Object:  StoredProcedure [dbo].[SaveDlgCardItemAdmin]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaveDlgCardItemAdmin]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SaveDlgCardItemAdmin]
GO
/****** Object:  StoredProcedure [dbo].[SaveDLGCard]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaveDLGCard]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[SaveDLGCard]
GO
/****** Object:  StoredProcedure [dbo].[RandomDigit]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RandomDigit]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[RandomDigit]
GO
/****** Object:  StoredProcedure [dbo].[GetUserMessage]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserMessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetUserMessage]
GO
/****** Object:  StoredProcedure [dbo].[GetTotalUserMessageById]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetTotalUserMessageById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetTotalUserMessageById]
GO
/****** Object:  StoredProcedure [dbo].[GetSalesReport]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesReport]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetSalesReport]
GO
/****** Object:  StoredProcedure [dbo].[GetPromotion]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPromotion]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPromotion]
GO
/****** Object:  StoredProcedure [dbo].[GetItemPriceDlgCard]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetItemPriceDlgCard]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetItemPriceDlgCard]
GO
/****** Object:  StoredProcedure [dbo].[GetItemDlgCardByItemTypeId]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetItemDlgCardByItemTypeId]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetItemDlgCardByItemTypeId]
GO
/****** Object:  StoredProcedure [dbo].[GetInterest]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetInterest]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetInterest]
GO
/****** Object:  StoredProcedure [dbo].[GetDLGCardStyle]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDLGCardStyle]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDLGCardStyle]
GO
/****** Object:  StoredProcedure [dbo].[GetDlgCardHistory]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDlgCardHistory]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDlgCardHistory]
GO
/****** Object:  StoredProcedure [dbo].[GetCreditcardassociations]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCreditcardassociations]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetCreditcardassociations]
GO
/****** Object:  StoredProcedure [dbo].[GetAllUser]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllUser]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllUser]
GO
/****** Object:  StoredProcedure [dbo].[GetAllPromotion]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllPromotion]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAllPromotion]
GO
/****** Object:  StoredProcedure [dbo].[GetAccountOrderList]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAccountOrderList]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAccountOrderList]
GO
/****** Object:  StoredProcedure [dbo].[GetAccountOrderDetail]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAccountOrderDetail]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetAccountOrderDetail]
GO
/****** Object:  StoredProcedure [dbo].[DLGCardBalanceById]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DLGCardBalanceById]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DLGCardBalanceById]
GO
/****** Object:  StoredProcedure [dbo].[DeleteUserType]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteUserType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteUserType]
GO
/****** Object:  StoredProcedure [dbo].[DeleteUserMessageAdmin]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteUserMessageAdmin]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteUserMessageAdmin]
GO
/****** Object:  StoredProcedure [dbo].[DeletePromotionAdmin]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeletePromotionAdmin]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeletePromotionAdmin]
GO
/****** Object:  StoredProcedure [dbo].[DeletePriceDlgCardItemAdmin]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeletePriceDlgCardItemAdmin]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeletePriceDlgCardItemAdmin]
GO
/****** Object:  StoredProcedure [dbo].[DeleteImageDlgCardAdmin]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteImageDlgCardAdmin]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteImageDlgCardAdmin]
GO
/****** Object:  StoredProcedure [dbo].[DeleteDlgCardAdmin]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteDlgCardAdmin]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DeleteDlgCardAdmin]
GO
/****** Object:  StoredProcedure [dbo].[AddUserType]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddUserType]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AddUserType]
GO
/****** Object:  StoredProcedure [dbo].[AddUserMessage]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddUserMessage]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AddUserMessage]
GO
/****** Object:  StoredProcedure [dbo].[AddPromotion]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddPromotion]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[AddPromotion]
GO
/****** Object:  View [dbo].[uv_SaleOrderReport]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[uv_SaleOrderReport]'))
DROP VIEW [dbo].[uv_SaleOrderReport]
GO
/****** Object:  View [dbo].[uv_SaleItemsReport]    Script Date: 9/16/2015 5:36:48 PM ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[uv_SaleItemsReport]'))
DROP VIEW [dbo].[uv_SaleItemsReport]
GO
/****** Object:  View [dbo].[uv_SaleItemsReport]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[uv_SaleItemsReport]'))
EXEC dbo.sp_executesql @statement = N'
CREATE VIEW [dbo].[uv_SaleItemsReport]
AS
WITH CalculatedOrderItems AS (SELECT        OrderItemId, ItemName, OrderId, ItemId, UnitPrice, Quantity, ShippingCost, SiteId, ReserveDate, VatId, ItemCouponId, VatRate, ReductionRate, ReductionType, 
                                                                                               (CASE WHEN [ReductionType] = 0 THEN ISNULL([ReductionRate], 0) ELSE (([UnitPrice] * ISNULL([ReductionRate], 0)) / 100) END) AS Discount
                                                                     FROM            dbo.OrderItem)
    SELECT DISTINCT 
                              TOP (100) PERCENT dbo.[Order].OrderId, DATENAME(month, dbo.[Order].OrderDate) AS MonthName, DATEPART(wk, dbo.[Order].OrderDate) AS NumOfWeek, dbo.[Order].TransactionId, dbo.[Order].OrderNumber, 
                              ISNULL(dbo.ItemCategory.CategoryName, ''-'') AS CategoryName, dbo.Item.ItemCode, oi.ItemName, dbo.Item.ItemTypeId, oi.ReserveDate, dbo.SiteLang.SiteName AS GolfName, dbo.Users.FirstName, 
                              dbo.Users.LastName, dbo.[Order].PaymentType, oi.Quantity AS Qty, oi.Quantity * oi.UnitPrice AS TotalBasePrice, (oi.Discount * oi.Quantity) AS Discount, oi.ShippingCost, CONVERT(decimal(10, 2), 
                              (oi.UnitPrice - oi.Discount + oi.ShippingCost) * oi.Quantity) AS TotalTTC, CONVERT(decimal(10, 2), ((oi.UnitPrice - oi.Discount + oi.ShippingCost) * oi.Quantity) / ((oi.VatRate + 100) / 100)) AS TotalHT, 
                              dbo.GolfBrand.GolfBrandName, dbo.Item.SiteId, dbo.[Order].OrderDate, dbo.[Order].Active, dbo.SiteLang.LangId
     FROM            CalculatedOrderItems AS oi INNER JOIN
                              dbo.[Order] ON dbo.[Order].OrderId = oi.OrderId INNER JOIN
                              dbo.Item ON oi.ItemId = dbo.Item.ItemId LEFT OUTER JOIN
                              dbo.ItemCategory ON dbo.Item.CategoryId = dbo.ItemCategory.CategoryId LEFT OUTER JOIN
                              dbo.SiteLang ON dbo.Item.SiteId = dbo.SiteLang.SiteId LEFT OUTER JOIN
                              dbo.Users ON dbo.[Order].CustomerId = dbo.Users.UserId LEFT OUTER JOIN
                              dbo.Site ON dbo.Item.SiteId = dbo.Site.SiteId LEFT OUTER JOIN
                              dbo.GolfBrand ON dbo.Site.GolfBrandId = dbo.GolfBrand.GolfBrandId
     WHERE        (dbo.[Order].Active = 1)

' 
GO
/****** Object:  View [dbo].[uv_SaleOrderReport]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[uv_SaleOrderReport]'))
EXEC dbo.sp_executesql @statement = N'
CREATE VIEW [dbo].[uv_SaleOrderReport]
AS
WITH PreCalculatedOrderItems AS (SELECT        OrderItemId, ItemName, OrderId, ItemId, UnitPrice, Quantity, ShippingCost, SiteId, ReserveDate, VatId, ItemCouponId, VatRate, 
                                                                                                      ReductionRate, ReductionType, (CASE WHEN [ReductionType] = 0 THEN [ReductionRate] ELSE (([UnitPrice] * [ReductionRate]) 
                                                                                                      / 100) END) AS Discount
                                                                             FROM            dbo.OrderItem), CalculatedOrderItems AS
    (SELECT        SUM((UnitPrice - Discount + ShippingCost) * Quantity) AS TotalPrice, OrderId
      FROM            PreCalculatedOrderItems AS PreCalculatedOrderItems_1
      GROUP BY OrderId)
    SELECT        dbo.Users.SiteId, dbo.Users.FirstName + '' '' + dbo.Users.LastName AS CustomerName, dbo.Users.UserId, oi.TotalPrice, it.OrderId, it.OrderNumber, it.OrderDate, 
                              it.PaymentType, it.PaymentStatus, it.Active
     FROM            dbo.[Order] AS it INNER JOIN
                              dbo.Users ON it.CustomerId = dbo.Users.UserId INNER JOIN
                              CalculatedOrderItems AS oi ON oi.OrderId = it.OrderId

' 
GO
/****** Object:  StoredProcedure [dbo].[AddPromotion]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddPromotion]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[AddPromotion] AS' 
END
GO

-- =============================================
-- Author:		kit
-- Create date: <Create Date,,>
-- Description:	[AddPromotion]
-- =============================================
ALTER PROCEDURE [dbo].[AddPromotion]
	@PromotionContent nvarchar(255) = null,
	@PromotionTimecontent nvarchar(255) = null,
	@BrandNameId INT,
	@PromotionId Int Out
AS
BEGIN
	
	SET NOCOUNT ON;

	INSERT INTO [dbo].[Promotion]
			   ([PromotionContent]
			   ,[PromotionTimecontent]
			   ,[CreateDate]
			   ,[UpdateDate]
			   ,[Active]
			   ,[BrandNameId])
		 VALUES
			   (@PromotionContent
			   ,@PromotionTimecontent
			   ,GETUTCDATE()
			   ,GETUTCDATE()
			   ,1
			   ,@BrandNameId)

	 set @PromotionId = SCOPE_IDENTITY()
END


GO
/****** Object:  StoredProcedure [dbo].[AddUserMessage]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddUserMessage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[AddUserMessage] AS' 
END
GO

ALTER PROCEDURE [dbo].[AddUserMessage]
@Subject NVARCHAR(500) = null,
@FromUserId INT = null,
@ToUserId INT = null,
@Body NVARCHAR(500) = null,
@BaseFileName NVARCHAR(500) = null,
@FileExtension NVARCHAR(50) = null,
@RandomfileName NVARCHAR(500) = null,
@Result INT OUT
AS
BEGIN
	SET NOCOUNT ON;

	IF OBJECT_ID('tempdb..#Tempuser') IS NOT NULL DROP Table #Tempuser

	CREATE TABLE #Tempuser (UserId Int);

	IF @ToUserId = -1
	BEGIN
		INSERT INTO #Tempuser (UserId)
		select [UserId] FROM [dbo].[Users]
	END
	ELSE
	BEGIN
		INSERT INTO #Tempuser (UserId)
		select [UserId] FROM [dbo].[Users] WHERE [UserId] = @ToUserId
	END


	DECLARE @tempuserid int 
	DECLARE @MessageId int
	Declare @hasfile bit = 0

	IF @FileExtension IS NOT NULL
	BEGIN
		set @hasfile = 1
	END

	while EXISTS(SELECT TOP 1 1 FROM #Tempuser)
	BEGIN
		--Get row from cursor to process
		SELECT TOP 1 @tempuserid = [UserId]  FROM #Tempuser  

		INSERT INTO [dbo].[UserMessage]
           (
           [Subject]
           ,[FromUserId]
           ,[ToUserId]
           ,[Body]
           ,[SentDate]
           ,[ReadDate]
           ,[Active]
		   ,[HasAttachedFile]
           )
     VALUES
           (@Subject 
           ,@FromUserId
           ,@tempuserid
           ,@Body
           ,GETUTCDATE()
           ,null
           ,1
		   ,@hasfile)

		 SET @MessageId =  SCOPE_IDENTITY()

		 If @FileExtension IS NOT NULL
		 BEGIN
			 INSERT INTO [dbo].[UserMessageAttached]
			   ([MessageId]
			   ,[BaseFileName]
			   ,[FileExtension]
			   ,[FileName]
			   ,[InsertDate])
			 VALUES
				   (@MessageId
				   ,@BaseFileName
				   ,@FileExtension
				   ,@RandomfileName
				   ,GETUTCDATE())
		 END

		--Remove processed row from cursor.
		DELETE FROM #Tempuser WHERE [UserId] = @tempuserid  
	END
	DROP TABLE #Tempuser
	                                 
         
	SET @Result = 1	

END


GO
/****** Object:  StoredProcedure [dbo].[AddUserType]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AddUserType]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[AddUserType] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single User Type
-- =============================================
ALTER PROCEDURE [dbo].[AddUserType]
	@UserTypeId INT = NULL,
	@UserTypeName NVARCHAR(50) = NULL,
	@InsertedId INT OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [UserTypes](UserTypeId, UserTypeName) VALUES(@UserTypeId, @UserTypeName)
	
	SET @InsertedId = @@IDENTITY
END


GO
/****** Object:  StoredProcedure [dbo].[DeleteDlgCardAdmin]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteDlgCardAdmin]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[DeleteDlgCardAdmin] AS' 
END
GO

ALTER PROCEDURE [dbo].[DeleteDlgCardAdmin]
@ItemId int ,
@result int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[Item] SET [Active] = 0 WHERE [ItemId] = @ItemId



	SET @result = 1
END


GO
/****** Object:  StoredProcedure [dbo].[DeleteImageDlgCardAdmin]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteImageDlgCardAdmin]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[DeleteImageDlgCardAdmin] AS' 
END
GO

ALTER PROCEDURE [dbo].[DeleteImageDlgCardAdmin]
@ItemId Int
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM [dbo].[DLGCardStyle]
    WHERE [ItemId] = @ItemId
END 


GO
/****** Object:  StoredProcedure [dbo].[DeletePriceDlgCardItemAdmin]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeletePriceDlgCardItemAdmin]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[DeletePriceDlgCardItemAdmin] AS' 
END
GO

ALTER PROCEDURE [dbo].[DeletePriceDlgCardItemAdmin]
@ItemId  int
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM [dbo].[ItemPrice]
      WHERE [ItemId] = @ItemId
END




GO
/****** Object:  StoredProcedure [dbo].[DeletePromotionAdmin]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeletePromotionAdmin]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[DeletePromotionAdmin] AS' 
END
GO

ALTER PROCEDURE [dbo].[DeletePromotionAdmin]
@PromotionId int ,
@result int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[Promotion] SET [Active] = 0 WHERE [PromotionId] = @PromotionId



	SET @result = 1
END


GO
/****** Object:  StoredProcedure [dbo].[DeleteUserMessageAdmin]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteUserMessageAdmin]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[DeleteUserMessageAdmin] AS' 
END
GO

ALTER PROCEDURE [dbo].[DeleteUserMessageAdmin]
@MessageId int ,
@result int OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE [dbo].[UserMessage] SET [Active] = 0 WHERE [MessageId] = @MessageId
	SET @result = 1
END



GO
/****** Object:  StoredProcedure [dbo].[DeleteUserType]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DeleteUserType]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[DeleteUserType] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single User Type
-- =============================================
ALTER PROCEDURE [dbo].[DeleteUserType]
	@UserTypeId INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM [UserTypes] WHERE UserTypeId = @UserTypeId
END


GO
/****** Object:  StoredProcedure [dbo].[DLGCardBalanceById]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DLGCardBalanceById]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[DLGCardBalanceById] AS' 
END
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[DLGCardBalanceById]
@DlgcardId int = null
AS
BEGIN

	SET NOCOUNT ON;
	SELECT TOP 1000 [Id]
		  ,[DLGCardId]
		  ,[UserId]
		  ,[ActionType]
		  ,[Description]
		  ,[Debit]
		  ,[Credit]
		  ,[Balance]
		  ,[InsertDate]
		  ,[SaleId]
		  ,[Active]
	  FROM [dbo].[DLGCardBalance]
	  WHERE [DLGCardId] = @DlgcardId

END

GO
/****** Object:  StoredProcedure [dbo].[GetAccountOrderDetail]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAccountOrderDetail]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetAccountOrderDetail] AS' 
END
GO

ALTER PROCEDURE [dbo].[GetAccountOrderDetail]
@OrderId INT = NULL
AS
BEGIN
SET NOCOUNT ON;
	SELECT CategoryName, ItemName, UnitPrice, Quantity, UnitPrice * Quantity AS TotalPrice
	FROM OrderItem
		LEFT OUTER JOIN Item ON OrderItem.ItemId = Item.ItemId
		LEFT OUTER JOIN ItemCategory ON Item.CategoryId = ItemCategory.CategoryId
	WHERE OrderItem.OrderId = @OrderId
END



GO
/****** Object:  StoredProcedure [dbo].[GetAccountOrderList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAccountOrderList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetAccountOrderList] AS' 
END
GO

ALTER PROCEDURE [dbo].[GetAccountOrderList]
@UserId INT = NULL
AS
BEGIN
	SET NOCOUNT ON;
	SELECT [Order].OrderId,CustomerId, 
		ISNULL((SELECT TOP 1 CategoryName 
		FROM OrderItem 
			INNER JOIN Item ON OrderItem.ItemId = Item.ItemId
			INNER JOIN ItemCategory ON Item.CategoryId = ItemCategory.CategoryId
		WHERE OrderItem.OrderId = [Order].OrderId), '') AS CategoryName,
		TransactionId, [OrderNumber], [OrderDate], SUM(UnitPrice * Quantity) AS TotalPrice, '' AS ShippingStatus
	FROM [Order]
		INNER JOIN [OrderItem] ON [Order].OrderId = [OrderItem].OrderId
	WHERE [Order].Active = 1 
	AND (@UserId IS NULL OR CustomerId = @UserId)
	GROUP BY [Order].OrderId, [OrderNumber], [OrderDate], TransactionId,CustomerId
END


GO
/****** Object:  StoredProcedure [dbo].[GetAllPromotion]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllPromotion]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetAllPromotion] AS' 
END
GO
/****** Script for SelectTopNRows command from SSMS  ******/
ALTER PROCEDURE [dbo].[GetAllPromotion]
	@StartIndex INT = NULL,
    @MaxRetriveItem INT = NULL,
    @SortColumnName NVARCHAR(50) = NULL,
    @SortDirection INT = NULL,
	@SearchText NVARCHAR(50) = NULL
AS
BEGIN

SET NOCOUNT ON;
WITH CTE_Result AS
	(
SELECT [pr].[PromotionId]
      ,[pr].[PromotionImage]
      ,[pr].[PromotionBrandImage]
      ,[pr].[PromotionContent]
      ,[pr].[PromotionTimecontent]
      ,[pr].[CreateDate]
      ,[pr].[UpdateDate]
      ,[pr].[Active]
      ,[pr].[BrandNameId]
	   ,[br].[BrandName]
	  ,ROW_NUMBER() OVER(
										ORDER BY 
												CASE WHEN @SortDirection = 0 THEN 
													CASE 
														WHEN @SortColumnName IS NULL THEN  [PromotionId]
													END
												END ASC, 
												CASE WHEN @SortDirection = 1 THEN 
													CASE 
															WHEN @SortColumnName IS NULL THEN [PromotionId]
													END
												END DESC
									) AS RowNumber
  FROM [dbo].[Promotion] pr
  INNER JOIN [dbo].[Brand] br  ON pr.BrandNameId = br.BrandId
  WHERE [pr].[Active] = 1
  AND (@SearchText IS NULL OR [pr].[BrandNameId] LIKE '%' + @SearchText +'%')
  )

  SELECT *,  (SELECT MAX(RowNumber) FROM CTE_Result) ToltalItemCount
	FROM CTE_Result
	WHERE 
			RowNumber > @StartIndex
			AND  (@MaxRetriveItem IS NULL OR @MaxRetriveItem <= 0 OR (RowNumber <=  (@StartIndex + @MaxRetriveItem)))

  END

GO
/****** Object:  StoredProcedure [dbo].[GetAllUser]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetAllUser]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetAllUser] AS' 
END
GO

	ALTER PROCEDURE [dbo].[GetAllUser]
	AS
	BEGIN

		SET NOCOUNT ON;

		SELECT
		   [UserId]
		  ,[FirstName]
		  ,[LastName]
	  FROM [dbo].[Users]
	  WHERE [Active] = 1

	END

GO
/****** Object:  StoredProcedure [dbo].[GetCreditcardassociations]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetCreditcardassociations]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetCreditcardassociations] AS' 
END
GO
ALTER PROCEDURE [dbo].[GetCreditcardassociations]
AS
BEGIN

	SET NOCOUNT ON;
	SELECT [CardassociationsId]
      ,[CardassociationsType]
      ,[Active]
      ,[CreateDate]
  FROM [dbo].[Creditcardassociations]
  WHERE [Active] = 1

END
GO
/****** Object:  StoredProcedure [dbo].[GetDlgCardHistory]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDlgCardHistory]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetDlgCardHistory] AS' 
END
GO

-- =============================================
-- Author:		kit
-- Create date: 2014-11-25
-- Description:	[dbo].[GetDlgCardHistory]
-- =============================================
ALTER PROCEDURE [dbo].[GetDlgCardHistory]
AS
BEGIN

	SET NOCOUNT ON;
	SELECT [DLGCardId]
      ,[ItemId]
      ,[SaleId]
      ,[FirstName]
      ,[LastName]
      ,[Email]
      ,[CardNumber]
      ,[Message]
      ,[BeginBalance]
      ,[InsertDate]
      ,[UpdateDate]
      ,[UserId]
      ,[Active]
      ,[SelectedCardStyleId]
  FROM [dbo].[DLGCard]
END


GO
/****** Object:  StoredProcedure [dbo].[GetDLGCardStyle]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDLGCardStyle]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetDLGCardStyle] AS' 
END
GO
ALTER PROCEDURE [dbo].[GetDLGCardStyle]
@ItemId INT = 0 
AS
BEGIN
	SET NOCOUNT ON;
	
  SELECT [CardStyleId]
      ,[ItemId]
      ,[ImageName]
      ,[ListNo]
  FROM [dbo].[DLGCardStyle]
  WHERE [ItemId] = @ItemId

END

GO
/****** Object:  StoredProcedure [dbo].[GetInterest]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetInterest]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetInterest] AS' 
END
GO
-- =============================================
-- Author:		kit
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[GetInterest]
	@LangId INT 
AS
BEGIN

	SET NOCOUNT ON;

   SELECT il.[InterestId]
      ,il.[LangId]
      ,il.[InterestName]
      ,il.[InterestDesc]
	FROM [dbo].[InterestLang] il
	INNER JOIN [dbo].[Interest] i ON il.InterestId = i.InterestId 
	WHERE [LangId] = @LangId
	AND [i].[Active] = 1
END


GO
/****** Object:  StoredProcedure [dbo].[GetItemDlgCardByItemTypeId]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetItemDlgCardByItemTypeId]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetItemDlgCardByItemTypeId] AS' 
END
GO
ALTER PROCEDURE [dbo].[GetItemDlgCardByItemTypeId]
	@ItemId  int = NULL ,
	@ItemTypeId Int = 0,
	@StartIndex INT = NULL,
    @MaxRetriveItem INT = NULL,
    @SortColumnName NVARCHAR(50) = NULL,
    @SortDirection INT = NULL,
	@SearchText NVARCHAR(50) = NULL
AS
BEGIN
	SET NOCOUNT ON;
	WITH CTE_Result AS
	(
		SELECT [Item].*, [ItemCategory].[CategoryName], img.[ItemImageId], img.[ListNo], img.[ImageName], img.[BaseName], img.[FileExtension], [DLGCard].[Message]
		,ROW_NUMBER() OVER(
										ORDER BY 
												CASE WHEN @SortDirection = 0 THEN 
													CASE 
														WHEN @SortColumnName IS NULL THEN  [Item].ItemCode
													END
												END ASC, 
												CASE WHEN @SortDirection = 1 THEN 
													CASE 
															WHEN @SortColumnName IS NULL THEN [Item].ItemCode
													END
												END DESC
									) AS RowNumber
		FROM [Item]
		LEFT JOIN [ItemCategory] ON [ItemCategory].[CategoryId] = [Item].[CategoryId]
		LEFT JOIN [ItemImage] img ON img.[ItemImageId] = (SELECT TOP(1) [ItemImageId] FROM [ItemImage] WHERE [ItemImage].[ItemId] = [Item].[ItemId] ORDER BY [ItemImage].[ListNo], [ItemImage].[ItemImageId])
		LEFT JOIN [DLGCard] ON [DLGCard].[ItemId] = [Item].[ItemId]
		where [Item].ItemTypeId = 6 
		AND [Item].Active = 1
		AND (@SearchText IS NULL OR [Item].ItemCode LIKE '%' + @SearchText +'%')
		AND (@ItemId IS NULL OR [Item].[ItemId] = @ItemId)

	)
	SELECT *,  (SELECT MAX(RowNumber) FROM CTE_Result) ToltalItemCount
	FROM CTE_Result
	WHERE 
			RowNumber > @StartIndex
			AND  (@MaxRetriveItem IS NULL OR @MaxRetriveItem <= 0 OR (RowNumber <=  (@StartIndex + @MaxRetriveItem)))

END
GO
/****** Object:  StoredProcedure [dbo].[GetItemPriceDlgCard]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetItemPriceDlgCard]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetItemPriceDlgCard] AS' 
END
GO
ALTER PROCEDURE [dbo].[GetItemPriceDlgCard]
	@ItemId INT = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [ItemPriceId]
      ,[ItemId]
      ,[SiteId]
      ,[CustomerTypeId]
      ,[StartDate]
      ,[EndDate]
      ,[Price]
      ,[InsertDate]
      ,[UpdateDate]
      ,[Active]
      ,[UserId]
  FROM [dbo].[ItemPrice]
  WHERE [ItemId] = @ItemId

END

GO
/****** Object:  StoredProcedure [dbo].[GetPromotion]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPromotion]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetPromotion] AS' 
END
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[GetPromotion]
	@PromotionId Int
AS
BEGIN

	SET NOCOUNT ON;

	SELECT [PromotionId]
		  ,[PromotionImage]
		  ,[PromotionBrandImage]
		  ,[PromotionContent]
		  ,[PromotionTimecontent]
		  ,[CreateDate]
		  ,[UpdateDate]
		  ,[Active]
		  ,[BrandNameId]
	  FROM [dbo].[Promotion]
	  Where [PromotionId] = @PromotionId AND [Active] = 1
END


GO
/****** Object:  StoredProcedure [dbo].[GetSalesReport]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetSalesReport]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetSalesReport] AS' 
END
GO
ALTER PROCEDURE [dbo].[GetSalesReport]
@FromDate nvarchar(256) = null,
@ToDate	 nvarchar(256) = null,
@SideId INT = null,
@OrderBy  nvarchar(256) = null 
AS 
Begin
DECLARE @SQLQuery AS NVARCHAR(500)
  
  
SET @SQLQuery = 'select 
SiteId,CustomerName,UserId,IsNull(TotalPrice,0) AS TotalPrice,
OrderId,OrderNumber,
OrderDate,PaymentType,PaymentStatus, Active
 from uv_SaleOrderReport WHERE  OrderDate BETWEEN 
 
  CONVERT(datetime,''' +@FromDate +' 00:00:00'',103)
 AND 
  CONVERT(datetime,''' +@ToDate +' 23:59:59'',103) '
   
   
    IF(@SideId IS NOT NULL) 
        SET @SQLQuery = @SQLQuery + ' AND SiteId = ' + CAST(@SideId AS NVARCHAR)
        
    IF(@OrderBy IS NOT NULL) OR (LEN(@OrderBy) > 0) 
        SET @SQLQuery = @SQLQuery + ' Order By ' + @OrderBy
	  
/* Execute Transact-SQL String */
print @SQLQuery;
 EXECUTE(@SQLQuery)
		  
		  
END 
GO
/****** Object:  StoredProcedure [dbo].[GetTotalUserMessageById]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetTotalUserMessageById]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetTotalUserMessageById] AS' 
END
GO
-- =============================================
-- Author:		<Kid>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[GetTotalUserMessageById] 
	@UserId INT = null,
	@result INT OUTPUT
AS
BEGIN

	SET NOCOUNT ON;

  SET @result = (SELECT COUNT(*)
  FROM [dbo].[UserMessage]
  WHERE [ToUserId] = @UserId AND [ReadDate] Is null)
END


GO
/****** Object:  StoredProcedure [dbo].[GetUserMessage]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetUserMessage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[GetUserMessage] AS' 
END
GO
ALTER PROCEDURE [dbo].[GetUserMessage]
	@StartIndex INT = NULL,
    @MaxRetriveItem INT = NULL,
    @SortColumnName NVARCHAR(50) = NULL,
    @SortDirection INT = NULL,
	@SearchText NVARCHAR(50) = NULL,
	@UserId INT = NULL,
	@messageId INT = NULL
	AS
	BEGIN
		SET NOCOUNT ON;
		WITH CTE_Result AS
	(
		SELECT [Umess].[MessageId],[Umess].[MessageTypeId] ,[Umess].[Subject],[Umess].[FromUserId],[Umess].[ToUserId],[Umess].[Body],
		[Umess].[SentDate],[Umess].[ReadDate],[Umess].[Active],[Umess].[HasAttachedFile],[Umess].[IsFlag]
		,[UmessAtt].[AttachedId],[UmessAtt].[BaseFileName],[UmessAtt].[FileExtension],[UmessAtt].[FileName],[UmessAtt].[InsertDate],
		us.FirstName,us.LastName,
		(SELECT [FirstName] + ' ' + [LastName] from [dbo].[Users] WHERE [userId] =[Umess].[FromUserId]) as FromName
		,(SELECT [FirstName] + ' ' + [LastName] from [dbo].[Users] WHERE [userId] =[Umess].[ToUserId]) as ToName
		,ROW_NUMBER() OVER(
										ORDER BY 
												CASE WHEN @SortDirection = 0 THEN 
													CASE 
														WHEN @SortColumnName IS NULL THEN  [Umess].[MessageId]
													END
												END ASC, 
												CASE WHEN @SortDirection = 1 THEN 
													CASE 
															WHEN @SortColumnName IS NULL THEN [Umess].[MessageId]
													END
												END DESC
									) AS RowNumber
		  FROM [dbo].[UserMessage] Umess
		  Left JOIN [dbo].[UserMessageAttached] UmessAtt ON Umess.MessageId = UmessAtt.MessageId
		  INNER JOIN [dbo].[Users] us ON Umess.FromUserId = us.UserId 
		  WHERE (@SearchText IS NULL OR [Umess].[Subject] LIKE '%' + @SearchText +'%')
		  AND (@UserId IS NULL OR [Umess].ToUserId = @UserId)
		  AND (@messageId IS NULL OR [Umess].MessageId = @messageId)
	)

	SELECT *,  (SELECT MAX(RowNumber) FROM CTE_Result) ToltalItemCount
	FROM CTE_Result
	WHERE 
			RowNumber > @StartIndex
			AND  (@MaxRetriveItem IS NULL OR @MaxRetriveItem <= 0 OR (RowNumber <=  (@StartIndex + @MaxRetriveItem)))
	ORDER BY [MessageId] DESC 
	END

GO
/****** Object:  StoredProcedure [dbo].[RandomDigit]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RandomDigit]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[RandomDigit] AS' 
END
GO

ALTER PROCEDURE [dbo].[RandomDigit]
@randigit BIGINT OUTPUT
AS
BEGIN
	declare @ran BIGINT
	SET @ran =  CAST(round(RAND()*10000000000000000,0) AS BIGINT)
	IF EXISTS (select Top 1 1 from [DLGCard] where [CardNumber] = CAST(@randigit as nvarchar(16)))
	BEGIN
		exec RandomDigit
	END
	ELSE
	BEGIN
		set @randigit = @ran
	END

END

GO
/****** Object:  StoredProcedure [dbo].[SaveDLGCard]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaveDLGCard]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SaveDLGCard] AS' 
END
GO

ALTER PROCEDURE [dbo].[SaveDLGCard]
			@ItemId int
           ,@SaleId int
           ,@FirstName nvarchar(200)
           ,@LastName nvarchar(200)
           ,@Email nvarchar(200)
           ,@CardNumber nvarchar(100)
           ,@Message  nvarchar(1000)
           ,@itemPriceDlgCardId int
           ,@UserId int
           ,@SelectedCardStyleId int
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @BeginBalance money;
	SET @BeginBalance = (select [Price] from [dbo].[ItemPrice] where [ItemPriceId] = @itemPriceDlgCardId )

	INSERT INTO [dbo].[DLGCard]
           ([ItemId]
           ,[SaleId]
           ,[FirstName]
           ,[LastName]
           ,[Email]
           ,[CardNumber]
           ,[Message]
           ,[BeginBalance]
           ,[InsertDate]
           ,[UpdateDate]
           ,[UserId]
           ,[Active]
           ,[SelectedCardStyleId])
     VALUES
           (@ItemId
           ,@SaleId
           ,@FirstName
           ,@LastName
           ,@Email
           ,@CardNumber
           ,@Message
           ,@BeginBalance
           ,GETUTCDATE()
           ,GETUTCDATE()
           ,@UserId
           ,@UserId
           ,@SelectedCardStyleId)

	DECLARE @newDLGCardId int 
	SET @newDLGCardId = Scope_Identity() 


	INSERT INTO [dbo].[DLGCardBalance]
           ([DLGCardId]
           ,[UserId]
           ,[ActionType]
           ,[Description]
           ,[Debit]
           ,[Credit]
           ,[Balance]
           ,[InsertDate]
           ,[SaleId]
           ,[Active])
     VALUES
           (@newDLGCardId
           ,@UserId
           ,@UserId
           ,'Init new card'
           ,0
           ,@BeginBalance
           ,@BeginBalance
           ,GETUTCDATE()
           ,@SaleId
           ,1)
END



GO
/****** Object:  StoredProcedure [dbo].[SaveDlgCardItemAdmin]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaveDlgCardItemAdmin]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SaveDlgCardItemAdmin] AS' 
END
GO

ALTER PROCEDURE [dbo].[SaveDlgCardItemAdmin]
@ItemIdIn Int = NULL,
@CardName NVARCHAR(500),
@UserId int,
@ItemId int OUTPUT 
AS
BEGIN
	SET NOCOUNT ON;

	IF EXISTS(select top 1 1 from [dbo].[Item] where [ItemId] = @ItemIdIn)
	BEGIN
		UPDATE [dbo].[Item] SET [ItemCode] = @CardName , [ItemSlug] = @CardName where [ItemId] = @ItemIdIn

		SET @ItemId = @ItemIdIn
	END
	ELSE
	BEGIN
	INSERT INTO [dbo].[Item]
           ([CategoryId]
           ,[ItemTypeId]
           ,[ItemCode]
           ,[ItemSlug]
           ,[InsertDate]
           ,[Active]
           ,[UserId] 
           ,[IsPublish]
           ,[UnitInStock]
           ,[IsShowOnHomepage]
           ,[CourseId])

     VALUES
           (0
           ,6
           ,@CardName
           ,@CardName
           ,GETUTCDATE()
           ,1
           ,@UserId
           ,1
           ,0
           ,1
           ,0)

		   SET @ItemId = SCOPE_IDENTITY()
	END


	


END


GO
/****** Object:  StoredProcedure [dbo].[SaveItemImage]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaveItemImage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SaveItemImage] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-04-12
-- Description:	Save Item Images
-- =============================================
ALTER PROCEDURE [dbo].[SaveItemImage]
	@ItemImageId INT = -1,
	@ItemId INT = NULL,
	@ImageName NVARCHAR(255) = NULL,
	@ListNo INT = NULL,
	@BaseName NVARCHAR(255) = NULL,
	@FileExtension NVARCHAR(10) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT ItemImageId FROM [ItemImage] WHERE ItemImageId = @ItemImageId)
	BEGIN
		INSERT INTO [ItemImage](ItemId, ImageName, ListNo, BaseName, FileExtension) VALUES(@ItemId, @ImageName, @ListNo, @BaseName, @FileExtension)
	END
	ELSE
	BEGIN
		UPDATE [ItemImage]
		SET
			ItemId = @ItemId,
			ImageName = @ImageName,
			ListNo = @ListNo,
			BaseName = @BaseName,
			FileExtension = @FileExtension
		WHERE ItemImageId = @ItemImageId
	END
END


GO
/****** Object:  StoredProcedure [dbo].[SavePriceDlgCardItemAdmin]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SavePriceDlgCardItemAdmin]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SavePriceDlgCardItemAdmin] AS' 
END
GO

ALTER PROCEDURE [dbo].[SavePriceDlgCardItemAdmin]
@ItemId INT,
@Price money,
@UserId int
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[ItemPrice] ([ItemId],[Price],[InsertDate],[Active],[UserId]) VALUES (@ItemId,@Price,GETUTCDATE(),1,@UserId)
END


GO
/****** Object:  StoredProcedure [dbo].[SavePrimaryImageDlgCardAdmin]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SavePrimaryImageDlgCardAdmin]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SavePrimaryImageDlgCardAdmin] AS' 
END
GO

ALTER PROCEDURE [dbo].[SavePrimaryImageDlgCardAdmin]
@ItemId int ,
@ImageName NVARCHAR(255),
@BaseName NVARCHAR(255),
@FileExtension NVARCHAR(10)
AS
BEGIN
	SET NOCOUNT ON;

	DELETE FROM [dbo].[ItemImage] WHERE [ItemId]= @ItemId;

	INSERT INTO [dbo].[ItemImage]
           ([ItemId]
           ,[ImageName]
           ,[ListNo]
           ,[BaseName]
           ,[FileExtension])
     VALUES
           (@ItemId
           ,@ImageName
           ,1
           ,@BaseName
           ,@FileExtension)

END


GO
/****** Object:  StoredProcedure [dbo].[SaveSiteImage]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaveSiteImage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SaveSiteImage] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-04-12
-- Description:	Save Site Images
-- =============================================
ALTER PROCEDURE [dbo].[SaveSiteImage]
	@SiteImageId INT = -1,
	@SiteId INT = NULL,
	@ImageName NVARCHAR(255) = NULL,
	@ListNo INT = NULL,
	@BaseName NVARCHAR(255) = NULL,
	@FileExtension NVARCHAR(10) = NULL
AS
BEGIN
	SET NOCOUNT ON;

	IF NOT EXISTS(SELECT SiteImageId FROM [SiteImage] WHERE SiteImageId = @SiteImageId)
	BEGIN
		INSERT INTO [SiteImage](SiteId, ImageName, ListNo, BaseName, FileExtension) VALUES(@SiteId, @ImageName, @ListNo, @BaseName, @FileExtension)
	END
	ELSE
	BEGIN
		UPDATE [SiteImage]
		SET
			SiteId = @SiteId,
			ImageName = @ImageName,
			ListNo = @ListNo,
			BaseName = @BaseName,
			FileExtension = @FileExtension
		WHERE SiteImageId = @SiteImageId
	END
END


GO
/****** Object:  StoredProcedure [dbo].[SaveStyleImageDlgCardAdmin]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaveStyleImageDlgCardAdmin]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SaveStyleImageDlgCardAdmin] AS' 
END
GO

ALTER PROCEDURE [dbo].[SaveStyleImageDlgCardAdmin]
@ItemId int,
@ImageName NVARCHAR(255),
@ListNo int
As
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[DLGCardStyle] ([ItemId],[ImageName],[ListNo])
	VALUES(@ItemId, @ImageName,@ListNo)
END


GO
/****** Object:  StoredProcedure [dbo].[SearchGreenFee]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SearchGreenFee]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SearchGreenFee] AS' 
END
GO
ALTER PROCEDURE [dbo].[SearchGreenFee]
	@PageSize INT = NULL,
	@PageIndex INT = NULL,
	@RegionId INT = 0,
	@StateId INT = 0,
	@SiteId INT = 0,
	@FromDate DATETIME = NULL,
	@ToDate DATETIME = NULL,
	@TimeSlot INT = NULL,
	@IncludePractice BIT = NULL,
	@CategoryId INT = NULL,
	@NotIn NVARCHAR(MAX) = NULL,
	@LangId INT = 1
AS
BEGIN

	DECLARE @SQL NVARCHAR(MAX) = ''
	DECLARE @WITH NVARCHAR(MAX) = ''
	DECLARE @SELECT NVARCHAR(MAX) = ''
	DECLARE @FROM NVARCHAR(MAX) = ''
	DECLARE @WHERE NVARCHAR(MAX) = ''
	DECLARE @ORDER NVARCHAR(MAX) = ''
	DECLARE @FILTERS NVARCHAR(MAX) = ''
	
	SET NOCOUNT ON;

	SET @SELECT = ' [Item].*, [ItemLang].[ItemName], [ItemLang].[ItemShortDesc], img.[ItemImageId], img.[ListNo], img.[ImageName], img.[BaseName], img.[FileExtension], [SiteLang].[SiteName], ISNULL((SELECT AVG(Rating) FROM [ItemReview] WHERE [ItemReview].[ItemId] = [Item].[ItemId] AND [ItemReview].[IsApproved] = 1), 0) AS AverageRating, ISNULL(ipp.[Price], 0) AS PeriodPrice, ISNULL(idp.[Price], 0) AS SpecialPrice,
	(SELECT MIN([TeeSheet].[Discount]) AS [CheapestPrice] FROM [TeeSheet] WHERE [TeeSheet].[Price] > 0 AND [TeeSheet].[Discount] > 0 AND [TeeSheet].[ItemId] = [Item].[ItemId] AND [TeeSheet].[TeeSheetDate] >= GETDATE()) AS TeeSheetCheapestPrice, [Site].[AlbatrosCourseId]'

	SET @WITH = 'WITH GreenFees AS ('
	SET @WITH = @WITH + ' SELECT DISTINCT ' + @SELECT
	SET @WITH = @WITH + ' FROM [Item] 
	LEFT JOIN [ItemCategory] ON [ItemCategory].[CategoryId] = [Item].[CategoryId]
	LEFT JOIN [SiteLang] ON [Item].[SiteId] = [SiteLang].[SiteId] AND [SiteLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) +  CHAR(13) + CHAR(10) +
	' LEFT JOIN [Site] ON [Site].[SiteId] = [Item].[SiteId] AND [Site].[Active] = 1 AND [Site].[Visible] = 1' + CHAR(13) + CHAR(10) +
	' LEFT JOIN [State] ON [State].[StateId] = [Site].[StateId]' + CHAR(13) + CHAR(10) +
	' LEFT JOIN [Region] ON [Region].[RegionId] = [State].[RegionId]' + CHAR(13) + CHAR(10) +
	' LEFT JOIN [TeeSheet] ON [TeeSheet].[ItemId] = [Item].[ItemId]' + CHAR(13) + CHAR(10) +
	' LEFT JOIN [ItemLang] ON [ItemLang].[ItemId] = [Item].[ItemId] AND [ItemLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) +
	' LEFT JOIN [ItemImage] img ON img.[ItemImageId] = (SELECT TOP(1) [ItemImageId] FROM [ItemImage] WHERE [ItemImage].[ItemId] = [Item].[ItemId] ORDER BY [ItemImage].[ListNo], [ItemImage].[ItemImageId]) ' + CHAR(13) + CHAR(10) +
	' LEFT JOIN (SELECT [ItemId],[Price] FROM [ItemPrice] WHERE [PriceType] = 0 AND GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate])) ipp ON ipp.[ItemId] = [Item].[ItemId] ' + CHAR(13) + CHAR(10) +
	' LEFT JOIN (SELECT [ItemId],[Price] FROM [ItemPrice] WHERE [PriceType] = 1 AND GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate])) idp ON idp.[ItemId] = [Item].[ItemId] ' + CHAR(13) + CHAR(10) +
	' WHERE [Item].[Active] = 1 AND [Item].[IsPublish] = 1 AND [Item].[ItemTypeId] = 2 AND ISNULL([Site].[AlbatrosCourseId], 0) <= 0
		AND (PublishStartDate IS NULL OR PublishEndDate IS NULL OR (GETDATE() BETWEEN PublishStartDate AND PublishEndDate))' + CHAR(13) + CHAR(10)
	
		IF(@NotIn IS NOT NULL AND @NotIn <> '')
		BEGIN
			SET @WITH = @WITH + ' AND [Item].[ItemId] NOT IN(' + @NotIn + ')'
		END
		
		IF(@FromDate IS NOT NULL AND @ToDate IS NOT NULL)
			SET @WITH = @WITH + ' AND ([TeeSheet].[TeeSheetDate] BETWEEN ''' + CONVERT(VARCHAR, @FromDate, 21) +''' AND ''' + CONVERT(VARCHAR, @ToDate, 21) + ''')' + CHAR(13) + CHAR(10)

		IF(ISNULL(@SiteId, 0) > 0)
			SET @WITH = @WITH + ' AND Item.SiteId = ' + CAST(@SiteId AS NVARCHAR) + CHAR(13) + CHAR(10)
		ELSE IF(ISNULL(@StateId, 0) > 0)
			SET @WITH = @WITH + ' AND [Site].StateId = ' + CAST(@StateId AS NVARCHAR) + CHAR(13) + CHAR(10)
		ELSE IF(ISNULL(@RegionId, 0) > 0)
			SET @WITH = @WITH + ' AND [State].RegionId = ' + CAST(@RegionId AS NVARCHAR) + CHAR(13) + CHAR(10)
		
		IF(ISNULL(@CategoryId, 0) > 0)
			SET @WITH = @WITH + ' AND [Item].[CategoryId] = ' + CAST(@CategoryId AS NVARCHAR) + CHAR(13) + CHAR(10)
		IF(ISNULL(@TimeSlot, 0) > 0)
			SET @WITH = @WITH + ' AND [TeeSheet].[FromTime] = ''' + CAST(@TimeSlot AS NVARCHAR) + ':00'''
			
		IF(@IncludePractice IS NOT NULL)
			SET @WITH = @WITH + ' AND ISNULL([Item].[IncludePractice], 0) = ' + CAST(@IncludePractice AS NVARCHAR)
	 	
	 	IF(LEN(@WHERE) > 0)
			SET @WITH = @WITH + ' AND ([Item].[ItemCode] LIKE N''%' + @WHERE + '%'' OR [ItemLang].[ItemName] LIKE N''%' + @WHERE + '%'')'
		
	
	-- UNION Albatros TeeSheet...
	SET @WITH = @WITH + 'UNION'
	
	SET @WITH = @WITH + ' SELECT DISTINCT ' + @SELECT
	SET @WITH = @WITH + ' FROM [Item] 
	LEFT JOIN [ItemCategory] ON [ItemCategory].[CategoryId] = [Item].[CategoryId]
	LEFT JOIN [SiteLang] ON [Item].[SiteId] = [SiteLang].[SiteId] AND [SiteLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) +  CHAR(13) + CHAR(10) +
	' LEFT JOIN [Site] ON [Site].[SiteId] = [Item].[SiteId] AND [Site].[Active] = 1 AND [Site].[Visible] = 1' + CHAR(13) + CHAR(10) +
	' LEFT JOIN [State] ON [State].[StateId] = [Site].[StateId]' + CHAR(13) + CHAR(10) +
	' LEFT JOIN [Region] ON [Region].[RegionId] = [State].[RegionId]' + CHAR(13) + CHAR(10) +
	' LEFT JOIN [ItemLang] ON [ItemLang].[ItemId] = [Item].[ItemId] AND [ItemLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) +
	' LEFT JOIN [ItemImage] img ON img.[ItemImageId] = (SELECT TOP(1) [ItemImageId] FROM [ItemImage] WHERE [ItemImage].[ItemId] = [Item].[ItemId] ORDER BY [ItemImage].[ListNo], [ItemImage].[ItemImageId]) ' + CHAR(13) + CHAR(10) +
	' LEFT JOIN (SELECT [ItemId],[Price] FROM [ItemPrice] WHERE [PriceType] = 0 AND GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate])) ipp ON ipp.[ItemId] = [Item].[ItemId] ' + CHAR(13) + CHAR(10) +
	' LEFT JOIN (SELECT [ItemId],[Price] FROM [ItemPrice] WHERE [PriceType] = 1 AND GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate])) idp ON idp.[ItemId] = [Item].[ItemId] ' + CHAR(13) + CHAR(10) +
	' WHERE [Item].[Active] = 1 AND [Item].[IsPublish] = 1 AND [Item].[ItemTypeId] = 2 AND ISNULL([Site].[AlbatrosCourseId], 0) > 0
		AND (PublishStartDate IS NULL OR PublishEndDate IS NULL OR (GETDATE() BETWEEN PublishStartDate AND PublishEndDate))' + CHAR(13) + CHAR(10)
	
		IF(@NotIn IS NOT NULL AND @NotIn <> '')
		BEGIN
			SET @WITH = @WITH + ' AND [Item].[ItemId] NOT IN(' + @NotIn + ')'
		END
		
		IF(ISNULL(@SiteId, 0) > 0)
			SET @WITH = @WITH + ' AND Item.SiteId = ' + CAST(@SiteId AS NVARCHAR) + CHAR(13) + CHAR(10)
		ELSE IF(ISNULL(@StateId, 0) > 0)
			SET @WITH = @WITH + ' AND [Site].StateId = ' + CAST(@StateId AS NVARCHAR) + CHAR(13) + CHAR(10)
		ELSE IF(ISNULL(@RegionId, 0) > 0)
			SET @WITH = @WITH + ' AND [State].RegionId = ' + CAST(@RegionId AS NVARCHAR) + CHAR(13) + CHAR(10)
		
		IF(ISNULL(@CategoryId, 0) > 0)
			SET @WITH = @WITH + ' AND [Item].[CategoryId] = ' + CAST(@CategoryId AS NVARCHAR) + CHAR(13) + CHAR(10)
		--IF(ISNULL(@TimeSlot, 0) > 0)
		--	SET @WITH = @WITH + ' AND [TeeSheet].[FromTime] = ''' + CAST(@TimeSlot AS NVARCHAR) + ':00'''
			
		IF(@IncludePractice IS NOT NULL)
			SET @WITH = @WITH + ' AND ISNULL([Item].[IncludePractice], 0) = ' + CAST(@IncludePractice AS NVARCHAR)
	 	
	 	IF(LEN(@WHERE) > 0)
			SET @WITH = @WITH + ' AND ([Item].[ItemCode] LIKE N''%' + @WHERE + '%'' OR [ItemLang].[ItemName] LIKE N''%' + @WHERE + '%'')'
	
	SET @WITH = @WITH + ')' + CHAR(13) + CHAR(10) -- End of WITH Command
	
	SET @SQL = @WITH + 'SELECT '
	
	IF(ISNULL(@PageSize, 0) > 0)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@PageSize AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM ('
	
	SET @SQL = @SQL + 'SELECT ROW_NUMBER() OVER (ORDER BY [ItemId] DESC) AS RowNumber, it.* FROM GreenFees it'
	
	SET @SQL = @SQL + ') AS it '
	
	SET @WHERE = ' WHERE 1 = 1'
	
	IF((@PageSize IS NOT NULL AND @PageSize > 0) AND (@PageIndex IS NOT NULL AND @PageIndex > 0))
	BEGIN
		SET @WHERE = @WHERE + ' AND RowNumber >= ' + CAST((@PageSize * (@PageIndex - 1)) AS NVARCHAR)
	END	
	
	IF((@PageSize IS NOT NULL AND @PageSize > 0) AND (@PageIndex IS NOT NULL AND @PageIndex > 0))
		SET @ORDER = ' ORDER BY RowNumber;' + CHAR(13) + CHAR(13)
	ELSE
		SET @ORDER = ' ORDER BY NEWID();' + CHAR(13) + CHAR(13)
		
	SET @SQL = @SQL + @ORDER
	
	EXEC SP_EXECUTESQL @SQL
	--PRINT @SQL
	
	IF(@PageSize IS NOT NULL AND @PageSize > 0)
		SET @SQL = @WITH + 'SELECT (COUNT(*) / ' + CAST(@PageSize AS NVARCHAR) + ') + 1 AS ''TotalPage'' FROM GreenFees '
	ELSE
		SET @SQL = 'SELECT 1 AS TotalPage'

	EXEC SP_EXECUTESQL @SQL
	--PRINT @SQL
	
	RETURN
END

GO
/****** Object:  StoredProcedure [dbo].[SelectBrandsList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectBrandsList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectBrandsList] AS' 
END
GO
ALTER PROCEDURE [dbo].[SelectBrandsList]
(
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL
)
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('
	SET @SQL = @SQL + 'SELECT ROW_NUMBER() OVER (ORDER BY BrandName) AS RowNumber, * FROM [Brand] '
	SET @SQL = @SQL + ' WHERE Active = 1'
	SET @SQL = @SQL + ') AS it'
	SET @SQL = @SQL + ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'AND BrandName LIKE ''%' + @Search + '%'''
	END
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND it.RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'ORDER BY ' + @Order
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM Brand WHERE Active = 1'
	
	exec sp_executesql @SQL
END

GO
/****** Object:  StoredProcedure [dbo].[SelectCategoriesList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCategoriesList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectCategoriesList] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Categories List
-- =============================================
ALTER PROCEDURE [dbo].[SelectCategoriesList]
	@SiteId INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT * FROM [Category] WHERE Active = 1'

	IF(@SiteId IS NOT NULL)
		SET @SQL = @SQL + ' AND [SiteId] = ' + CAST(@SiteId AS NVARCHAR)
		
	SET @SQL = @SQL + ' ORDER BY CategoryName'
	
	exec sp_executesql @SQL
END


GO
/****** Object:  StoredProcedure [dbo].[SelectCategory]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCategory]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectCategory] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single User Type
-- =============================================
ALTER PROCEDURE [dbo].[SelectCategory]
	@CategoryId Int = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM [Category] WHERE [CategoryId] = @CategoryId
END


GO
/****** Object:  StoredProcedure [dbo].[SelectCountriesList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCountriesList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectCountriesList] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Country List
-- =============================================
ALTER PROCEDURE [dbo].[SelectCountriesList]
	@WHERE NVARCHAR(255) = '',
	@Total INT = NULL,
	@Start INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT '
		
	IF(ISNULL(@Total, 0) > 0)
		SET @SQL = @SQL + ' TOP(' + CAST(@Total AS NVARCHAR) + ')'
		
	SET @SQL = @SQL + ' it.* FROM (SELECT ROW_NUMBER() OVER (ORDER BY CountryName) AS RowNumber, * FROM [Country] '
	IF(LEN(@WHERE) > 0)
		SET @SQL = @SQL + ' WHERE Active = 1 AND [CountryName] LIKE ''%' + @WHERE + '%'''
		
	IF(ISNULL(@Start, 0) > 0)
		SET @SQL = @SQL + ' AND RowNumber >= ' + CAST(@Start AS NVARCHAR)
		
	SET @SQL = @SQL + ') AS it ORDER BY it.CountryName'
	
	exec sp_executesql @SQL
END


GO
/****** Object:  StoredProcedure [dbo].[SelectCoupon]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCoupon]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectCoupon] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-02-20
-- Description:	Select Single Coupon
-- =============================================
ALTER PROCEDURE [dbo].[SelectCoupon]
	@CouponId Int = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [Coupon].* FROM [Coupon]
	WHERE [CouponId] = @CouponId
END


GO
/****** Object:  StoredProcedure [dbo].[SelectCouponGroupsList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCouponGroupsList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectCouponGroupsList] AS' 
END
GO
ALTER PROCEDURE [dbo].[SelectCouponGroupsList]
(
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL
)
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('
	SET @SQL = @SQL + 'SELECT ROW_NUMBER() OVER (ORDER BY CouponGroupName) AS RowNumber, [CouponGroup].*, CAST(c1.CouponsUsed AS NVARCHAR) + ''/'' + CAST(c1.TotalCoupons AS NVARCHAR) AS CouponsCount'
	SET @SQL = @SQL + ' FROM [CouponGroup] '
	SET @SQL = @SQL + ' LEFT JOIN (SELECT COUNT([Order].[OrderId]) AS CouponsUsed, COUNT([Coupon].[CouponId]) AS TotalCoupons, [CouponGroupId] FROM [Coupon]
						LEFT JOIN [Order] ON [Order].[CouponId] = [Coupon].[CouponId]
						GROUP BY CouponGroupId) c1 ON c1.CouponGroupId = [CouponGroup].[CouponGroupId]'
	SET @SQL = @SQL + ' WHERE Active = 1'
	SET @SQL = @SQL + ') AS it'
	SET @SQL = @SQL + ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'AND CouponGroupName LIKE ''%' + @Search + '%'''
	END
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND it.RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'ORDER BY ' + @Order
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM CouponGroup WHERE Active = 1'
	
	exec sp_executesql @SQL
END
GO
/****** Object:  StoredProcedure [dbo].[SelectCouponsList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCouponsList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectCouponsList] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2015-02-20
-- Description:	Select Coupons List
-- =============================================
ALTER PROCEDURE [dbo].[SelectCouponsList]
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT ROW_NUMBER() OVER (ORDER BY CouponName) AS RowNumber, * FROM [Coupon] '
	SET @SQL = @SQL + ' WHERE Active = 1'
	SET @SQL = @SQL + '  ORDER BY CouponCode'
	
	exec sp_executesql @SQL
END


GO
/****** Object:  StoredProcedure [dbo].[SelectCourse]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCourse]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectCourse] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single Course
-- =============================================
ALTER PROCEDURE [dbo].[SelectCourse]
	@CourseId Int = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [Course].*, [CourseType].[CourseTypeName], [SiteLang].[SiteName] FROM [Course]
	LEFT JOIN [CourseType] ON [CourseType].[CourseTypeId] = [Course].[CourseTypeId]
	LEFT JOIN [Site] ON [Site].[SiteId] = [Course].[SiteId]
	LEFT JOIN [SiteLang] ON [SiteLang].[SiteId] = [Site].[SiteId]
	WHERE [CourseId] = @CourseId
END


GO
/****** Object:  StoredProcedure [dbo].[SelectCoursesList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCoursesList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectCoursesList] AS' 
END
GO
ALTER PROCEDURE [dbo].[SelectCoursesList]
(
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL
)
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('
	SET @SQL = @SQL + 'SELECT ROW_NUMBER() OVER (ORDER BY CourseName) AS RowNumber, * FROM [Course] '
	SET @SQL = @SQL + ' WHERE Active = 1'
	SET @SQL = @SQL + ') AS it'
	SET @SQL = @SQL + ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'AND CourseName LIKE ''%' + @Search + '%'''
	END
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND it.RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'ORDER BY ' + @Order
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM Course WHERE Active = 1'
	
	exec sp_executesql @SQL
END

GO
/****** Object:  StoredProcedure [dbo].[SelectCourseType]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCourseType]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectCourseType] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single Course Type
-- =============================================
ALTER PROCEDURE [dbo].[SelectCourseType]
	@CourseTypeId Int = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [CourseType].* FROM [CourseType]
	WHERE [CourseTypeId] = @CourseTypeId
END


GO
/****** Object:  StoredProcedure [dbo].[SelectCourseTypesList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCourseTypesList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectCourseTypesList] AS' 
END
GO
ALTER PROCEDURE [dbo].[SelectCourseTypesList]
(
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL
)
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('
	SET @SQL = @SQL + 'SELECT ROW_NUMBER() OVER (ORDER BY CourseTypeName) AS RowNumber, * FROM [CourseType] '
	SET @SQL = @SQL + ' WHERE Active = 1'
	SET @SQL = @SQL + ') AS it'
	SET @SQL = @SQL + ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'AND CourseTypeName LIKE ''%' + @Search + '%'''
	END
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND it.RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'ORDER BY ' + @Order
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM CourseType WHERE Active = 1'
	
	exec sp_executesql @SQL
END

GO
/****** Object:  StoredProcedure [dbo].[SelectCustomerGroup]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCustomerGroup]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectCustomerGroup] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single User Type
-- =============================================
ALTER PROCEDURE [dbo].[SelectCustomerGroup]
	@CustomerGroupId Int = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM [CustomerGroup] WHERE [CustomerGroupId] = @CustomerGroupId
END


GO
/****** Object:  StoredProcedure [dbo].[SelectCustomerGroupsList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCustomerGroupsList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectCustomerGroupsList] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single User Type
-- =============================================
ALTER PROCEDURE [dbo].[SelectCustomerGroupsList]
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;
	
	SET @SQL = 'SELECT ROW_NUMBER() OVER (ORDER BY CustomerGroupName) AS RowNumber, * FROM [CustomerGroup] '	
	SET @SQL = @SQL + ' ORDER BY CustomerGroupName'
	
	exec sp_executesql @SQL
END


GO
/****** Object:  StoredProcedure [dbo].[SelectCustomersList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectCustomersList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectCustomersList] AS' 
END
GO
ALTER PROCEDURE [dbo].[SelectCustomersList]
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL,
	@SiteId INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;
	
	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('
	SET @SQL = @SQL + 'SELECT ROW_NUMBER() OVER (ORDER BY Email) AS RowNumber, * FROM [Users] '
	SET @SQL = @SQL + ' WHERE UserTypeId = 2 AND Active = 1'	

	IF(@SiteId IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND [SiteId] = ' + CAST(@SiteId AS NVARCHAR)
	END
		
	SET @SQL = @SQL + ') AS it'
	SET @SQL = @SQL + ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'AND (Email LIKE ''%' + @Search + '%'' OR Firstname LIKE ''%' + @Search + '%'' OR Lastname LIKE ''%' + @Search + '%'' OR Middlename LIKE ''%' + @Search + '%'')'
	END
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND it.RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'ORDER BY ' + @Order
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM Users WHERE UserTypeId = 2 AND Active = 1'
	
	exec sp_executesql @SQL
	
END

GO
/****** Object:  StoredProcedure [dbo].[SelectDrivingRange]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectDrivingRange]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectDrivingRange] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single Item
-- =============================================
ALTER PROCEDURE [dbo].[SelectDrivingRange]
	@ItemId Int = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [Item].*, [ItemCategory].[CategoryName] FROM [Item]
	LEFT JOIN [ItemCategory] ON [ItemCategory].[CategoryId] = [Item].[CategoryId]
	WHERE [ItemId] = @ItemId
	
	SELECT [ItemLang].* FROM [ItemLang] WHERE [ItemLang].[ItemId] = @ItemId ORDER BY [ItemLang].[LangId]
	
	SELECT TOP 1 [ItemPosition].* FROM [ItemPosition] WHERE [ItemPosition].[ItemId] = @ItemId
END


GO
/****** Object:  StoredProcedure [dbo].[SelectDrivingRangesList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectDrivingRangesList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectDrivingRangesList] AS' 
END
GO
ALTER PROCEDURE [dbo].[SelectDrivingRangesList]
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL,
	@LangId INT = 1,
	@SiteId INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;	

	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('
	SET @SQL = @SQL + ' SELECT ROW_NUMBER() OVER (ORDER BY ItemCode) AS RowNumber, [Item].*, [ItemLang].[ItemName], [ItemCategory].[CategoryName], [SiteLang].[SiteName] FROM [Item] '
	SET @SQL = @SQL + ' LEFT JOIN [ItemLang] ON [ItemLang].[ItemId] = [Item].[ItemId] AND [ItemLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) 
	SET @SQL = @SQL + ' LEFT JOIN [ItemCategory] ON [ItemCategory].[CategoryId] = [Item].[CategoryId]'
	SET @SQL = @SQL + ' LEFT JOIN [SiteLang] ON [SiteLang].[SiteId] = [Item].[SiteId] AND [SiteLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) 
	SET @SQL = @SQL + ' WHERE [Item].[Active] = 1 AND [Item].[ItemTypeId] = 5 '
	
	IF(@SiteId IS NOT NULL)
	SET @SQL = @SQL + ' AND  [Item].[SiteId] = ' + CAST(@SiteId AS NVARCHAR)
	
	SET @SQL = @SQL + ') AS it'
	SET @SQL = @SQL + ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'AND (it.ItemCode LIKE ''%' + @Search + '%'' OR it.ItemName LIKE ''%' + @Search + '%'' OR it.SiteName LIKE ''%' + @Search + '%'')'
	END
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND it.RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'ORDER BY ' + @Order
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM Item WHERE Active = 1 AND ItemTypeId = 5'
	
	exec sp_executesql @SQL
END

SET ANSI_NULLS ON

GO
/****** Object:  StoredProcedure [dbo].[SelectEmailingsList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectEmailingsList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectEmailingsList] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Categories List
-- =============================================
ALTER PROCEDURE [dbo].[SelectEmailingsList]
	@WHERE NVARCHAR(255) = '',
	@Total INT = NULL,
	@Start INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT '
		
	IF(ISNULL(@Total, 0) > 0)
		SET @SQL = @SQL + ' TOP(' + CAST(@Total AS NVARCHAR) + ')'
		
	SET @SQL = @SQL + ' it.* FROM (SELECT ROW_NUMBER() OVER (ORDER BY EmailName) AS RowNumber, * FROM [Emailing] '
	SET @SQL = @SQL + ' WHERE Active = 1'
	IF(LEN(@WHERE) > 0)
		SET @SQL = @SQL + ' AND ([EmailName] LIKE ''%' + @WHERE + '%'' OR [Subject] LIKE ''%' + @WHERE + '%'' OR [FromName] LIKE ''%' + @WHERE + '%'' OR [FromEmail] LIKE ''%' + @WHERE + '%'')'
		
	IF(ISNULL(@Start, 0) > 0)
		SET @SQL = @SQL + ' AND RowNumber >= ' + CAST(@Start AS NVARCHAR)
		
	SET @SQL = @SQL + ') AS it ORDER BY it.EmailName'
	
	exec sp_executesql @SQL
END


GO
/****** Object:  StoredProcedure [dbo].[SelectEmailTemplateList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectEmailTemplateList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectEmailTemplateList] AS' 
END
GO
ALTER PROCEDURE [dbo].[SelectEmailTemplateList]
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL,
	@LangId NVARCHAR(MAX) = 1
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.TemplateId, it.Name FROM('
	SET @SQL = @SQL + 'SELECT ROW_NUMBER() OVER (ORDER BY Name) AS RowNumber, [EmailTemplate].[TemplateId], [EmailTemplateLang].[Name] AS [Name] FROM [EmailTemplate] '
	SET @SQL = @SQL + ' LEFT JOIN [EmailTemplateLang] ON [EmailTemplateLang].[TemplateId] = [EmailTemplate].[TemplateId] AND [EmailTemplateLang].[LangId] = ' + CAST(@LangId AS NVARCHAR)
	SET @SQL = @SQL + ' WHERE Active = 1'
	SET @SQL = @SQL + ') AS it'
	SET @SQL = @SQL + ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'AND Name LIKE ''%' + @Search + '%'''
	END
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND it.RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'ORDER BY ' + @Order
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM EmailTemplate WHERE Active = 1'
	
	exec sp_executesql @SQL
END
GO
/****** Object:  StoredProcedure [dbo].[SelectGolfBrandsList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectGolfBrandsList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectGolfBrandsList] AS' 
END
GO


-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-11-24
-- Description:	Select GolfBrands List
-- =============================================
ALTER PROCEDURE [dbo].[SelectGolfBrandsList]
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT ROW_NUMBER() OVER (ORDER BY GolfBrandName) AS RowNumber, * FROM [GolfBrand] '
	SET @SQL = @SQL + ' WHERE Active = 1'
	SET @SQL = @SQL + '  ORDER BY GolfBrandName'
	
	exec sp_executesql @SQL
END



GO
/****** Object:  StoredProcedure [dbo].[SelectGolfLessonCategoriesList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectGolfLessonCategoriesList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectGolfLessonCategoriesList] AS' 
END
GO
-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select GolfLesson Categories List
-- =============================================
ALTER PROCEDURE [dbo].[SelectGolfLessonCategoriesList]
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL,
	@SiteId INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('

	SET @SQL = @SQL + ' SELECT ROW_NUMBER() OVER (ORDER BY CategoryName) AS RowNumber, * FROM [ItemCategory] '
	SET @SQL = @SQL + ' WHERE [ItemTypeId] = 4 AND Active = 1 '	
	
	IF(@SiteId IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND [SiteId] =  ' +	 CAST(@SiteId AS NVARCHAR)
	END
		
	SET @SQL = @SQL + ') AS it'
	SET @SQL = @SQL + ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'AND CategoryName LIKE ''%' + @Search + '%'''
	END
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND it.RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'ORDER BY ' + @Order
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM ItemCategory WHERE Active = 1 AND ItemTypeId = 4'
	
	exec sp_executesql @SQL
END

GO
/****** Object:  StoredProcedure [dbo].[SelectGolfLessonsList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectGolfLessonsList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectGolfLessonsList] AS' 
END
GO
-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Items List
-- =============================================
ALTER PROCEDURE [dbo].[SelectGolfLessonsList]
	@LangId INT = 1,
	@SiteId INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT ROW_NUMBER() OVER (ORDER BY ItemCode) AS RowNumber, [Item].*, [ItemLang].[ItemName], [ItemCategory].[CategoryName], [SiteLang].[SiteName] FROM [Item] '
	SET @SQL = @SQL + ' LEFT JOIN [ItemLang] ON [ItemLang].[ItemId] = [Item].[ItemId] AND [ItemLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) 
	SET @SQL = @SQL + ' LEFT JOIN [ItemCategory] ON [ItemCategory].[CategoryId] = [Item].[CategoryId]'
	SET @SQL = @SQL + ' LEFT JOIN [SiteLang] ON [SiteLang].[SiteId] = [Item].[SiteId] AND [SiteLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) 
	SET @SQL = @SQL + ' WHERE [Item].[Active] = 1 AND [Item].[ItemTypeId] = 4 '	
	
	IF (@SiteId IS NOT NULL)
		SET @SQL = @SQL + ' AND [Item].[SiteId] = ' + CAST(@SiteId AS NVARCHAR)
		
	SET @SQL = @SQL + ' ORDER BY ItemCode'
	
	exec sp_executesql @SQL
END


/****** Object:  StoredProcedure [dbo].[SelectGreenFeesList]    Script Date: 03/22/2015 10:56:34 ******/
SET ANSI_NULLS ON

GO
/****** Object:  StoredProcedure [dbo].[SelectGreenFeeCategoriesList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectGreenFeeCategoriesList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectGreenFeeCategoriesList] AS' 
END
GO
-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2015-06-25
-- Description:	Select Green Fee Categories List
-- =============================================
ALTER PROCEDURE [dbo].[SelectGreenFeeCategoriesList]
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL,
	@SiteId INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('

	SET @SQL = @SQL + ' SELECT ROW_NUMBER() OVER (ORDER BY CategoryName) AS RowNumber, * FROM [ItemCategory] '
	SET @SQL = @SQL + ' WHERE [ItemTypeId] = 2 AND Active = 1 '	
	
	IF(@SiteId IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND [SiteId] =  ' +	 CAST(@SiteId AS NVARCHAR)
	END
		
	SET @SQL = @SQL + ') AS it'
	SET @SQL = @SQL + ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'AND CategoryName LIKE ''%' + @Search + '%'''
	END
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND it.RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'ORDER BY ' + @Order
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM ItemCategory WHERE Active = 1 AND ItemTypeId = 2'
	
	exec sp_executesql @SQL
END

GO
/****** Object:  StoredProcedure [dbo].[SelectGreenFeesList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectGreenFeesList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectGreenFeesList] AS' 
END
GO
-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Items List
-- =============================================
ALTER PROCEDURE [dbo].[SelectGreenFeesList]
	@LangId INT = 1,
	@SiteId INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT ROW_NUMBER() OVER (ORDER BY ItemCode) AS RowNumber, [Item].*, [ItemLang].[ItemName], [ItemCategory].[CategoryName], [SiteLang].[SiteName] FROM [Item] '
	SET @SQL = @SQL + ' LEFT JOIN [ItemLang] ON [ItemLang].[ItemId] = [Item].[ItemId] AND [ItemLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) 
	SET @SQL = @SQL + ' LEFT JOIN [ItemCategory] ON [ItemCategory].[CategoryId] = [Item].[CategoryId]'
	SET @SQL = @SQL + ' LEFT JOIN [SiteLang] ON [SiteLang].[SiteId] = [Item].[SiteId] AND [SiteLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) 
	SET @SQL = @SQL + ' WHERE [Item].[Active] = 1 AND [Item].[ItemTypeId] = 2 '	
	
	IF(@SiteId IS NOT NULL)
	SET @SQL = @SQL + ' AND [Item].[SiteId] = ' + CAST(@SiteId AS NVARCHAR)
	
	SET @SQL = @SQL + ' ORDER BY ItemCode'
	
	exec sp_executesql @SQL
END

/****** Object:  StoredProcedure [dbo].[SelectDrivingRange]    Script Date: 03/22/2015 10:56:24 ******/
SET ANSI_NULLS ON

GO
/****** Object:  StoredProcedure [dbo].[SelectImpressum]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectImpressum]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectImpressum] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single Impressum
-- =============================================
ALTER PROCEDURE [dbo].[SelectImpressum]
	@ImpressumId Int = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM [Impressum] WHERE [ImpressumId] = @ImpressumId
END


GO
/****** Object:  StoredProcedure [dbo].[SelectImpressumsList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectImpressumsList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectImpressumsList] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Impressums List
-- =============================================
ALTER PROCEDURE [dbo].[SelectImpressumsList]
	@SiteId INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT * FROM [Impressum] WHERE 1 = 1'

	IF(@SiteId IS NOT NULL)
		SET @SQL = @SQL + ' AND [SiteId] = ' + CAST(@SiteId AS NVARCHAR)
		
	SET @SQL = @SQL + ' ORDER BY Name'
	
	exec sp_executesql @SQL
END


GO
/****** Object:  StoredProcedure [dbo].[SelectInterest]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectInterest]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectInterest] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single Interest
-- =============================================
ALTER PROCEDURE [dbo].[SelectInterest]
	@InterestId Int = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [Interest].* FROM [Interest]
	WHERE [InterestId] = @InterestId
	
	SELECT [InterestLang].* FROM [InterestLang] WHERE [InterestLang].[InterestId] = @InterestId ORDER BY [InterestLang].[LangId]
END


GO
/****** Object:  StoredProcedure [dbo].[SelectInterestsList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectInterestsList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectInterestsList] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Sites List
-- =============================================
ALTER PROCEDURE [dbo].[SelectInterestsList]
	@LangId INT = 1
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;
		
	SET @SQL = 'SELECT ROW_NUMBER() OVER (ORDER BY [InterestName]) AS RowNumber, [Interest].*, [InterestLang].[InterestName] AS InterestName, [InterestLang].[InterestDesc] AS InterestDesc, [ItemCategory].[CategoryName] AS [CategoryName] FROM [Interest] '
	SET @SQL = @SQL + ' LEFT JOIN [InterestLang] ON [InterestLang].[InterestId] = [Interest].[InterestId] AND [InterestLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) 
	SET @SQL = @SQL + ' LEFT JOIN [ItemCategory] ON [ItemCategory].[CategoryId] = [Interest].[CategoryId] '
	SET @SQL = @SQL + ' WHERE [Interest].[Active] = 1'
	SET @SQL = @SQL + ' ORDER BY InterestName'
	
	exec sp_executesql @SQL
	
END


GO
/****** Object:  StoredProcedure [dbo].[SelectItem]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectItem]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectItem] AS' 
END
GO
ALTER PROCEDURE [dbo].[SelectItem]
	@ItemId INT = 0,
	@LangId INT = 1
AS
BEGIN
	SET NOCOUNT ON;

	WITH PeriodPrices
    AS
    (
        SELECT [ItemId], MIN([Price]) AS [Price]
        FROM [ItemPrice] p1
        WHERE 
        --GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate]) AND 
        [PriceType] = 0
        GROUP BY [ItemId]
    ),
    SpecialPrices
    AS
    (
        SELECT [ItemId], MIN([Price]) AS [Price]
        FROM [ItemPrice] p1
        WHERE GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate]) AND [PriceType] = 0
        GROUP BY [ItemId]
    ),
	MaxDatePricing
	AS
	(
		SELECT [ItemId], MAX([EndDate]) AS [MaxDate]
		FROM [ItemPrice]
		WHERE [Price] > 0 AND DATEADD(SECOND, -1, DATEADD(DAY, 1, [EndDate])) > GETDATE()
		GROUP BY [ItemId]
	)
    SELECT [Item].*, [ItemCategory].[CategoryName], img.[ItemImageId], img.[ListNo], img.[ImageName], img.[BaseName], img.[FileExtension], [Site].[SiteSlug], [SiteLang].[SiteName], [State].[StateName], ISNULL([Tax].[TaxPercent], 0) AS [TaxRate], ISNULL(idp.[Price], 0) AS SpecialPrice, ISNULL(ipp.[Price], 0) AS PeriodPrice, ISNULL(mdp.[MaxDate], GETDATE()) AS [ItemMaxDate], [Site].[AlbatrosCourseId]  FROM [Item]
	LEFT JOIN [ItemCategory] ON [ItemCategory].[CategoryId] = [Item].[CategoryId]
	LEFT JOIN [ItemImage] img ON img.[ItemImageId] = (SELECT TOP(1) [ItemImageId] FROM [ItemImage] WHERE [ItemImage].[ItemId] = [Item].[ItemId] ORDER BY [ItemImage].[ListNo], [ItemImage].[ItemImageId])
    LEFT JOIN [Site] ON [Site].[SiteId] = [Item].[SiteId]
	LEFT JOIN [Tax] ON [Tax].[TaxId] = [Item].[TaxId]
    LEFT JOIN [SiteLang] ON [SiteLang].[SiteId] = [Site].[SiteId] AND ([SiteLang].[LangId] = @LangId OR [SiteLang].[LangId] = 1)
    LEFT JOIN [State] ON [Site].[StateId] = [State].[StateId]
    LEFT JOIN PeriodPrices ipp ON ipp.[ItemId] = [Item].[ItemId]
    LEFT JOIN SpecialPrices idp ON idp.[ItemId] = [Item].[ItemId]
    LEFT JOIN MaxDatePricing mdp ON mdp.[ItemId] = [Item].[ItemId]
	WHERE [Item].[ItemId] = @ItemId
	
	SELECT [ItemLang].* FROM [ItemLang] WHERE [ItemLang].[ItemId] = @ItemId ORDER BY [ItemLang].[LangId]
	
	SELECT [SiteLang].* FROM [SiteLang]
	JOIN [Item] ON [Item].[SiteId] = [SiteLang].[SiteId] WHERE [Item].[ItemId] = @ItemId ORDER BY [SiteLang].[LangId]
	
	SELECT AVG(Rating) AS AverageRating FROM [ItemReview] WHERE [ItemId] = @ItemId
	
	SELECT [ItemReview].*, [Users].[Firstname] AS [ReviewerName], [City].[CityName] AS [ReviewerCityName]
	FROM [ItemReview]
    INNER JOIN [Users] ON [Users].[UserId] = [ItemReview].[UserId]
    INNER JOIN [City] ON [City].[CityId] = [Users].[CityId] 
    WHERE [ItemId] = @ItemId ORDER BY [UpdatedDate] DESC
	
	SELECT * FROM [Course]
	INNER JOIN [Item] ON [Item].[CourseId] = [Course].[CourseId] AND [Item].[ItemId] = @ItemId
	
	SELECT * FROM [ItemPrice] WHERE [ItemId] = @ItemId ORDER BY [ItemId], [StartDate], [EndDate]
END

GO
/****** Object:  StoredProcedure [dbo].[SelectItemById]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectItemById]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectItemById] AS' 
END
GO

ALTER PROCEDURE [dbo].[SelectItemById]
	@LangId INT,
	@ItemId INT
AS
BEGIN
	SELECT Item.*, ItemLang.ItemName, ItemLang.ItemDesc, ItemLang.Accommodation, ItemLang.TrainingArea, ItemLang.UserId, ItemLang.[LangId], ItemLang.UpdateDate AS ItemLangUpdateDate FROM Item
	LEFT JOIN ItemLang ON ItemLang.ItemId = Item.ItemId AND ItemLang.[LangId] = ISNULL(@LangId, 0)
	WHERE Item.ItemId = ISNULL(@ItemId, 0)
	
	SELECT * FROM [ItemPrice] WHERE [ItemId] = @ItemId ORDER BY [ItemId], [StartDate], [EndDate]
END


GO
/****** Object:  StoredProcedure [dbo].[SelectItemCategoriesList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectItemCategoriesList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectItemCategoriesList] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Item Categories List
-- =============================================
ALTER PROCEDURE [dbo].[SelectItemCategoriesList]
	@SiteId INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = @SQL + ' SELECT ROW_NUMBER() OVER (ORDER BY CategoryName) AS RowNumber, * FROM [ItemCategory] '
	SET @SQL = @SQL + ' WHERE Active = 1 AND ItemTypeId = 1'	
	
	IF(@SiteId IS NOT NULL)
		SET @SQL = @SQL + ' AND [SiteId] =  ' +	 CAST(@SiteId AS NVARCHAR)
	SET @SQL = @SQL + ' ORDER BY CategoryName'
	
	exec sp_executesql @SQL
END


GO
/****** Object:  StoredProcedure [dbo].[SelectItemCategory]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectItemCategory]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectItemCategory] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single Item Category
-- =============================================
ALTER PROCEDURE [dbo].[SelectItemCategory]
	@CategoryId Int = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [ItemCategory].* FROM [ItemCategory]
	WHERE [CategoryId] = @CategoryId
END


GO
/****** Object:  StoredProcedure [dbo].[SelectItemReviewsList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectItemReviewsList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectItemReviewsList] AS' 
END
GO
ALTER PROCEDURE [dbo].[SelectItemReviewsList]
(
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL,
	@LangId INT = 1
)
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = '',
	@WHERE NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('
	SET @SQL = @SQL + 'SELECT ROW_NUMBER() OVER (ORDER BY ItemReviewId DESC) AS RowNumber, [ItemReview].*, [Users].[Firstname] + '' '' + [Users].[Lastname] AS ReviewerName, [City].[CityName] AS ReviewerCityName, [Item].[ItemSlug], [ItemLang].[ItemName] AS ItemName FROM [ItemReview] '
					+ ' INNER JOIN [Users] ON [Users].[UserId] = [ItemReview].[UserId]'
					+ ' LEFT JOIN [City] ON [City].[CityId] = [Users].[CityId]'
					+ ' INNER JOIN [Item] ON [Item].[ItemId] = [ItemReview].[ItemId]'
					+ ' LEFT JOIN [ItemLang] ON [ItemLang].[ItemId] = [Item].[ItemId] AND [ItemLang].[LangId] = ' + CAST(@LangId AS NVARCHAR)
	--SET @SQL = @SQL + ' WHERE Active = 1'
	SET @SQL = @SQL + ') AS it'
	SET @WHERE = ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @WHERE = @WHERE + ' AND (Subject LIKE ''%' + @Search + '%'' OR Message LIKE ''%' + @Search + '%'')'
	END

	SET @SQL = @SQL + @WHERE

	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' ORDER BY ' + @Order
	END
	ELSE
	BEGIN
		SET @SQL = @SQL + ' ORDER BY [ItemReviewId] DESC'
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM [ItemReview]'
	+ ' INNER JOIN [Users] ON [Users].[UserId] = [ItemReview].[UserId]'
	+ ' INNER JOIN [Item] ON [Item].[ItemId] = [ItemReview].[ItemId]'
	
	EXEC sp_executesql @SQL
	--PRINT @SQL
END

GO
/****** Object:  StoredProcedure [dbo].[SelectItemsList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectItemsList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectItemsList] AS' 
END
GO
ALTER PROCEDURE [dbo].[SelectItemsList]
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL,
	@ItemTypeId INT = 1,
	@LangId INT = 1,
	@SiteId INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;	
	
	IF(@ItemTypeId IS NULL)
		SET @ItemTypeId = 1

	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('
	SET @SQL = @SQL + ' SELECT ROW_NUMBER() OVER (ORDER BY ItemCode) AS RowNumber, [Item].*, [ItemLang].[ItemName], [ItemCategory].[CategoryName], [SiteLang].[SiteName] FROM [Item] '
	SET @SQL = @SQL + ' LEFT JOIN [ItemLang] ON [ItemLang].[ItemId] = [Item].[ItemId] AND [ItemLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) 
	SET @SQL = @SQL + ' LEFT JOIN [ItemCategory] ON [ItemCategory].[CategoryId] = [Item].[CategoryId]'
	SET @SQL = @SQL + ' LEFT JOIN [SiteLang] ON [SiteLang].[SiteId] = [Item].[SiteId] AND [SiteLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) 
	SET @SQL = @SQL + ' WHERE [Item].[Active] = 1 AND [Item].[ItemTypeId] = ' + CAST(@ItemTypeId AS NVARCHAR)
	
	IF(@SiteId IS NOT NULL)
	SET @SQL = @SQL + ' AND  [Item].[SiteId] = ' + CAST(@SiteId AS NVARCHAR)
	
	SET @SQL = @SQL + ') AS it'
	SET @SQL = @SQL + ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'AND (it.ItemCode LIKE ''%' + @Search + '%'' OR it.ItemName LIKE ''%' + @Search + '%'' OR it.SiteName LIKE ''%' + @Search + '%'')'
	END
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND it.RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'ORDER BY ' + @Order
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM Item WHERE Active = 1 AND ItemTypeId = ' + CAST(@ItemTypeId AS NVARCHAR)
	
	exec sp_executesql @SQL
END

SET ANSI_NULLS ON

GO
/****** Object:  StoredProcedure [dbo].[SelectItemType]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectItemType]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectItemType] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single Item Type
-- =============================================
ALTER PROCEDURE [dbo].[SelectItemType]
	@ItemTypeId Int = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [ItemTypes].* FROM [ItemTypes]
	WHERE [ItemTypeId] = @ItemTypeId
END


GO
/****** Object:  StoredProcedure [dbo].[SelectItemTypesList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectItemTypesList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectItemTypesList] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Item Types List
-- =============================================
ALTER PROCEDURE [dbo].[SelectItemTypesList]
	@WHERE NVARCHAR(255) = '',
	@Total INT = NULL,
	@Start INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT '
		
	IF(ISNULL(@Total, 0) > 0)
		SET @SQL = @SQL + ' TOP(' + CAST(@Total AS NVARCHAR) + ')'
		
	SET @SQL = @SQL + ' it.* FROM (SELECT ROW_NUMBER() OVER (ORDER BY ItemTypeName) AS RowNumber, * FROM [ItemType] '
	
	IF(LEN(@WHERE) > 0)
		SET @SQL = @SQL + ' WHERE ([ItemTypeName] LIKE ''%' + @WHERE + '%'')'
		
	IF(ISNULL(@Start, 0) > 0)
		SET @SQL = @SQL + ' AND RowNumber >= ' + CAST(@Start AS NVARCHAR)
		
	SET @SQL = @SQL + ') AS it ORDER BY it.ItemTypeName'
	
	exec sp_executesql @SQL
END


GO
/****** Object:  StoredProcedure [dbo].[SelectLatestDLGItems]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectLatestDLGItems]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectLatestDLGItems] AS' 
END
GO

ALTER PROCEDURE [dbo].[SelectLatestDLGItems]
	@PageSize INT = NULL,
	@PageIndex INT = NULL,
	@Level INT = NULL,
	@Shape NVARCHAR(100) = NULL,
	@Genre NVARCHAR(100) = NULL,
	@Dexterity INT = NULL,
	@Shaft INT = NULL,
	@SkipId INT = NULL,
	@SearchText NVARCHAR(255) = NULL,
	@CategoryId INT = NULL,
	@SubCategoryId INT = NULL,
	@LangId INT = 1,
	@BrandNameId INT = NULL,
	@SiteId INT = NULL
AS
BEGIN
	IF(@LangId IS NULL)
		SET @LangId = 1
	
	DECLARE @SQL NVARCHAR(MAX)
	SET @SQL = 'WITH PeriodPrices
				AS
				(
					SELECT [ItemId], [Price] AS [Price]
					FROM [ItemPrice]
					WHERE 
					GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate]) AND 
					[PriceType] = 0 AND [Price] > 0
				),
				CheapestPeriodPrices
				AS
				(
					SELECT [ItemId], MIN([Price]) AS [Price]
					FROM [ItemPrice]
					WHERE [PriceType] = 0 AND [Price] > 0 AND [EndDate] > GETDATE()
					GROUP BY [ItemId]
				),
				SpecialPrices
				AS
				(
					SELECT [ItemId], MIN([Price]) AS [Price]
					FROM [ItemPrice]
					WHERE 
					GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate]) AND 
					[PriceType] = 1 AND [Price] > 0
					GROUP BY [ItemId]
				)
				SELECT '
	IF(@PageSize > 0)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@PageSize AS NVARCHAR) + ') '
	END
	
	SET @SQL = @SQL +' it.* FROM 
	(SELECT ROW_NUMBER() OVER(ORDER BY [Item].[ItemId] DESC) AS RowNumber, [Item].*, [ItemLang].[ItemName], [ItemLang].[ItemDesc], [ItemLang].[ItemShortDesc], [ItemLang].[TrainingArea], [ItemLang].[Accommodation], img.[ItemImageId], img.[ListNo], img.[ImageName], img.[BaseName], img.[FileExtension], [SiteLang].[SiteName], ISNULL(ipp.[Price], 0) AS PeriodPrice, ISNULL(idp.[Price], 0) AS SpecialPrice
	FROM [Item] 
	LEFT JOIN [ItemCategory] ON [ItemCategory].[CategoryId] = [Item].[CategoryId]
	LEFT JOIN [SiteLang] ON [Item].[SiteId] = [SiteLang].[SiteId] AND [SiteLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) +
	' LEFT JOIN PeriodPrices ipp ON ipp.[ItemId] = [Item].[ItemId]
	  LEFT JOIN SpecialPrices idp ON idp.[ItemId] = [Item].[ItemId]'
	SET @SQL = @SQL + ' LEFT JOIN [ItemLang] ON [ItemLang].[ItemId] = [Item].[ItemId] AND [ItemLang].[LangId] = ' + CAST(@LangId AS NVARCHAR)
	
	SET @SQL = @SQL + ' LEFT JOIN [ItemImage] img ON img.[ItemImageId] = (SELECT TOP(1) [ItemImageId] FROM [ItemImage] WHERE [ItemImage].[ItemId] = [Item].[ItemId] ORDER BY [ItemImage].[ListNo], [ItemImage].[ItemImageId])
	WHERE [Item].[Active] = 1 AND [Item].[ItemTypeId] = 1 '
	
	IF(@SiteId IS NOT NULL AND @SiteId > 0)
	BEGIN
		SET @SQL = @SQL + ' AND [Item].[SiteId] = ' + CAST(@SiteId AS NVARCHAR)
	END
	
	IF(@BrandNameId IS NOT NULL AND @BrandNameId > 0)
	BEGIN
		SET @SQL = @SQL + ' AND [Item].[BrandId] = ' + CAST(@BrandNameId AS NVARCHAR)
		SET @SQL = @SQL + ' AND [Item].[PromotionStatusId] = 1'
	END


	IF(@Shape IS NOT NULL AND LEN(@Shape) > 0)
	BEGIN
		SET @SQL = @SQL + ' AND [Item].[Shape] IN(' + CAST(@Shape AS NVARCHAR) + ')'
	END
	
	IF(@Genre IS NOT NULL AND LEN(@Genre) > 0)
	BEGIN
		SET @SQL = @SQL + ' AND [Item].[Genre] IN(' + CAST(@Genre AS NVARCHAR) + ')'
	END
	
	IF(@Dexterity IS NOT NULL AND @Dexterity > 0)
	BEGIN
		SET @SQL = @SQL + ' AND [Item].[Dexterity] = ' + CAST(@Dexterity AS NVARCHAR)
	END
	
	IF(@Shaft IS NOT NULL AND @Shaft > 0)
	BEGIN
		SET @SQL = @SQL + ' AND [Item].[Shaft] = ' + CAST(@Shaft AS NVARCHAR)
	END
	
	IF(@SubCategoryId IS NOT NULL AND @SubCategoryId > 0)
	BEGIN
		SET @SQL = @SQL + ' AND [Item].[CategoryId] = ' + CAST(@SubCategoryId AS NVARCHAR)
	END
	ELSE IF(@CategoryId IS NOT NULL AND @CategoryId > 0)
	BEGIN
		SET @SQL = @SQL + ' AND [ItemCategory].[ParentCategoryId] = ' + CAST(@CategoryId AS NVARCHAR)
	END
	
	IF(@SearchText IS NOT NULL AND @SearchText <> '')
	BEGIN
		SET @SQL = @SQL + ' AND ([ItemLang].[ItemName] LIKE ''%' + @SearchText + '%'' OR [ItemLang].[ItemDesc] LIKE ''%' + @SearchText + '%'' OR [ItemLang].[ItemShortDesc] LIKE ''%' + @SearchText + '%'')'
	END
	
	SET @SQL = @SQL + ') AS it WHERE 1 = 1 '
	
	IF(@PageSize > 0 AND @PageIndex > 0)
	BEGIN
		SET @SQL = @SQL + ' AND RowNumber >= ' + CAST((@PageSize * (@PageIndex - 1)) AS NVARCHAR)
	END
	
	IF(@SkipId IS NOT NULL AND @SkipId > 0)
	BEGIN
		SET @SQL = @SQL + ' AND it.[ItemId] <> ' + CAST(@SkipId AS NVARCHAR)
	END
	
	SET @SQL = @SQL + ' ORDER BY it.[ItemId] DESC'
	EXEC SP_EXECUTESQL @SQL
	--PRINT @SQL
	
	SELECT CEILING(COUNT(*) / @PageSize) AS 'TotalPage' FROM [Item] WHERE [Active] = 1 AND [ItemTypeId] = 1
	RETURN
END

GO
/****** Object:  StoredProcedure [dbo].[SelectLatestItems]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectLatestItems]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectLatestItems] AS' 
END
GO
ALTER PROCEDURE [dbo].[SelectLatestItems]
	@PageSize INT = NULL,
	@PageIndex INT = NULL,
	@CountryId INT = NULL,
	@RegionId INT = NULL,
	@StateId INT = NULL,
	@SiteId INT = NULL,
	@SkipId INT = NULL,
	@SearchText NVARCHAR(255) = NULL,
	@IncludeAccommodation BIT = NULL,
	@DepartureMonthStart DateTime = NULL,
	@DepartureMonthEnd DateTime = NULL,
	@ItemTypeId INT = NULL,
	@CategoryId INT = NULL,
	@NotIn NVARCHAR(MAX) = NULL,
	@LangId INT = 1
AS
BEGIN
	IF(@LangId IS NULL)
		SET @LangId = 1
	
	DECLARE @SQL NVARCHAR(MAX),
	@SELECT NVARCHAR(MAX),
	@WITH NVARCHAR(MAX),
	@FROM NVARCHAR(MAX),
	@WHERE NVARCHAR(MAX),
	@ORDER NVARCHAR(MAX)
	
	SET @SELECT = ' [Item].[ItemId], [Item].[ItemTypeId], [Item].[ItemSlug], [Item].[Price], [ItemLang].[ItemName], [ItemLang].[ItemShortDesc], img.[ItemImageId], img.[ListNo], img.[ImageName], img.[BaseName], img.[FileExtension], [SiteLang].[SiteName]'
	
	SET @WITH = 'WITH PeriodPrices
				AS
				(
					SELECT [ItemId], [Price] AS [Price]
					FROM [ItemPrice]
					WHERE 
					GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate]) AND 
					[PriceType] = 0 AND [Price] > 0
				),
				CheapestPeriodPrices
				AS
				(
					SELECT [ItemId], MIN([Price]) AS [Price]
					FROM [ItemPrice]
					WHERE [PriceType] = 0 AND DATEADD(SECOND, -1, DATEADD(DAY, 1, [EndDate])) >= GETDATE()
					GROUP BY [ItemId]
				),
				SpecialPrices
				AS
				(
					SELECT [ItemId], MIN([Price]) AS [Price]
					FROM [ItemPrice]
					WHERE 
					GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate]) AND 
					[PriceType] = 1 AND [Price] > 0
					GROUP BY [ItemId]
				),
				TeeSheetCheapestPrice
				AS
				(
					SELECT [ItemId], MIN([Discount]) AS [CheapestPrice] FROM [TeeSheet] WHERE [Price] > 0 AND [Discount] > 0 AND [TeeSheetDate] >= GETDATE() GROUP BY [ItemId]
				)' + CHAR(13)
				
	SET @SQL = @WITH + 'SELECT '
	
	IF(@PageSize IS NOT NULL AND @PageSize > 0)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@PageSize AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM (SELECT DISTINCT '
	
	IF((@PageSize IS NOT NULL AND @PageSize > 0) AND (@PageIndex IS NOT NULL AND @PageIndex > 0))
	BEGIN
		SET @SQL = @SQL + ' ROW_NUMBER() OVER (ORDER BY [Item].[ItemId] DESC) AS RowNumber, '
	END
	
	SET @SQL = @SQL + @SELECT + ', ISNULL(ipp.[Price], 0) AS PeriodPrice, ISNULL(icpp.[Price], 0) AS CheapestPeriodPrice, ISNULL(idp.[Price], 0) AS SpecialPrice, ISNULL(tp.[CheapestPrice], 0) AS TeeSheetCheapestPrice , AVG(Rating) AS AverageRating, ISNULL([Site].[AlbatrosCourseId], 0) AS AlbatrosCourseId'
	SET @FROM = N'FROM [Item] 
	LEFT JOIN [ItemCategory] ON [ItemCategory].[CategoryId] = [Item].[CategoryId]
	LEFT JOIN [SiteLang] ON [Item].[SiteId] = [SiteLang].[SiteId] AND [SiteLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) +
	' LEFT JOIN [Site] ON [Site].[SiteId] = [Item].[SiteId] AND [Site].[Active] = 1 AND [Site].[Visible] = 1
	  LEFT JOIN [State] ON [State].[StateId] = [Site].[StateId]
	  LEFT JOIN [Region] ON [Region].[RegionId] = [State].[RegionId]
	  LEFT JOIN [Country] ON [Country].[CountryId] = [Region].[CountryId]
	  LEFT JOIN [ItemReview] ON [ItemReview].[ItemId] = [Item].[ItemId] AND [ItemReview].[IsApproved] = 1
	  LEFT JOIN PeriodPrices ipp ON ipp.[ItemId] = [Item].[ItemId] AND ipp.[Price] > 0
	  LEFT JOIN CheapestPeriodPrices icpp ON icpp.[ItemId] = [Item].[ItemId] AND icpp.[Price] > 0
	  LEFT JOIN SpecialPrices idp ON idp.[ItemId] = [Item].[ItemId] AND idp.[Price] > 0
	  LEFT JOIN TeeSheetCheapestPrice tp ON tp.[ItemId] = [Item].[ItemId]'
	SET @FROM = @FROM + ' LEFT JOIN [ItemLang] ON [ItemLang].[ItemId] = [Item].[ItemId] AND [ItemLang].[LangId] = ' + CAST(@LangId AS NVARCHAR)
	
	SET @FROM = @FROM + ' LEFT JOIN [ItemImage] img ON img.[ItemImageId] = (SELECT TOP(1) [ItemImageId] FROM [ItemImage] WHERE [ItemImage].[ItemId] = [Item].[ItemId] ORDER BY [ItemImage].[ListNo], [ItemImage].[ItemImageId])'
	SET @WHERE = N'WHERE [Item].[Active] = 1 AND [Item].[IsPublish] = 1 AND [Item].[ItemTypeId] <> 6 AND (ISNULL([PublishStartDate], GETDATE()) <= GETDATE() AND DATEADD(DAY, 1, ISNULL([PublishEndDate], GETDATE())) >= GETDATE())'
	
	IF(@ItemTypeId IS NOT NULL AND @ItemTypeId > 0)
	BEGIN
		SET @WHERE = @WHERE + ' AND [Item].[ItemTypeId] = ' + CAST(@ItemTypeId AS NVARCHAR)
	END
	
	IF(@CategoryId IS NOT NULL AND @CategoryId > 0)
	BEGIN
		SET @WHERE = @WHERE + ' AND [Item].[CategoryId] = ' + CAST(@CategoryId AS NVARCHAR)
	END
	
	IF(@IncludeAccommodation IS NOT NULL)
	BEGIN
		SET @WHERE = @WHERE + ' AND [Item].[IncludeAccommodation] = ' + CAST(@IncludeAccommodation AS NVARCHAR)
	END	
	
	IF(@SiteId IS NOT NULL AND @SiteId > 0)
	BEGIN
		SET @WHERE = @WHERE + ' AND [Item].[SiteId] = ' + CAST(@SiteId AS NVARCHAR)
	END
	ELSE IF(@StateId IS NOT NULL AND @StateId > 0)
	BEGIN
		SET @WHERE = @WHERE + ' AND [Site].[StateId] = ' + CAST(@StateId AS NVARCHAR)
	END
	ELSE IF(@RegionId IS NOT NULL AND @RegionId > 0)
	BEGIN
		SET @WHERE = @WHERE + ' AND [State].[RegionId] = ' + CAST(@RegionId AS NVARCHAR)
	END
	ELSE IF(@CountryId IS NOT NULL AND @CountryId > 0)
	BEGIN
		SET @WHERE = @WHERE + ' AND [Region].[CountryId] = ' + CAST(@CountryId AS NVARCHAR)
	END
	
	IF(@SearchText IS NOT NULL AND @SearchText <> '')
	BEGIN
		SET @WHERE = @WHERE + ' AND ([ItemLang].[ItemName] LIKE ''%' + @SearchText + '%'' OR [ItemLang].[ItemDesc] LIKE ''%' + @SearchText + '%'' OR [ItemLang].[ItemShortDesc] LIKE ''%' + @SearchText + '%'' OR [SiteLang].[SiteName] LIKE ''%' + @SearchText + '%'')'
	END
	
	IF(@DepartureMonthStart IS NOT NULL AND @DepartureMonthEnd IS NOT NULL)
	BEGIN
		SET @WHERE = @WHERE + ' AND ((''' + CONVERT(NVARCHAR, @DepartureMonthStart, 12) + ''' BETWEEN [Item].[PublishStartDate] AND [Item].[PublishEndDate]) OR (''' + CONVERT(NVARCHAR, @DepartureMonthEnd, 12) + ''' BETWEEN [Item].[PublishStartDate] AND [Item].[PublishEndDate]))'
	END
	
	IF(@NotIn IS NOT NULL AND @NotIn <> '')
	BEGIN
		SET @WHERE = @WHERE + ' AND [Item].[ItemId] NOT IN(' + @NotIn + ')'
	END
	
	SET @SQL = @SQL + CHAR(13) + @FROM + CHAR(13) + @WHERE + CHAR(13)
	
	SET @SQL = @SQL + ' GROUP BY ' + @SELECT + ', ipp.[Price], icpp.[Price], idp.[Price], tp.[CheapestPrice], Site.AlbatrosCourseId'
	
	SET @SQL = @SQL + ') AS it WHERE 1 = 1 '
	
	IF((@PageSize IS NOT NULL AND @PageSize > 0) AND (@PageIndex IS NOT NULL AND @PageIndex > 0))
	BEGIN
		SET @SQL = @SQL + ' AND RowNumber >= ' + CAST((@PageSize * (@PageIndex - 1)) AS NVARCHAR)
	END
	
	IF(@SkipId IS NOT NULL AND @SkipId > 0)
	BEGIN
		SET @SQL = @SQL + ' AND it.[ItemId] <> ' + CAST(@SkipId AS NVARCHAR)
	END
	
	IF((@PageSize IS NOT NULL AND @PageSize > 0) AND (@PageIndex IS NOT NULL AND @PageIndex > 0))
		SET @SQL = @SQL + ' ORDER BY RowNumber'
	ELSE
		SET @SQL = @SQL + ' ORDER BY NEWID()'
	
	
	SET @SQL = @SQL + ';'
	
	EXEC SP_EXECUTESQL @SQL
	--PRINT @SQL
	
	IF(@PageSize IS NOT NULL AND @PageSize > 0)
		SET @SQL = @WITH + CHAR(13) + 'SELECT (COUNT(*) / ' + CAST(@PageSize AS NVARCHAR) + ') + 1 AS ''TotalPage'' ' + CHAR(13) + @FROM + CHAR(13)
	ELSE
		SET @SQL = 'SELECT 1 AS TotalPage'

	EXEC SP_EXECUTESQL @SQL
	--PRINT @SQL
	RETURN
END
GO
/****** Object:  StoredProcedure [dbo].[SelectMailingListsList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectMailingListsList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectMailingListsList] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select List of Mailing List
-- =============================================
ALTER PROCEDURE [dbo].[SelectMailingListsList]
	@SiteId INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT* FROM [MailingList] WHERE 1 = 1 '

	IF(@SiteId IS NOT NULL)
		SET @SQL = @SQL + ' SiteId = ' + CAST(@SiteId AS NVARCHAR)
		
	SET @SQL = @SQL + ' ORDER BY MailingListName'
	
	exec sp_executesql @SQL
END


GO
/****** Object:  StoredProcedure [dbo].[SelectModifier]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectModifier]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectModifier] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single Modifier
-- =============================================
ALTER PROCEDURE [dbo].[SelectModifier]
	@ModifierId Int = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [Modifier].* FROM [Modifier]
	WHERE [ModifierId] = @ModifierId
	
	SELECT [ModifierLang].* FROM [ModifierLang]
	WHERE [ModifierId] = @ModifierId
	ORDER BY [ModifierId], [LangId]
END


GO
/****** Object:  StoredProcedure [dbo].[SelectModifiersList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectModifiersList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectModifiersList] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-04-01
-- Description:	Select Modifiers List
-- =============================================
ALTER PROCEDURE [dbo].[SelectModifiersList]
	@LangId INT = 1
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT '
		
	SET @SQL = @SQL + ' it.* FROM (SELECT ROW_NUMBER() OVER (ORDER BY ModifierName) AS RowNumber, [Modifier].*, [ModifierLang].[ModifierName], [ModifierLang].[ModifierDesc] FROM [Modifier] '
	SET @SQL = @SQL + ' LEFT OUTER JOIN [ModifierLang] ON [ModifierLang].[ModifierId] = [Modifier].[ModifierId] AND [ModifierLang].[LangId] = ' + CAST(@LangId AS NVARCHAR)
	
	IF(ISNULL(@LangId, 0) > 0)
		
	SET @SQL = @SQL + ') AS it ORDER BY it.ModifierName'
	
	exec sp_executesql @SQL
END


GO
/****** Object:  StoredProcedure [dbo].[SelectRandomItems]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectRandomItems]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectRandomItems] AS' 
END
GO

ALTER PROCEDURE [dbo].[SelectRandomItems]
	@PageSize INT = NULL,
	@PageIndex INT = NULL,
	@CountryId INT = NULL,
	@SiteId INT = NULL,
	@CourseId INT = NULL,
	@SkipId INT = NULL,
	@SearchText NVARCHAR(255) = NULL,
	@ItemTypeId INT = NULL,
	@CategoryId INT = NULL,
	@LangId INT = 1
AS
BEGIN
	IF(@LangId IS NULL)
		SET @LangId = 1
	
	DECLARE @SQL NVARCHAR(MAX),
	@SELECT NVARCHAR(MAX)
	SET @SQL = 'WITH PeriodPrices
				AS
				(
					SELECT [ItemId], [Price] AS [Price]
					FROM [ItemPrice]
					WHERE 
					GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate]) AND 
					[PriceType] = 0 AND [Price] > 0
				),
				CheapestPeriodPrices
				AS
				(
					SELECT [ItemId], MIN([Price]) AS [Price]
					FROM [ItemPrice]
					WHERE [PriceType] = 0 AND [Price] > 0
					GROUP BY [ItemId]
				),
				SpecialPrices
				AS
				(
					SELECT [ItemId], MIN([Price]) AS [Price]
					FROM [ItemPrice]
					WHERE 
					GETDATE() BETWEEN [StartDate] AND DATEADD(DAY, 1, [EndDate]) AND 
					[PriceType] = 1 AND [Price] > 0
					GROUP BY [ItemId]
				),
				TeeSheetCheapestPrice
				AS
				(
					SELECT [ItemId], MIN([Discount]) AS [CheapestPrice] FROM [TeeSheet] WHERE [Price] > 0 AND [Discount] > 0 AND [TeeSheetDate] >= GETDATE() GROUP BY [ItemId]
				)
				SELECT '
	IF(@PageSize > 0)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@PageSize AS NVARCHAR) + ') '
	END
	
	SET @SELECT = ' [Item].[ItemId], [Item].[ItemTypeId], [Item].[ItemSlug], [Item].[Price], [ItemLang].[ItemName], [ItemLang].[ItemShortDesc], img.[ItemImageId], img.[ListNo], img.[ImageName], img.[BaseName], img.[FileExtension], [SiteLang].[SiteName] '
	
	SET @SQL = @SQL +' it.* FROM 
	(SELECT DISTINCT ROW_NUMBER() OVER(ORDER BY [Item].[ItemId] DESC) AS RowNumber, ' + @SELECT + ', ISNULL(ipp.[Price], 0) AS PeriodPrice, ISNULL(icpp.[Price], 0) AS CheapestPeriodPrice, ISNULL(idp.[Price], 0) AS SpecialPrice, ISNULL(tp.[CheapestPrice], 0) AS TeeSheetCheapestPrice, AVG(Rating) AS AverageRating
	FROM [Item] 
	LEFT JOIN [ItemReview] ON [ItemReview].[ItemId] = [Item].[ItemId] AND [ItemReview].[IsApproved] = 1
	LEFT JOIN [ItemCategory] ON [ItemCategory].[CategoryId] = [Item].[CategoryId]
	LEFT JOIN [SiteLang] ON [Item].[SiteId] = [SiteLang].[SiteId] AND [SiteLang].[LangId] = ' + CAST(@LangId AS NVARCHAR)
	SET @SQL = @SQL + ' LEFT JOIN [ItemLang] ON [ItemLang].[ItemId] = [Item].[ItemId] AND [ItemLang].[LangId] = ' + CAST(@LangId AS NVARCHAR)
	
	SET @SQL = @SQL + ' LEFT JOIN [ItemImage] img ON img.[ItemImageId] = (SELECT TOP(1) [ItemImageId] FROM [ItemImage] WHERE [ItemImage].[ItemId] = [Item].[ItemId] ORDER BY [ItemImage].[ListNo], [ItemImage].[ItemImageId])
	  LEFT JOIN PeriodPrices ipp ON ipp.[ItemId] = [Item].[ItemId] AND ipp.[Price] > 0
	  LEFT JOIN CheapestPeriodPrices icpp ON icpp.[ItemId] = [Item].[ItemId] AND icpp.[Price] > 0
	  LEFT JOIN SpecialPrices idp ON idp.[ItemId] = [Item].[ItemId] AND idp.[Price] > 0
	  LEFT JOIN TeeSheetCheapestPrice tp ON tp.[ItemId] = [Item].[ItemId]
	WHERE [Item].[Active] = 1 AND [IsPublish] = 1 AND [Item].[IsPublish] = 1 AND [Item].[ItemTypeId] <> 6'
	
	IF(@ItemTypeId IS NOT NULL AND @ItemTypeId > 0)
	BEGIN
		SET @SQL = @SQL + ' AND [Item].[ItemTypeId] = ' + CAST(@ItemTypeId AS NVARCHAR)
	END
	
	IF(@CategoryId IS NOT NULL AND @CategoryId > 0)
	BEGIN
		SET @SQL = @SQL + ' AND [Item].[CategoryId] = ' + CAST(@CategoryId AS NVARCHAR)
	END
	
	
	IF(@CourseId IS NOT NULL AND @CourseId > 0)
	BEGIN
		SET @SQL = @SQL + ' AND [Item].[CourseId] = ' + CAST(@CourseId AS NVARCHAR)
	END
	ELSE
	BEGIN
		IF(@SiteId IS NOT NULL AND @SiteId > 0)
		BEGIN
			SET @SQL = @SQL + ' AND [Item].[SiteId] = ' + CAST(@SiteId AS NVARCHAR)
		END
	END
	
	IF(@SearchText IS NOT NULL AND @SearchText <> '')
	BEGIN
		SET @SQL = @SQL + ' AND ([ItemLang].[ItemName] LIKE ''%' + @SearchText + '%'' OR [ItemLang].[ItemDesc] LIKE ''%' + @SearchText + '%'' OR [ItemLang].[ItemShortDesc] LIKE ''%' + @SearchText + '%'')'
	END
	
	SET @SQL = @SQL + ' GROUP BY ' + @SELECT + ', idp.[Price], ipp.[Price], icpp.[Price], tp.[CheapestPrice]'
	
	SET @SQL = @SQL + ') AS it WHERE 1 = 1 '
	
	IF(@PageSize > 0 AND @PageIndex > 0)
	BEGIN
		SET @SQL = @SQL + ' AND RowNumber >= ' + CAST((@PageSize * (@PageIndex - 1)) AS NVARCHAR)
	END
	
	IF(@SkipId IS NOT NULL AND @SkipId > 0)
	BEGIN
		SET @SQL = @SQL + ' AND it.[ItemId] <> ' + CAST(@SkipId AS NVARCHAR)
	END
	
	SET @SQL = @SQL + ' ORDER BY NEWID()'
	EXEC SP_EXECUTESQL @SQL
	--PRINT @SQL
	
	SELECT CEILING(COUNT(*) / @PageSize) AS 'TotalPage' FROM [Item]
	RETURN
END

GO
/****** Object:  StoredProcedure [dbo].[SelectResellersList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectResellersList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectResellersList] AS' 
END
GO
ALTER PROCEDURE [dbo].[SelectResellersList]
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL,
	@SiteId INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;
	
	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('
	SET @SQL = @SQL + 'SELECT ROW_NUMBER() OVER (ORDER BY Email) AS RowNumber, * FROM [Users] '
	SET @SQL = @SQL + ' WHERE UserTypeId = 3 AND Active = 1'	

	IF(@SiteId IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND [SiteId] = ' + CAST(@SiteId AS NVARCHAR)
	END
		
	SET @SQL = @SQL + ') AS it'
	SET @SQL = @SQL + ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'AND (Email LIKE ''%' + @Search + '%'' OR Firstname LIKE ''%' + @Search + '%'' OR Lastname LIKE ''%' + @Search + '%'' OR Middlename LIKE ''%' + @Search + '%'')'
	END
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND it.RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'ORDER BY ' + @Order
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM Users WHERE UserTypeId = 3 AND Active = 1'
	
	exec sp_executesql @SQL
	
END

GO
/****** Object:  StoredProcedure [dbo].[SelectSaleItemsReport]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectSaleItemsReport]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectSaleItemsReport] AS' 
END
GO

ALTER PROCEDURE [dbo].[SelectSaleItemsReport]
(
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL,
	@FromDate NVARCHAR(MAX) = NULL,
	@ToDate NVARCHAR(MAX) = NULL,
	@SiteId INT = 0,
	@LangId INT = 1
)
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = '',
	@Select NVARCHAR(MAX) = '',
	@From NVARCHAR(MAX) = '',
	@WHERE NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	

	SET @SQL = 'SET LANGUAGE FRENCH;'
	
	SET @SQL = @SQL + 'SELECT '
	
	IF(@Length IS NOT NULL AND @Length > 0)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('
	
	SET @SQL = @SQL + 'SELECT ROW_NUMBER() OVER (ORDER BY OrderDate) AS RowNumber, OrderId, MonthName, NumOfWeek, OrderDate, TransactionId, OrderNumber, CategoryName, ItemCode, ItemName, ItemTypeId, ReserveDate, GolfName,
		FirstName, LastName, PaymentType, PaymentStatus, Qty, TotalBasePrice, Discount, ShippingCost, TotalTTC, TotalHT, GolfBrandName'
	
	SET @From = ' FROM	uv_SaleItemsReport
				WHERE (SiteId = ' + CAST(@SiteId AS NVARCHAR) + ' OR ' + CAST(@SiteId AS NVARCHAR) + ' = 0) AND (LangId = ' + CAST(@LangId AS NVARCHAR) + ')'
				
	IF(@FromDate IS NOT NULL AND @ToDate IS NOT NULL)
		SET @From = @From + ' AND OrderDate BETWEEN CONVERT(datetime, ''' + @FromDate + ' 00:00:00:000'', 103) AND CONVERT(datetime, ''' + @ToDate + ' 23:59:59:000'', 103)'
	
	SET @SQL = @SQL + @From
	SET @SQL = @SQL + ') AS it'
	SET @WHERE = ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @WHERE = @WHERE + ' AND (OrderNumber LIKE ''%' + @Search + '%'' OR CategoryName LIKE ''%' + @Search + '%'' OR ItemCode LIKE ''%' + @Search + '%'' OR ItemName LIKE ''%' + @Search + '%'' OR GolfName LIKE ''%' + @Search + '%'' OR FirstName LIKE ''%' + @Search + '%'' OR LastName LIKE ''%' + @Search + '%'')'
	END

	SET @SQL = @SQL + @WHERE

	IF(@Start IS NOT NULL AND @Start > 0)
	BEGIN
		SET @SQL = @SQL + ' AND RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' ORDER BY ' + @Order
	END
	ELSE
	BEGIN
		SET @SQL = @SQL + ' ORDER BY RowNumber'
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows ' + @From
	
	EXEC sp_executesql @SQL
	--PRINT @SQL
END

GO
/****** Object:  StoredProcedure [dbo].[SelectSite]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectSite]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectSite] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single Site
-- =============================================
ALTER PROCEDURE [dbo].[SelectSite]
	@SiteId Int,
	@LangId Int
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [Site].* FROM [Site]
	WHERE [SiteId] = @SiteId
	
	SELECT [SiteLang].* FROM [SiteLang] WHERE [SiteLang].[SiteId] = @SiteId AND [SiteLang].[LangId] = @LangId ORDER BY [SiteLang].[LangId]
END


GO
/****** Object:  StoredProcedure [dbo].[SelectSiteById]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectSiteById]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectSiteById] AS' 
END
GO

ALTER PROCEDURE [dbo].[SelectSiteById]
	@SiteId INT = 0,
	@LangId INT = 1
AS
BEGIN
	SELECT * FROM [Site] si
	INNER JOIN [dbo].[SiteLang] sl ON si.SiteId = sl.SiteId
	WHERE si.SiteId = @SiteId AND sl.LangId = @LangId
	
	SELECT [SiteLang].* FROM [SiteLang] WHERE [SiteId] = @SiteId
END


GO
/****** Object:  StoredProcedure [dbo].[SelectSiteReviewsList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectSiteReviewsList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectSiteReviewsList] AS' 
END
GO
ALTER PROCEDURE [dbo].[SelectSiteReviewsList]
(
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL,
	@LangId INT = 1
)
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = '',
	@WHERE NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('
	SET @SQL = @SQL + 'SELECT ROW_NUMBER() OVER (ORDER BY SiteReviewId DESC) AS RowNumber, [SiteReview].*, [Users].[Firstname] + '' '' + [Users].[Lastname] AS ReviewerName, [City].[CityName] AS ReviewerCityName, [Site].[SiteSlug], [SiteLang].[SiteName] AS SiteName FROM [SiteReview] '
					+ ' INNER JOIN [Users] ON [Users].[UserId] = [SiteReview].[UserId]'
					+ ' LEFT JOIN [City] ON [City].[CityId] = [Users].[CityId]'
					+ ' INNER JOIN [Site] ON [Site].[SiteId] = [SiteReview].[SiteId]'
					+ ' LEFT JOIN [SiteLang] ON [SiteLang].[SiteId] = [Site].[SiteId] AND [SiteLang].[LangId] = ' + CAST(@LangId AS NVARCHAR)
	--SET @SQL = @SQL + ' WHERE Active = 1'
	SET @SQL = @SQL + ') AS it'
	SET @WHERE = ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @WHERE = @WHERE + ' AND (Subject LIKE ''%' + @Search + '%'' OR Message LIKE ''%' + @Search + '%'')'
	END

	SET @SQL = @SQL + @WHERE

	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' ORDER BY ' + @Order
	END
	ELSE
	BEGIN
		SET @SQL = @SQL + ' ORDER BY [SiteReviewId] DESC'
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM [SiteReview]'
	+ ' INNER JOIN [Users] ON [Users].[UserId] = [SiteReview].[UserId]'
	+ ' INNER JOIN [Site] ON [Site].[SiteId] = [SiteReview].[SiteId]'
	
	EXEC sp_executesql @SQL
	--PRINT @SQL
END
GO
/****** Object:  StoredProcedure [dbo].[SelectSitesList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectSitesList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectSitesList] AS' 
END
GO
ALTER PROCEDURE [dbo].[SelectSitesList]
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL,
	@LangId INT = 1
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;
	
	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('

	SET @SQL = @SQL + 'SELECT ROW_NUMBER() OVER (ORDER BY SiteName) AS RowNumber, [Site].*, [SiteLang].[SiteName] FROM [Site] '
	SET @SQL = @SQL + ' LEFT JOIN [SiteLang] ON [SiteLang].[SiteId] = [Site].[SiteId] AND [SiteLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) 
	SET @SQL = @SQL + ' WHERE Active = 1'
	SET @SQL = @SQL + ') AS it'
	SET @SQL = @SQL + ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'AND (SiteName LIKE ''%' + @Search + '%'')'
	END
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND it.RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'ORDER BY ' + @Order
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM Site WHERE Active = 1'
	
	exec sp_executesql @SQL
END
GO
/****** Object:  StoredProcedure [dbo].[SelectStayPackage]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectStayPackage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectStayPackage] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single Item
-- =============================================
ALTER PROCEDURE [dbo].[SelectStayPackage]
	@ItemId Int = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [Item].* FROM [Item]
	WHERE [ItemId] = @ItemId
	
	SELECT [ItemLang].* FROM [ItemLang] WHERE [ItemLang].[ItemId] = @ItemId ORDER BY [ItemLang].[LangId]
	
	SELECT TOP 1 [ItemPosition].* FROM [ItemPosition] WHERE [ItemPosition].[ItemId] = @ItemId
END


GO
/****** Object:  StoredProcedure [dbo].[SelectStayPackagesList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectStayPackagesList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectStayPackagesList] AS' 
END
GO
ALTER PROCEDURE [dbo].[SelectStayPackagesList]
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL,
	@LangId INT = 1,
	@SiteId INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;	

	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('
	SET @SQL = @SQL + ' SELECT ROW_NUMBER() OVER (ORDER BY ItemCode) AS RowNumber, [Item].*, [ItemLang].[ItemName], [ItemCategory].[CategoryName], [SiteLang].[SiteName] FROM [Item] '
	SET @SQL = @SQL + ' LEFT JOIN [ItemLang] ON [ItemLang].[ItemId] = [Item].[ItemId] AND [ItemLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) 
	SET @SQL = @SQL + ' LEFT JOIN [ItemCategory] ON [ItemCategory].[CategoryId] = [Item].[CategoryId]'
	SET @SQL = @SQL + ' LEFT JOIN [SiteLang] ON [SiteLang].[SiteId] = [Item].[SiteId] AND [SiteLang].[LangId] = ' + CAST(@LangId AS NVARCHAR) 
	SET @SQL = @SQL + ' WHERE [Item].[Active] = 1 AND [Item].[ItemTypeId] = 2 '
	
	IF(@SiteId IS NOT NULL)
	SET @SQL = @SQL + ' AND  [Item].[SiteId] = ' + CAST(@SiteId AS NVARCHAR)
	
	SET @SQL = @SQL + ') AS it'
	SET @SQL = @SQL + ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'AND (it.ItemCode LIKE ''%' + @Search + '%'' OR it.ItemName LIKE ''%' + @Search + '%'' OR it.SiteName LIKE ''%' + @Search + '%'')'
	END
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND it.RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'ORDER BY ' + @Order
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM Item WHERE Active = 1 AND ItemTypeId = 2'
	
	exec sp_executesql @SQL
END

SET ANSI_NULLS ON

GO
/****** Object:  StoredProcedure [dbo].[SelectSupplier]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectSupplier]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectSupplier] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single Supplier
-- =============================================
ALTER PROCEDURE [dbo].[SelectSupplier]
	@SupplierId Int = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [Supplier].* FROM [Supplier]
	WHERE [SupplierId] = @SupplierId
END


GO
/****** Object:  StoredProcedure [dbo].[SelectSuppliersList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectSuppliersList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectSuppliersList] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Item Suppliers List
-- =============================================
ALTER PROCEDURE [dbo].[SelectSuppliersList]
	@WHERE NVARCHAR(255) = '',
	@Total INT = NULL,
	@Start INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = '',
	@SQLWHERE NVARCHAR(MAX) = '',
	@SQLTotal NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT '
		
	--IF(ISNULL(@Total, 0) > 0)
	--	SET @SQL = @SQL + ' TOP(' + CAST(@Total AS NVARCHAR) + ')'
	
	SET @SQLWHERE = ' Active = 1'
	
	IF(LEN(@WHERE) > 0)
		SET @SQLWHERE = @SQLWHERE + ' AND ([SupplierName] LIKE ''%' + @WHERE + '%'')'
		
	SET @SQL = @SQL + ' it.* FROM (SELECT ROW_NUMBER() OVER (ORDER BY SupplierName) AS RowNumber, * FROM [Supplier] '
	SET @SQL = @SQL + ' WHERE Active = 1 '	
	IF(LEN(@WHERE) > 0)
		SET @SQL = @SQL + ' AND ([SupplierName] LIKE ''%' + @WHERE + '%'')'
		
	--IF(ISNULL(@Start, 0) > 0)
	--	SET @SQL = @SQL + ' AND RowNumber >= ' + CAST(@Start AS NVARCHAR)
		
	SET @SQL = @SQL + ') AS it ORDER BY it.SupplierName'
	
	exec sp_executesql @SQL
	
	SET @SQLTotal = 'SELECT COUNT(*) AS TotalItems FROM [Supplier] WHERE ' + @SQLWHERE
	exec sp_executesql @SQLTotal
END


GO
/****** Object:  StoredProcedure [dbo].[SelectTax]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectTax]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectTax] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single Tax
-- =============================================
ALTER PROCEDURE [dbo].[SelectTax]
	@TaxId Int = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT [Tax].* FROM [Tax]
	WHERE [TaxId] = @TaxId
END


GO
/****** Object:  StoredProcedure [dbo].[SelectTaxesList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectTaxesList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectTaxesList] AS' 
END
GO
ALTER PROCEDURE [dbo].[SelectTaxesList]
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;
	
	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('	
	SET @SQL = 'SELECT ROW_NUMBER() OVER (ORDER BY TaxCode) AS RowNumber, * FROM [Tax] ORDER BY TaxCode'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'AND (TaxCode LIKE ''%' + @Search + '%'' OR TaxName LIKE ''%' + @Search + '%'')'
	END
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND it.RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'ORDER BY ' + @Order
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM Users WHERE UserTypeId = 1 AND Active = 1'
	
	exec sp_executesql @SQL
END
GO
/****** Object:  StoredProcedure [dbo].[SelectUser]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectUser]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectUser] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-02-12
-- Description:	Select Single User
-- =============================================
ALTER PROCEDURE [dbo].[SelectUser]
	@UserId Int = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM [Users] WHERE [UserId] = @UserId
END


GO
/****** Object:  StoredProcedure [dbo].[SelectUsersList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectUsersList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectUsersList] AS' 
END
GO
ALTER PROCEDURE [dbo].[SelectUsersList]
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL,
	@SiteId INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;
	
	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('
	SET @SQL = @SQL + 'SELECT ROW_NUMBER() OVER (ORDER BY Email) AS RowNumber, * FROM [Users] '
	SET @SQL = @SQL + ' WHERE UserTypeId = 1 AND Active = 1'	

	IF(@SiteId IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND [SiteId] = ' + CAST(@SiteId AS NVARCHAR)
	END
		
	SET @SQL = @SQL + ') AS it'
	SET @SQL = @SQL + ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'AND (Email LIKE ''%' + @Search + '%'' OR Firstname LIKE ''%' + @Search + '%'' OR Lastname LIKE ''%' + @Search + '%'' OR Middlename LIKE ''%' + @Search + '%'')'
	END
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND it.RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'ORDER BY ' + @Order
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM Users WHERE UserTypeId = 1 AND Active = 1'
	
	exec sp_executesql @SQL
	
END

GO
/****** Object:  StoredProcedure [dbo].[SelectUserType]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectUserType]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectUserType] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single User Type
-- =============================================
ALTER PROCEDURE [dbo].[SelectUserType]
	@UserTypeId Int = 0
AS
BEGIN
	SET NOCOUNT ON;

	SELECT * FROM [UserTypes] WHERE [UserTypeId] = @UserTypeId
END


GO
/****** Object:  StoredProcedure [dbo].[SelectUserTypesList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectUserTypesList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectUserTypesList] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Select Single User Type
-- =============================================
ALTER PROCEDURE [dbo].[SelectUserTypesList]
	@WHERE NVARCHAR(255) = '',
	@Total INT = NULL,
	@Start INT = NULL
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT '
		
	IF(ISNULL(@Total, 0) > 0)
		SET @SQL = @SQL + ' TOP(' + CAST(@Total AS NVARCHAR) + ')'
		
	SET @SQL = @SQL + ' it.* FROM (SELECT ROW_NUMBER() OVER (ORDER BY UserTypeName) AS RowNumber, * FROM [UserTypes] '
	IF(LEN(@WHERE) > 0)
		SET @SQL = @SQL + ' WHERE [UserTypeName] LIKE ''%' + @WHERE + '%'''
		
	IF(ISNULL(@Start, 0) > 0)
		SET @SQL = @SQL + ' AND RowNumber >= ' + CAST(@Start AS NVARCHAR)
		
	SET @SQL = @SQL + ') AS it ORDER BY it.UserTypeName'
	
	exec sp_executesql @SQL
END


GO
/****** Object:  StoredProcedure [dbo].[SelectWebContentsList]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SelectWebContentsList]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[SelectWebContentsList] AS' 
END
GO
ALTER PROCEDURE [dbo].[SelectWebContentsList]
(
	@Start INT = NULL,
	@Length INT = NULL,
	@Search NVARCHAR(MAX) = NULL,
	@Order NVARCHAR(MAX) = NULL,
	@LangId INT = NULL
)
AS
BEGIN
	DECLARE @SQL NVARCHAR(MAX) = ''
	SET NOCOUNT ON;

	SET @SQL = 'SELECT '
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' TOP(' + CAST(@Length AS NVARCHAR) + ')'
	END
	
	SET @SQL = @SQL + ' it.* FROM('
	SET @SQL = @SQL + 'SELECT ROW_NUMBER() OVER (ORDER BY OrderNumber, TopicName, [WebContent].ContentId) AS RowNumber, [WebContent].*, [WebContentLang].[TopicName] FROM [WebContent] '
	SET @SQL = @SQL + ' LEFT JOIN [WebContentLang] ON [WebContentLang].[ContentId] = [WebContent].[ContentId] AND [WebContentLang].[LangId] = ' + CAST(@LangId AS NVARCHAR)
	SET @SQL = @SQL + ' WHERE Active = 1'
	SET @SQL = @SQL + ') AS it'
	SET @SQL = @SQL + ' WHERE 1 = 1'
	
	IF(@Search IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'AND (ContentCategory LIKE ''%' + @Search + '%'' OR ContentKey LIKE ''%' + @Search + '%'' OR TopicName LIKE ''%' + @Search + '%'')'
	END
	
	IF(@Length IS NOT NULL AND @Start IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + ' AND it.RowNumber > ' + CAST(@Start AS NVARCHAR)
	END
	
	IF(@Order IS NOT NULL)
	BEGIN
		SET @SQL = @SQL + 'ORDER BY ' + @Order
	END
	
	SET @SQL = @SQL + CHAR(13) + 'SELECT COUNT(*) AS TotalRows FROM WebContent WHERE Active = 1'
	
	exec sp_executesql @SQL
END
GO
/****** Object:  StoredProcedure [dbo].[UpdateConnectWithFacebook]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateConnectWithFacebook]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[UpdateConnectWithFacebook] AS' 
END
GO
ALTER PROCEDURE [dbo].[UpdateConnectWithFacebook]
@UserId INT,
@AppId NVARCHAR(512)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE [dbo].[Users] SET [FBAccount] = @AppId WHERE [UserId] = @UserId
END

GO
/****** Object:  StoredProcedure [dbo].[UpdateFlagMessage]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateFlagMessage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[UpdateFlagMessage] AS' 
END
GO
ALTER PROCEDURE [dbo].[UpdateFlagMessage]
	@MessageId INT  = NULL,
	@Result INT OUTPUT
AS
BEGIN

	SET NOCOUNT ON;

	declare @setisflag  int
	set @setisflag = (select [IsFlag] from [dbo].[UserMessage] WHERE [MessageId] = @MessageId)


	IF @setisflag = 1
	BEGIN
		UPDATE [dbo].[UserMessage]
		SET 
		  [IsFlag] = 0 
		WHERE [MessageId] = @MessageId
	END
	ELSE
	BEGIN
		UPDATE [dbo].[UserMessage]
			SET 
		  [IsFlag] = 1 
		WHERE [MessageId] = @MessageId
	END
	

	SET @Result  = 1

END

GO
/****** Object:  StoredProcedure [dbo].[UpdatePromotion]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdatePromotion]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[UpdatePromotion] AS' 
END
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[UpdatePromotion]
@PromotionImage nvarchar(255) = null,
@PromotionBrandImage nvarchar(255) = null,
@PromotionContent nvarchar(255) = null,
@PromotionTimecontent nvarchar(255) = null,
@BrandNameId INT = null,
@PromotionId Int = null
AS
BEGIN

	SET NOCOUNT ON;
	UPDATE [dbo].[Promotion]
	   SET [PromotionImage] = @PromotionImage
		  ,[PromotionBrandImage] = @PromotionBrandImage
		  ,[PromotionContent] = @PromotionContent
		  ,[PromotionTimecontent] = @PromotionTimecontent
		  ,[UpdateDate] = GETUTCDATE()
		  ,[BrandNameId] = @BrandNameId
	 WHERE [PromotionId] =  @PromotionId
END


GO
/****** Object:  StoredProcedure [dbo].[UpdateReadMessage]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateReadMessage]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[UpdateReadMessage] AS' 
END
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[UpdateReadMessage]
	@MessageId INT  = NULL,
	@Result INT OUTPUT
AS
BEGIN

	SET NOCOUNT ON;
	UPDATE [dbo].[UserMessage]
    SET 
      [ReadDate] = GETUTCDATE()   
	WHERE [MessageId] = @MessageId

	SET @Result  = 1

END

GO
/****** Object:  StoredProcedure [dbo].[UpdateUserType]    Script Date: 9/16/2015 5:36:48 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[UpdateUserType]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE PROCEDURE [dbo].[UpdateUserType] AS' 
END
GO

-- =============================================
-- Author:		Weerayut Teja
-- Create date: 2014-03-01
-- Description:	Update User Type
-- =============================================
ALTER PROCEDURE [dbo].[UpdateUserType]
	@UserTypeId INT = NULL,
	@UserTypeName NVARCHAR(50) = NULL,
	@RowCount INT = NULL OUTPUT
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE [UserTypes] SET UserTypeName = @UserTypeName WHERE UserTypeId = @UserTypeId
	
	SELECT @RowCount = @@ROWCOUNT
END


GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_DiagramPane1' , N'SCHEMA',N'dbo', N'VIEW',N'uv_SaleItemsReport', NULL,NULL))
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[21] 4[22] 2[23] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "oi"
            Begin Extent = 
               Top = 6
               Left = 246
               Bottom = 135
               Right = 416
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Order"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 135
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Item"
            Begin Extent = 
               Top = 6
               Left = 454
               Bottom = 135
               Right = 672
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ItemCategory"
            Begin Extent = 
               Top = 6
               Left = 710
               Bottom = 135
               Right = 891
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SiteLang"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 267
               Right = 217
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Users"
            Begin Extent = 
               Top = 138
               Left = 255
               Bottom = 267
               Right = 496
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Site"
            Begin Extent = 
               Top = 138
               Left = 534
               Bottom = 267
               Right = 720
            End
            DisplayFlags = 280
           ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'uv_SaleItemsReport'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_DiagramPane2' , N'SCHEMA',N'dbo', N'VIEW',N'uv_SaleItemsReport', NULL,NULL))
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' TopColumn = 0
         End
         Begin Table = "GolfBrand"
            Begin Extent = 
               Top = 138
               Left = 758
               Bottom = 267
               Right = 932
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 22
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'uv_SaleItemsReport'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_DiagramPane1' , N'SCHEMA',N'dbo', N'VIEW',N'uv_SaleOrderReport', NULL,NULL))
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "it"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 135
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Users"
            Begin Extent = 
               Top = 138
               Left = 38
               Bottom = 267
               Right = 279
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "oi"
            Begin Extent = 
               Top = 6
               Left = 246
               Bottom = 101
               Right = 416
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 9
         Width = 284
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'uv_SaleOrderReport'
GO
IF NOT EXISTS (SELECT * FROM ::fn_listextendedproperty(N'MS_DiagramPaneCount' , N'SCHEMA',N'dbo', N'VIEW',N'uv_SaleOrderReport', NULL,NULL))
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'uv_SaleOrderReport'
GO
