using System;
using FluentMigrator;

namespace TysonFury.Migrations.Initial
{
    [Migration(version: 1, transactionBehavior: TransactionBehavior.Default, description: "Создание схемы и таблиц для Quartz.")]
    public class InitialMigration : Migration
    {
        public override void Up()
        {
            Console.WriteLine("Жесть!");
            Create.Schema("scheduler");
            Execute.EmbeddedScript("TysonFury.Migrations.Initial.initial.sql");
        }

        public override void Down()
        {
        }
    }
}
