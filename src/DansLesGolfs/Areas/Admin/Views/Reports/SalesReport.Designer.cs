namespace DansLesGolfs.Areas.Admin.Views.Reports
{
    partial class SalesReport
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DevExpress.DataAccess.Sql.StoredProcQuery storedProcQuery1 = new DevExpress.DataAccess.Sql.StoredProcQuery();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter1 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter2 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter3 = new DevExpress.DataAccess.Sql.QueryParameter();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SalesReport));
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.xrTable2 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow2 = new DevExpress.XtraReports.UI.XRTableRow();
            this.Head_OrderNumber = new DevExpress.XtraReports.UI.XRTableCell();
            this.Head_OrderDate = new DevExpress.XtraReports.UI.XRTableCell();
            this.Head_CustomerName = new DevExpress.XtraReports.UI.XRTableCell();
            this.Head_ItemCode = new DevExpress.XtraReports.UI.XRTableCell();
            this.Head_ItemName = new DevExpress.XtraReports.UI.XRTableCell();
            this.Head_UnitPrice = new DevExpress.XtraReports.UI.XRTableCell();
            this.Head_Quantity = new DevExpress.XtraReports.UI.XRTableCell();
            this.Head_ExtendePrice = new DevExpress.XtraReports.UI.XRTableCell();
            this.Head_PaymentType = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell2 = new DevExpress.XtraReports.UI.XRTableCell();
            this.White = new DevExpress.XtraReports.UI.XRControlStyle();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            this.TableHeaderStyle = new DevExpress.XtraReports.UI.XRControlStyle();
            this.LavenderStyle = new DevExpress.XtraReports.UI.XRControlStyle();
            this.formattingRule1 = new DevExpress.XtraReports.UI.FormattingRule();
            this.LightBlue = new DevExpress.XtraReports.UI.XRControlStyle();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrTable1 = new DevExpress.XtraReports.UI.XRTable();
            this.xrTableRow1 = new DevExpress.XtraReports.UI.XRTableRow();
            this.OrderNumber = new DevExpress.XtraReports.UI.XRTableCell();
            this.OrderDate = new DevExpress.XtraReports.UI.XRTableCell();
            this.objectDataSource1 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource();
            this.CustomerName = new DevExpress.XtraReports.UI.XRTableCell();
            this.ItemCode = new DevExpress.XtraReports.UI.XRTableCell();
            this.ItemName = new DevExpress.XtraReports.UI.XRTableCell();
            this.UnitPrice = new DevExpress.XtraReports.UI.XRTableCell();
            this.Quantity = new DevExpress.XtraReports.UI.XRTableCell();
            this.ExtendPrice = new DevExpress.XtraReports.UI.XRTableCell();
            this.PaymentType = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrTableCell1 = new DevExpress.XtraReports.UI.XRTableCell();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.TableStyle = new DevExpress.XtraReports.UI.XRControlStyle();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.objectDataSource3 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource();
            this.objectDataSource2 = new DevExpress.DataAccess.ObjectBinding.ObjectDataSource();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // BottomMargin
            // 
            this.BottomMargin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BottomMargin.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.BottomMargin.HeightF = 273F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionName = "ConnectionString";
            this.sqlDataSource1.Name = "sqlDataSource1";
            storedProcQuery1.Name = "GetSalesReport";
            queryParameter1.Name = "@FromDate";
            queryParameter1.Type = typeof(System.DateTime);
            queryParameter1.ValueInfo = "1/1/2296 0:00:00";
            queryParameter2.Name = "@ToDate";
            queryParameter2.Type = typeof(System.DateTime);
            queryParameter2.ValueInfo = "1/1/2296 0:00:00";
            queryParameter3.Name = "@SideId";
            queryParameter3.Type = typeof(int);
            queryParameter3.ValueInfo = "0";
            storedProcQuery1.Parameters.Add(queryParameter1);
            storedProcQuery1.Parameters.Add(queryParameter2);
            storedProcQuery1.Parameters.Add(queryParameter3);
            storedProcQuery1.StoredProcName = "GetSalesReport";
            this.sqlDataSource1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            storedProcQuery1});
            this.sqlDataSource1.ResultSchemaSerializable = resources.GetString("sqlDataSource1.ResultSchemaSerializable");
            // 
            // TopMargin
            // 
            this.TopMargin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TopMargin.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TopMargin.HeightF = 54F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.StylePriority.UseBackColor = false;
            this.TopMargin.StylePriority.UseBorderColor = false;
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // GroupHeader1
            // 
            this.GroupHeader1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.GroupHeader1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable2});
            this.GroupHeader1.GroupFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("ProductName", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            this.GroupHeader1.HeightF = 27.08333F;
            this.GroupHeader1.Name = "GroupHeader1";
            this.GroupHeader1.RepeatEveryPage = true;
            // 
            // xrTable2
            // 
            this.xrTable2.BackColor = System.Drawing.Color.White;
            this.xrTable2.ForeColor = System.Drawing.Color.Black;
            this.xrTable2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrTable2.Name = "xrTable2";
            this.xrTable2.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow2});
            this.xrTable2.SizeF = new System.Drawing.SizeF(699F, 25F);
            this.xrTable2.StyleName = "TableHeaderStyle";
            // 
            // xrTableRow2
            // 
            this.xrTableRow2.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.Head_OrderNumber,
            this.Head_OrderDate,
            this.Head_CustomerName,
            this.Head_ItemCode,
            this.Head_ItemName,
            this.Head_UnitPrice,
            this.Head_Quantity,
            this.Head_ExtendePrice,
            this.Head_PaymentType,
            this.xrTableCell2});
            this.xrTableRow2.Name = "xrTableRow2";
            this.xrTableRow2.Weight = 1D;
            // 
            // Head_OrderNumber
            // 
            this.Head_OrderNumber.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Head_OrderNumber.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Head_OrderNumber.Name = "Head_OrderNumber";
            this.Head_OrderNumber.StylePriority.UseBackColor = false;
            this.Head_OrderNumber.StylePriority.UseFont = false;
            this.Head_OrderNumber.Text = "Order Number";
            this.Head_OrderNumber.Weight = 0.74269911984936754D;
            // 
            // Head_OrderDate
            // 
            this.Head_OrderDate.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Head_OrderDate.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Head_OrderDate.Name = "Head_OrderDate";
            this.Head_OrderDate.StylePriority.UseBackColor = false;
            this.Head_OrderDate.StylePriority.UseFont = false;
            this.Head_OrderDate.Text = "Order Date";
            this.Head_OrderDate.Weight = 0.812706404788708D;
            // 
            // Head_CustomerName
            // 
            this.Head_CustomerName.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Head_CustomerName.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Head_CustomerName.Name = "Head_CustomerName";
            this.Head_CustomerName.StyleName = "TableHeaderStyle";
            this.Head_CustomerName.StylePriority.UseBackColor = false;
            this.Head_CustomerName.StylePriority.UseFont = false;
            this.Head_CustomerName.Text = "Customer Name";
            this.Head_CustomerName.Weight = 0.90881753786326436D;
            // 
            // Head_ItemCode
            // 
            this.Head_ItemCode.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Head_ItemCode.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Head_ItemCode.Name = "Head_ItemCode";
            this.Head_ItemCode.StylePriority.UseBackColor = false;
            this.Head_ItemCode.StylePriority.UseFont = false;
            this.Head_ItemCode.Text = "Item Code";
            this.Head_ItemCode.Weight = 0.9132144214524831D;
            // 
            // Head_ItemName
            // 
            this.Head_ItemName.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Head_ItemName.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Head_ItemName.Name = "Head_ItemName";
            this.Head_ItemName.StylePriority.UseBackColor = false;
            this.Head_ItemName.StylePriority.UseFont = false;
            this.Head_ItemName.Text = "Item Name";
            this.Head_ItemName.Weight = 1.0275354722803214D;
            // 
            // Head_UnitPrice
            // 
            this.Head_UnitPrice.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Head_UnitPrice.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Head_UnitPrice.Name = "Head_UnitPrice";
            this.Head_UnitPrice.StylePriority.UseBackColor = false;
            this.Head_UnitPrice.StylePriority.UseFont = false;
            this.Head_UnitPrice.Text = "Unit Price";
            this.Head_UnitPrice.Weight = 0.89948486086433643D;
            // 
            // Head_Quantity
            // 
            this.Head_Quantity.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Head_Quantity.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Head_Quantity.Name = "Head_Quantity";
            this.Head_Quantity.StylePriority.UseBackColor = false;
            this.Head_Quantity.StylePriority.UseFont = false;
            this.Head_Quantity.Text = "Quantity";
            this.Head_Quantity.Weight = 0.87387555502697589D;
            // 
            // Head_ExtendePrice
            // 
            this.Head_ExtendePrice.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Head_ExtendePrice.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Head_ExtendePrice.Name = "Head_ExtendePrice";
            this.Head_ExtendePrice.StylePriority.UseBackColor = false;
            this.Head_ExtendePrice.StylePriority.UseFont = false;
            this.Head_ExtendePrice.Text = "Extended Price";
            this.Head_ExtendePrice.Weight = 0.89948561116627712D;
            // 
            // Head_PaymentType
            // 
            this.Head_PaymentType.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.Head_PaymentType.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Head_PaymentType.Name = "Head_PaymentType";
            this.Head_PaymentType.StylePriority.UseBackColor = false;
            this.Head_PaymentType.StylePriority.UseFont = false;
            this.Head_PaymentType.Text = "Payment Type";
            this.Head_PaymentType.Weight = 0.56737157425977358D;
            // 
            // xrTableCell2
            // 
            this.xrTableCell2.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.xrTableCell2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell2.Name = "xrTableCell2";
            this.xrTableCell2.StylePriority.UseBackColor = false;
            this.xrTableCell2.StylePriority.UseFont = false;
            this.xrTableCell2.Text = "Payment Status";
            this.xrTableCell2.Weight = 0.94757939207582786D;
            // 
            // White
            // 
            this.White.BackColor = System.Drawing.Color.White;
            this.White.BorderWidth = 0F;
            this.White.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.White.ForeColor = System.Drawing.Color.Black;
            this.White.Name = "White";
            this.White.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 0, 0, 0, 100F);
            // 
            // PageFooter
            // 
            this.PageFooter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.PageFooter.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.PageFooter.Borders = ((DevExpress.XtraPrinting.BorderSide)((DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.PageFooter.HeightF = 100F;
            this.PageFooter.Name = "PageFooter";
            // 
            // TableHeaderStyle
            // 
            this.TableHeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(150)))), ((int)(((byte)(122)))));
            this.TableHeaderStyle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.TableHeaderStyle.ForeColor = System.Drawing.Color.White;
            this.TableHeaderStyle.Name = "TableHeaderStyle";
            this.TableHeaderStyle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 0, 0, 0, 100F);
            // 
            // LavenderStyle
            // 
            this.LavenderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(235)))), ((int)(((byte)(235)))), ((int)(((byte)(235)))));
            this.LavenderStyle.Font = new System.Drawing.Font("Segoe UI", 9.75F);
            this.LavenderStyle.ForeColor = System.Drawing.Color.Black;
            this.LavenderStyle.Name = "LavenderStyle";
            this.LavenderStyle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 0, 0, 0, 100F);
            // 
            // formattingRule1
            // 
            this.formattingRule1.Name = "formattingRule1";
            // 
            // LightBlue
            // 
            this.LightBlue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(119)))), ((int)(((byte)(136)))), ((int)(((byte)(153)))));
            this.LightBlue.Name = "LightBlue";
            // 
            // Detail
            // 
            this.Detail.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Detail.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrTable1});
            this.Detail.ForeColor = System.Drawing.Color.Black;
            this.Detail.HeightF = 25F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.StylePriority.UseForeColor = false;
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrTable1
            // 
            this.xrTable1.EvenStyleName = "LavenderStyle";
            this.xrTable1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrTable1.Name = "xrTable1";
            this.xrTable1.OddStyleName = "White";
            this.xrTable1.Rows.AddRange(new DevExpress.XtraReports.UI.XRTableRow[] {
            this.xrTableRow1});
            this.xrTable1.SizeF = new System.Drawing.SizeF(699F, 25F);
            // 
            // xrTableRow1
            // 
            this.xrTableRow1.Cells.AddRange(new DevExpress.XtraReports.UI.XRTableCell[] {
            this.OrderNumber,
            this.OrderDate,
            this.CustomerName,
            this.ItemCode,
            this.ItemName,
            this.UnitPrice,
            this.Quantity,
            this.ExtendPrice,
            this.PaymentType,
            this.xrTableCell1});
            this.xrTableRow1.Name = "xrTableRow1";
            this.xrTableRow1.Weight = 1D;
            // 
            // OrderNumber
            // 
            this.OrderNumber.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "OrderNumber")});
            this.OrderNumber.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OrderNumber.Name = "OrderNumber";
            this.OrderNumber.StylePriority.UseFont = false;
            this.OrderNumber.Text = "OrderNumber";
            this.OrderNumber.Weight = 0.604166696030468D;
            // 
            // OrderDate
            // 
            this.OrderDate.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", this.objectDataSource1, "OrderDate")});
            this.OrderDate.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OrderDate.Name = "OrderDate";
            this.OrderDate.StylePriority.UseFont = false;
            this.OrderDate.Text = "OrderDate";
            this.OrderDate.Weight = 0.66111601708062628D;
            // 
            // objectDataSource1
            // 
            this.objectDataSource1.DataSource = typeof(DansLesGolfs.BLL.SalesReport);
            this.objectDataSource1.Name = "objectDataSource1";
            // 
            // CustomerName
            // 
            this.CustomerName.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "CustomerName")});
            this.CustomerName.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CustomerName.Name = "CustomerName";
            this.CustomerName.StylePriority.UseFont = false;
            this.CustomerName.Text = "CustomerName";
            this.CustomerName.Weight = 0.73929995137014093D;
            // 
            // ItemCode
            // 
            this.ItemCode.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "ItemCode")});
            this.ItemCode.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ItemCode.Name = "ItemCode";
            this.ItemCode.StylePriority.UseFont = false;
            this.ItemCode.Weight = 0.74287668570547738D;
            // 
            // ItemName
            // 
            this.ItemName.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "ItemName", "{0:MM/dd}")});
            this.ItemName.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ItemName.Name = "ItemName";
            this.ItemName.StylePriority.UseFont = false;
            this.ItemName.Text = "Item Name";
            this.ItemName.Weight = 0.83587365811462921D;
            // 
            // UnitPrice
            // 
            this.UnitPrice.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "UnitPrice")});
            this.UnitPrice.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UnitPrice.Name = "UnitPrice";
            this.UnitPrice.StylePriority.UseFont = false;
            this.UnitPrice.Text = "Unit Price";
            this.UnitPrice.Weight = 0.73170800953606718D;
            // 
            // Quantity
            // 
            this.Quantity.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "Quantity")});
            this.Quantity.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Quantity.Name = "Quantity";
            this.Quantity.StylePriority.UseFont = false;
            this.Quantity.Text = "Quantity";
            this.Quantity.Weight = 0.71087518609530131D;
            // 
            // ExtendPrice
            // 
            this.ExtendPrice.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "ExtendedPrice")});
            this.ExtendPrice.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExtendPrice.Name = "ExtendPrice";
            this.ExtendPrice.StylePriority.UseFont = false;
            this.ExtendPrice.Text = "ExtendPrice";
            this.ExtendPrice.Weight = 0.73170861988759273D;
            // 
            // PaymentType
            // 
            this.PaymentType.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "PaymentType")});
            this.PaymentType.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PaymentType.Name = "PaymentType";
            this.PaymentType.StylePriority.UseFont = false;
            this.PaymentType.Text = "Payment Type";
            this.PaymentType.Weight = 0.4615423044892607D;
            // 
            // xrTableCell1
            // 
            this.xrTableCell1.DataBindings.AddRange(new DevExpress.XtraReports.UI.XRBinding[] {
            new DevExpress.XtraReports.UI.XRBinding("Text", null, "PaymentStatus")});
            this.xrTableCell1.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrTableCell1.Name = "xrTableCell1";
            this.xrTableCell1.StylePriority.UseFont = false;
            this.xrTableCell1.Text = "PaymentStatus";
            this.xrTableCell1.Weight = 0.77083244682273111D;
            // 
            // xrLabel1
            // 
            this.xrLabel1.BorderWidth = 0F;
            this.xrLabel1.Font = new System.Drawing.Font("Calibri", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel1.ForeColor = System.Drawing.Color.Gray;
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 10.00001F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(699F, 38.33334F);
            this.xrLabel1.StyleName = "TableStyle";
            this.xrLabel1.StylePriority.UseBorderColor = false;
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseForeColor = false;
            this.xrLabel1.Text = "Sales Report";
            // 
            // TableStyle
            // 
            this.TableStyle.BackColor = System.Drawing.Color.White;
            this.TableStyle.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(218)))), ((int)(((byte)(185)))));
            this.TableStyle.BorderDashStyle = DevExpress.XtraPrinting.BorderDashStyle.Solid;
            this.TableStyle.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.TableStyle.Font = new System.Drawing.Font("Calibri", 36F);
            this.TableStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(150)))), ((int)(((byte)(122)))));
            this.TableStyle.Name = "TableStyle";
            this.TableStyle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 0, 0, 0, 100F);
            // 
            // PageHeader
            // 
            this.PageHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.PageHeader.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel1});
            this.PageHeader.HeightF = 55.62503F;
            this.PageHeader.Name = "PageHeader";
            this.PageHeader.StylePriority.UseBackColor = false;
            this.PageHeader.StylePriority.UseBorderColor = false;
            // 
            // objectDataSource3
            // 
            this.objectDataSource3.DataSource = typeof(DansLesGolfs.BLL.SalesReport);
            this.objectDataSource3.Name = "objectDataSource3";
            // 
            // objectDataSource2
            // 
            this.objectDataSource2.DataSource = typeof(DansLesGolfs.Areas.Admin.Views.Reports.SalesReport);
            this.objectDataSource2.Name = "objectDataSource2";
            // 
            // SalesReport
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.GroupHeader1,
            this.PageHeader,
            this.PageFooter});
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.ComponentStorage.Add(this.sqlDataSource1);
            this.ComponentStorage.Add(this.objectDataSource1);
            this.ComponentStorage.Add(this.objectDataSource2);
            this.ComponentStorage.Add(this.objectDataSource2);
            this.ComponentStorage.Add(this.objectDataSource2);
            this.ComponentStorage.Add(this.objectDataSource2);
            this.ComponentStorage.Add(this.objectDataSource3);
            this.DataSource = this.objectDataSource3;
            this.Font = new System.Drawing.Font("Arial Narrow", 9.75F);
            this.FormattingRuleSheet.AddRange(new DevExpress.XtraReports.UI.FormattingRule[] {
            this.formattingRule1});
            this.Margins = new System.Drawing.Printing.Margins(50, 101, 54, 273);
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.LightBlue,
            this.TableHeaderStyle,
            this.TableStyle,
            this.White,
            this.LavenderStyle});
            this.Version = "14.2";
            ((System.ComponentModel.ISupportInitialize)(this.xrTable2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.xrTable1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.objectDataSource2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
        private DevExpress.XtraReports.UI.XRTable xrTable2;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow2;
        private DevExpress.XtraReports.UI.XRControlStyle White;
        private DevExpress.XtraReports.UI.PageFooterBand PageFooter;
        private DevExpress.XtraReports.UI.XRControlStyle TableHeaderStyle;
        private DevExpress.XtraReports.UI.XRControlStyle LavenderStyle;
        private DevExpress.XtraReports.UI.FormattingRule formattingRule1;
        private DevExpress.XtraReports.UI.XRControlStyle LightBlue;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRControlStyle TableStyle;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
        private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource1;
        public DevExpress.XtraReports.UI.XRTableCell Head_Quantity;
        public DevExpress.XtraReports.UI.XRTableCell Head_OrderNumber;
        public DevExpress.XtraReports.UI.XRTableCell Head_OrderDate;
        public DevExpress.XtraReports.UI.XRTableCell Head_CustomerName;
        public DevExpress.XtraReports.UI.XRTableCell Head_ItemCode;
        public DevExpress.XtraReports.UI.XRTableCell Head_ItemName;
        public DevExpress.XtraReports.UI.XRTableCell Head_UnitPrice;
        public DevExpress.XtraReports.UI.XRTableCell Head_ExtendePrice;
        public DevExpress.XtraReports.UI.XRTableCell Head_PaymentType;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell2;
        private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource2;
        private DevExpress.XtraReports.UI.XRTable xrTable1;
        private DevExpress.XtraReports.UI.XRTableRow xrTableRow1;
        public DevExpress.XtraReports.UI.XRTableCell OrderNumber;
        public DevExpress.XtraReports.UI.XRTableCell OrderDate;
        public DevExpress.XtraReports.UI.XRTableCell CustomerName;
        public DevExpress.XtraReports.UI.XRTableCell ItemCode;
        public DevExpress.XtraReports.UI.XRTableCell ItemName;
        public DevExpress.XtraReports.UI.XRTableCell UnitPrice;
        public DevExpress.XtraReports.UI.XRTableCell Quantity;
        public DevExpress.XtraReports.UI.XRTableCell ExtendPrice;
        public DevExpress.XtraReports.UI.XRTableCell PaymentType;
        private DevExpress.XtraReports.UI.XRTableCell xrTableCell1;
        private DevExpress.DataAccess.ObjectBinding.ObjectDataSource objectDataSource3;

    }
}
