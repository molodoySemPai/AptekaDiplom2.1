using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AptekaDiplom22.Migrations
{
    /// <inheritdoc />
    public partial class SyncChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Pharmacies",
                keyColumn: "Id",
                keyValue: 1,
                column: "WorkingHours",
                value: "08:00-22:00");

            migrationBuilder.UpdateData(
                table: "Pharmacies",
                keyColumn: "Id",
                keyValue: 2,
                column: "WorkingHours",
                value: "09:00-21:00");

            migrationBuilder.InsertData(
                table: "Pharmacies",
                columns: new[] { "Id", "Address", "Name", "Phone", "WorkingHours" },
                values: new object[] { 3, "ул. Садовая, 7", "Аптека №3 Заречная", "+79000000003", "08:00-20:00" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: "Жаропонижающее и обезболивающее средство");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: "Обезболивающее средство");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: "Витаминный комплекс для иммунитета");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "ActiveIngredient", "Description", "IsPrescriptionRequired", "Manufacturer", "Name", "Price" },
                values: new object[,]
                {
                    { 4, "Парацетамол", "Жаропонижающее средство", false, "Фармстандарт", "Парацетамол", 60m },
                    { 5, "Амоксициллин", "Антибиотик широкого спектра действия", true, "Sandoz", "Амоксициллин", 220m },
                    { 6, "Ибупрофен", "Противовоспалительное и обезболивающее средство", false, "Reckitt Benckiser", "Нурофен", 180m },
                    { 7, "Омепразол", "Средство для лечения язвенной болезни и гастрита", true, "Sandoz", "Омепразол", 130m },
                    { 8, "Лоратадин", "Противоаллергическое средство", false, "Evalar", "Лоратадин", 90m }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "FullName", "PasswordHash", "Phone", "Role" },
                values: new object[] { 1, "admin@apteka.ru", "Администратор", "$2a$11$0wAaiL2YzKzhuKZTQ4mYye5C6CW2/sFAJh8dCZAVUbm1bz3lkz4cu", "+79990000000", "Admin" });

            migrationBuilder.InsertData(
                table: "Stocks",
                columns: new[] { "Id", "PharmacyId", "ProductId", "Quantity", "ReservedQuantity" },
                values: new object[,]
                {
                    { 5, 1, 4, 80, 0 },
                    { 6, 3, 4, 60, 0 },
                    { 7, 1, 5, 25, 0 },
                    { 8, 2, 5, 10, 0 },
                    { 9, 2, 6, 70, 0 },
                    { 10, 3, 6, 40, 0 },
                    { 11, 1, 7, 15, 0 },
                    { 12, 3, 8, 55, 0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Stocks",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Pharmacies",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.UpdateData(
                table: "Pharmacies",
                keyColumn: "Id",
                keyValue: 1,
                column: "WorkingHours",
                value: null);

            migrationBuilder.UpdateData(
                table: "Pharmacies",
                keyColumn: "Id",
                keyValue: 2,
                column: "WorkingHours",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "Description",
                value: null);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "Description",
                value: null);
        }
    }
}
