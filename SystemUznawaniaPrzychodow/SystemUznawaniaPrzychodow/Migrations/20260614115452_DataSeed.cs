using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SystemUznawaniaPrzychodow.Migrations
{
    /// <inheritdoc />
    public partial class DataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "ClientId", "Address", "ClientType", "Email", "FirstName", "IsDeleted", "LastName", "Pesel", "PhoneNumber" },
                values: new object[] { 1, "Warszawa 01-001", "Individual", "jan.kowalski@gmail.com", "Jan", false, "Kowalski", "12345678901", "123456789" });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "ClientId", "Address", "ClientType", "CompanyName", "Email", "Krs", "PhoneNumber" },
                values: new object[] { 2, "Kraków 30-002", "Company", "SoftPol Sp. z o.o.", "kontakt@softpol.pl", "0000123456", "987654321" });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "ClientId", "Address", "ClientType", "Email", "FirstName", "IsDeleted", "LastName", "Pesel", "PhoneNumber" },
                values: new object[] { 3, "Gdańsk 80-003", "Individual", "anna.nowak@eduplus.edu", "Anna", false, "Nowak", "98765432109", "555666777" });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "ClientId", "Address", "ClientType", "CompanyName", "Email", "Krs", "PhoneNumber" },
                values: new object[] { 4, "Wrocław 50-004", "Company", "MegaCorp S.A.", "office@megacorp.com", "0000987654", "444333222" });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "ClientId", "Address", "ClientType", "Email", "FirstName", "IsDeleted", "LastName", "Pesel", "PhoneNumber" },
                values: new object[] { 5, "Poznań 60-005", "Individual", "p.zielinski@wp.pl", "Piotr", false, "Zieliński", "55544433322", "888777666" });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "ClientId", "Address", "ClientType", "CompanyName", "Email", "Krs", "PhoneNumber" },
                values: new object[] { 6, "Łódź 90-006", "Company", "TechSolutions", "info@techsol.pl", "0000555444", "111222333" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "EmployeeId", "Login", "PasswordHash", "Role" },
                values: new object[,]
                {
                    { 1, "admin", "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", "Admin" },
                    { 2, "user", "$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy", "User" }
                });

            migrationBuilder.InsertData(
                table: "Software",
                columns: new[] { "SoftwareId", "AnnualPrice", "Category", "Description", "SoftwareName", "Version" },
                values: new object[,]
                {
                    { 1, 4999.99m, "Biznes", "Zarządzanie przedsiębiorstwem", "ERP System Pro", "2026.1" },
                    { 2, 1499.00m, "Edukacja", "Platforma e-learningowa", "EduLearn Platform", "4.2" },
                    { 3, 2999.50m, "Biznes", "System zarządzania relacjami", "SecureVault CRM", "11.0" },
                    { 4, 899.00m, "Narzędzia", "Chmura dla firm", "CloudDrive Core", "2.5" },
                    { 5, 2450.00m, "HR", "Automatyzacja procesów HR", "HR Automate", "2026.2" },
                    { 6, 7900.00m, "Finanse", "Analityka finansowa AI", "FinAnalyse Premium", "1.0" }
                });

            migrationBuilder.InsertData(
                table: "Contracts",
                columns: new[] { "ContractId", "AdditionalSupportYears", "ClientId", "DateFrom", "DateTo", "Deadline", "IsSigned", "Price", "SoftwareId", "SoftwareVersion" },
                values: new object[,]
                {
                    { 1, 1, 1, new DateOnly(2026, 1, 15), new DateOnly(2027, 1, 15), new DateOnly(2026, 1, 30), true, 5000.00m, 1, "2026.1" },
                    { 2, 0, 4, new DateOnly(2026, 2, 10), new DateOnly(2027, 2, 10), new DateOnly(2026, 2, 25), true, 8000.00m, 3, "11.0" },
                    { 3, 2, 5, new DateOnly(2026, 5, 1), new DateOnly(2027, 5, 1), new DateOnly(2026, 5, 15), true, 3000.00m, 5, "2026.2" },
                    { 4, 1, 2, new DateOnly(2026, 7, 1), new DateOnly(2027, 7, 1), new DateOnly(2026, 7, 25), false, 12000.00m, 1, "2026.1" },
                    { 5, 0, 6, new DateOnly(2026, 6, 20), new DateOnly(2027, 6, 20), new DateOnly(2026, 7, 8), false, 4500.00m, 4, "2.5" },
                    { 6, 3, 3, new DateOnly(2026, 4, 1), new DateOnly(2027, 4, 1), new DateOnly(2026, 5, 1), false, 3500.00m, 2, "4.2" }
                });

            migrationBuilder.InsertData(
                table: "Discounts",
                columns: new[] { "DiscountId", "DateFrom", "DateTo", "DiscountName", "Offer", "Percentage", "SoftwareId" },
                values: new object[,]
                {
                    { 1, new DateOnly(2026, 5, 1), new DateOnly(2026, 8, 31), "Letnia Promocja ERP", "Subscription", 10.00m, 1 },
                    { 2, new DateOnly(2025, 12, 1), new DateOnly(2026, 2, 28), "Zimowa Promocja Edu", "Contract", 15.00m, 2 },
                    { 3, new DateOnly(2026, 3, 1), new DateOnly(2026, 6, 30), "Wiosenny CRM", "Contract", 5.00m, 3 },
                    { 4, new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), "Cloud Start Bonus", "Subscription", 20.00m, 4 },
                    { 5, new DateOnly(2026, 6, 1), new DateOnly(2026, 7, 15), "HR New Release", "Contract", 12.00m, 5 },
                    { 6, new DateOnly(2026, 6, 10), new DateOnly(2026, 6, 20), "AI Launch Promo", "Subscription", 25.00m, 6 }
                });

            migrationBuilder.InsertData(
                table: "Subscriptions",
                columns: new[] { "SubscriptionId", "ClientId", "IsActive", "RenewalAmount", "RenewalPeriod", "SoftwareId", "StartDate", "SubscriptionName" },
                values: new object[,]
                {
                    { 1, 1, true, 200.00m, 1, 2, new DateOnly(2026, 4, 15), "EduLearn Premium" },
                    { 2, 3, true, 500.00m, 1, 4, new DateOnly(2026, 5, 1), "CloudDrive Business" },
                    { 3, 4, true, 1500.00m, 3, 6, new DateOnly(2026, 6, 11), "FinAnalyse Enterprise" },
                    { 4, 5, true, 300.00m, 1, 1, new DateOnly(2026, 3, 10), "ERP Lite Client" },
                    { 5, 2, false, 450.00m, 3, 2, new DateOnly(2025, 9, 1), "EduLearn Basic" },
                    { 6, 6, false, 100.00m, 1, 3, new DateOnly(2026, 1, 1), "SecureVault Promo" }
                });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "PaymentId", "Amount", "ContractId", "IsRefunded", "PaymentDate" },
                values: new object[,]
                {
                    { 1, 5000.00m, 1, false, new DateOnly(2026, 1, 20) },
                    { 2, 4000.00m, 2, false, new DateOnly(2026, 2, 15) },
                    { 3, 4000.00m, 2, false, new DateOnly(2026, 2, 20) },
                    { 4, 3000.00m, 3, false, new DateOnly(2026, 5, 5) },
                    { 5, 5000.00m, 1, true, new DateOnly(2026, 1, 21) },
                    { 6, 1500.00m, 3, true, new DateOnly(2026, 5, 6) }
                });

            migrationBuilder.InsertData(
                table: "SubscriptionRenewals",
                columns: new[] { "RenewalId", "AmountPaid", "PaymentDate", "PeriodEnd", "PeriodStart", "SubscriptionId" },
                values: new object[,]
                {
                    { 1, 190.00m, new DateOnly(2026, 4, 15), new DateOnly(2026, 5, 15), new DateOnly(2026, 4, 15), 1 },
                    { 2, 190.00m, new DateOnly(2026, 5, 14), new DateOnly(2026, 6, 15), new DateOnly(2026, 5, 15), 1 },
                    { 3, 500.00m, new DateOnly(2026, 5, 1), new DateOnly(2026, 6, 1), new DateOnly(2026, 5, 1), 2 },
                    { 4, 500.00m, new DateOnly(2026, 5, 30), new DateOnly(2026, 7, 1), new DateOnly(2026, 6, 1), 2 },
                    { 5, 1125.00m, new DateOnly(2026, 6, 11), new DateOnly(2026, 9, 11), new DateOnly(2026, 6, 11), 3 },
                    { 6, 300.00m, new DateOnly(2026, 5, 10), new DateOnly(2026, 6, 10), new DateOnly(2026, 5, 10), 4 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Contracts",
                keyColumn: "ContractId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Contracts",
                keyColumn: "ContractId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Contracts",
                keyColumn: "ContractId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Discounts",
                keyColumn: "DiscountId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Discounts",
                keyColumn: "DiscountId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Discounts",
                keyColumn: "DiscountId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Discounts",
                keyColumn: "DiscountId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Discounts",
                keyColumn: "DiscountId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Discounts",
                keyColumn: "DiscountId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "PaymentId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SubscriptionRenewals",
                keyColumn: "RenewalId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SubscriptionRenewals",
                keyColumn: "RenewalId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SubscriptionRenewals",
                keyColumn: "RenewalId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SubscriptionRenewals",
                keyColumn: "RenewalId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SubscriptionRenewals",
                keyColumn: "RenewalId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SubscriptionRenewals",
                keyColumn: "RenewalId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Subscriptions",
                keyColumn: "SubscriptionId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Subscriptions",
                keyColumn: "SubscriptionId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "ClientId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "ClientId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Contracts",
                keyColumn: "ContractId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Contracts",
                keyColumn: "ContractId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Contracts",
                keyColumn: "ContractId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Subscriptions",
                keyColumn: "SubscriptionId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Subscriptions",
                keyColumn: "SubscriptionId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Subscriptions",
                keyColumn: "SubscriptionId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Subscriptions",
                keyColumn: "SubscriptionId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "ClientId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "ClientId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "ClientId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "ClientId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Software",
                keyColumn: "SoftwareId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Software",
                keyColumn: "SoftwareId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Software",
                keyColumn: "SoftwareId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Software",
                keyColumn: "SoftwareId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Software",
                keyColumn: "SoftwareId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Software",
                keyColumn: "SoftwareId",
                keyValue: 6);
        }
    }
}
