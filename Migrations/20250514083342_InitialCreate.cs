using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SIETE.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FundCluster",
                columns: table => new
                {
                    FundClusterID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FundClusterCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FundClusterName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FundCluster", x => x.FundClusterID);
                });

            migrationBuilder.CreateTable(
                name: "Office",
                columns: table => new
                {
                    OfficeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfficeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Acronym = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OfficeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RespCenter_Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Parent_Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Office", x => x.OfficeID);
                });

            migrationBuilder.CreateTable(
                name: "PlantillaPosition",
                columns: table => new
                {
                    PositionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PositionTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlantillaPosition", x => x.PositionID);
                });

            migrationBuilder.CreateTable(
                name: "PriceThreshold",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyThreshold = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SPThreshold = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PriceThreshold", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PropertyCard",
                columns: table => new
                {
                    PropertyCardID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyCardName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PropertyDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PropertyUnitMeasurement = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CurrentStockQuantity = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyCard", x => x.PropertyCardID);
                });

            migrationBuilder.CreateTable(
                name: "PropertyTransaction",
                columns: table => new
                {
                    TransactionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyTransaction", x => x.TransactionID);
                });

            migrationBuilder.CreateTable(
                name: "StockCard",
                columns: table => new
                {
                    StockCardID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StockCardName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StockDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    StockUnitMeasurement = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CurrentStockQuantity = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockCard", x => x.StockCardID);
                });

            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    SupplierID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContactNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TIN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsVatRegistered = table.Column<bool>(type: "bit", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.SupplierID);
                });

            migrationBuilder.CreateTable(
                name: "SupplyTransaction",
                columns: table => new
                {
                    TransactionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyTransaction", x => x.TransactionID);
                });

            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Suffix = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PositionID = table.Column<int>(type: "int", nullable: true),
                    OfficeID = table.Column<int>(type: "int", nullable: true),
                    IsAccountablePerson = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.EmployeeID);
                    table.ForeignKey(
                        name: "FK_Employee_Office_OfficeID",
                        column: x => x.OfficeID,
                        principalTable: "Office",
                        principalColumn: "OfficeID",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Employee_PlantillaPosition_PositionID",
                        column: x => x.PositionID,
                        principalTable: "PlantillaPosition",
                        principalColumn: "PositionID",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Property",
                columns: table => new
                {
                    PropertyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierID = table.Column<int>(type: "int", nullable: false),
                    FundClusterID = table.Column<int>(type: "int", nullable: false),
                    PropertyCardID = table.Column<int>(type: "int", nullable: true),
                    StockPropNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitMeasurement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DateAcquired = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Property", x => x.PropertyID);
                    table.ForeignKey(
                        name: "FK_Property_FundCluster_FundClusterID",
                        column: x => x.FundClusterID,
                        principalTable: "FundCluster",
                        principalColumn: "FundClusterID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Property_PropertyCard_PropertyCardID",
                        column: x => x.PropertyCardID,
                        principalTable: "PropertyCard",
                        principalColumn: "PropertyCardID");
                    table.ForeignKey(
                        name: "FK_Property_Supplier_SupplierID",
                        column: x => x.SupplierID,
                        principalTable: "Supplier",
                        principalColumn: "SupplierID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Supply",
                columns: table => new
                {
                    SupplyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierID = table.Column<int>(type: "int", nullable: false),
                    FundClusterID = table.Column<int>(type: "int", nullable: false),
                    StockCardID = table.Column<int>(type: "int", nullable: true),
                    StockPropNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SupplyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitMeasurement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    DateAcquired = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supply", x => x.SupplyID);
                    table.ForeignKey(
                        name: "FK_Supply_FundCluster_FundClusterID",
                        column: x => x.FundClusterID,
                        principalTable: "FundCluster",
                        principalColumn: "FundClusterID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Supply_StockCard_StockCardID",
                        column: x => x.StockCardID,
                        principalTable: "StockCard",
                        principalColumn: "StockCardID");
                    table.ForeignKey(
                        name: "FK_Supply_Supplier_SupplierID",
                        column: x => x.SupplierID,
                        principalTable: "Supplier",
                        principalColumn: "SupplierID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAccount",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAccount", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_UserAccount_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyAssignment",
                columns: table => new
                {
                    PropertyAssignmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyID = table.Column<int>(type: "int", nullable: false),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    DateAssigned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyAssignment", x => x.PropertyAssignmentID);
                    table.ForeignKey(
                        name: "FK_PropertyAssignment_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertyAssignment_Property_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "Property",
                        principalColumn: "PropertyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyAssignmentHistory",
                columns: table => new
                {
                    HistoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PropertyID = table.Column<int>(type: "int", nullable: false),
                    EmployeeID = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    TPSNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransferType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Remarks = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PropertyTransactionID = table.Column<int>(type: "int", nullable: true),
                    PreviousQuantity = table.Column<int>(type: "int", nullable: false),
                    LatestQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyAssignmentHistory", x => x.HistoryID);
                    table.ForeignKey(
                        name: "FK_PropertyAssignmentHistory_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employee",
                        principalColumn: "EmployeeID");
                    table.ForeignKey(
                        name: "FK_PropertyAssignmentHistory_PropertyTransaction_PropertyTransactionID",
                        column: x => x.PropertyTransactionID,
                        principalTable: "PropertyTransaction",
                        principalColumn: "TransactionID");
                    table.ForeignKey(
                        name: "FK_PropertyAssignmentHistory_Property_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "Property",
                        principalColumn: "PropertyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PropertyTransactionDetail",
                columns: table => new
                {
                    PropertyTransactionDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionID = table.Column<int>(type: "int", nullable: false),
                    PropertyID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    EmployeeID = table.Column<int>(type: "int", nullable: true),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DisposalType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AssignmentID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyTransactionDetail", x => x.PropertyTransactionDetailID);
                    table.ForeignKey(
                        name: "FK_PropertyTransactionDetail_Employee_EmployeeID",
                        column: x => x.EmployeeID,
                        principalTable: "Employee",
                        principalColumn: "EmployeeID");
                    table.ForeignKey(
                        name: "FK_PropertyTransactionDetail_PropertyTransaction_TransactionID",
                        column: x => x.TransactionID,
                        principalTable: "PropertyTransaction",
                        principalColumn: "TransactionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PropertyTransactionDetail_Property_PropertyID",
                        column: x => x.PropertyID,
                        principalTable: "Property",
                        principalColumn: "PropertyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplyInDetail",
                columns: table => new
                {
                    DetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionID = table.Column<int>(type: "int", nullable: false),
                    SupplyID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyInDetail", x => x.DetailID);
                    table.ForeignKey(
                        name: "FK_SupplyInDetail_SupplyTransaction_TransactionID",
                        column: x => x.TransactionID,
                        principalTable: "SupplyTransaction",
                        principalColumn: "TransactionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplyInDetail_Supply_SupplyID",
                        column: x => x.SupplyID,
                        principalTable: "Supply",
                        principalColumn: "SupplyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SupplyOutDetail",
                columns: table => new
                {
                    DetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionID = table.Column<int>(type: "int", nullable: false),
                    SupplyID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OfficeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplyOutDetail", x => x.DetailID);
                    table.ForeignKey(
                        name: "FK_SupplyOutDetail_Office_OfficeID",
                        column: x => x.OfficeID,
                        principalTable: "Office",
                        principalColumn: "OfficeID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplyOutDetail_SupplyTransaction_TransactionID",
                        column: x => x.TransactionID,
                        principalTable: "SupplyTransaction",
                        principalColumn: "TransactionID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupplyOutDetail_Supply_SupplyID",
                        column: x => x.SupplyID,
                        principalTable: "Supply",
                        principalColumn: "SupplyID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FundCluster",
                columns: new[] { "FundClusterID", "FundClusterCode", "FundClusterName" },
                values: new object[,]
                {
                    { 1, "F101", "MDS" },
                    { 2, "F102", "TRUST RECEIPTS" },
                    { 3, "F103", "TRUST TRAINING" },
                    { 4, "F104", "CFAG" }
                });

            migrationBuilder.InsertData(
                table: "Office",
                columns: new[] { "OfficeID", "Acronym", "IsActive", "OfficeName", "OfficeType", "Parent_Code", "RespCenter_Code" },
                values: new object[] { 1, "Ad", true, "Admin Office", "Office", "Admin", "Admin" });

            migrationBuilder.InsertData(
                table: "PlantillaPosition",
                columns: new[] { "PositionID", "IsActive", "PositionTitle" },
                values: new object[] { 1, true, "Programmer" });

            migrationBuilder.InsertData(
                table: "PriceThreshold",
                columns: new[] { "Id", "EffectiveDate", "PropertyThreshold", "SPThreshold" },
                values: new object[] { 1, new DateTime(2025, 5, 14, 16, 33, 40, 752, DateTimeKind.Local).AddTicks(7940), 50000m, 15000m });

            migrationBuilder.InsertData(
                table: "Employee",
                columns: new[] { "EmployeeID", "EmailAddress", "FirstName", "IsAccountablePerson", "IsActive", "LastName", "OfficeID", "PhoneNumber", "PositionID", "Suffix", "Title" },
                values: new object[] { 1, "niere@gmail.com", "Ernest", true, true, "Niere", 1, "09123456789", 1, "Jr.", "Mr." });

            migrationBuilder.InsertData(
                table: "UserAccount",
                columns: new[] { "UserID", "EmployeeID", "Password", "Role", "Username" },
                values: new object[] { 1, 1, "Admin123", 1, "Admin" });

            migrationBuilder.CreateIndex(
                name: "IX_Employee_OfficeID",
                table: "Employee",
                column: "OfficeID");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_PositionID",
                table: "Employee",
                column: "PositionID");

            migrationBuilder.CreateIndex(
                name: "IX_Property_FundClusterID",
                table: "Property",
                column: "FundClusterID");

            migrationBuilder.CreateIndex(
                name: "IX_Property_PropertyCardID",
                table: "Property",
                column: "PropertyCardID");

            migrationBuilder.CreateIndex(
                name: "IX_Property_SupplierID",
                table: "Property",
                column: "SupplierID");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyAssignment_EmployeeID",
                table: "PropertyAssignment",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyAssignment_PropertyID",
                table: "PropertyAssignment",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyAssignmentHistory_EmployeeID",
                table: "PropertyAssignmentHistory",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyAssignmentHistory_PropertyID",
                table: "PropertyAssignmentHistory",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyAssignmentHistory_PropertyTransactionID",
                table: "PropertyAssignmentHistory",
                column: "PropertyTransactionID");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyTransactionDetail_EmployeeID",
                table: "PropertyTransactionDetail",
                column: "EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyTransactionDetail_PropertyID",
                table: "PropertyTransactionDetail",
                column: "PropertyID");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyTransactionDetail_TransactionID",
                table: "PropertyTransactionDetail",
                column: "TransactionID");

            migrationBuilder.CreateIndex(
                name: "IX_Supply_FundClusterID",
                table: "Supply",
                column: "FundClusterID");

            migrationBuilder.CreateIndex(
                name: "IX_Supply_StockCardID",
                table: "Supply",
                column: "StockCardID");

            migrationBuilder.CreateIndex(
                name: "IX_Supply_SupplierID",
                table: "Supply",
                column: "SupplierID");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyInDetail_SupplyID",
                table: "SupplyInDetail",
                column: "SupplyID");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyInDetail_TransactionID",
                table: "SupplyInDetail",
                column: "TransactionID");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyOutDetail_OfficeID",
                table: "SupplyOutDetail",
                column: "OfficeID");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyOutDetail_SupplyID",
                table: "SupplyOutDetail",
                column: "SupplyID");

            migrationBuilder.CreateIndex(
                name: "IX_SupplyOutDetail_TransactionID",
                table: "SupplyOutDetail",
                column: "TransactionID");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccount_EmployeeID",
                table: "UserAccount",
                column: "EmployeeID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PriceThreshold");

            migrationBuilder.DropTable(
                name: "PropertyAssignment");

            migrationBuilder.DropTable(
                name: "PropertyAssignmentHistory");

            migrationBuilder.DropTable(
                name: "PropertyTransactionDetail");

            migrationBuilder.DropTable(
                name: "SupplyInDetail");

            migrationBuilder.DropTable(
                name: "SupplyOutDetail");

            migrationBuilder.DropTable(
                name: "UserAccount");

            migrationBuilder.DropTable(
                name: "PropertyTransaction");

            migrationBuilder.DropTable(
                name: "Property");

            migrationBuilder.DropTable(
                name: "SupplyTransaction");

            migrationBuilder.DropTable(
                name: "Supply");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "PropertyCard");

            migrationBuilder.DropTable(
                name: "FundCluster");

            migrationBuilder.DropTable(
                name: "StockCard");

            migrationBuilder.DropTable(
                name: "Supplier");

            migrationBuilder.DropTable(
                name: "Office");

            migrationBuilder.DropTable(
                name: "PlantillaPosition");
        }
    }
}
