namespace Sistema.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Sistema.Models.Contexto>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
        }

        protected override void Seed(Sistema.Models.Contexto context)
        {
            context.Tag.AddOrUpdate(t => t.Nome,
            new Models.Tag { Id = 1, Nome = "Tecnologia" },
            new Models.Tag { Id = 2, Nome = "Saúde" },
            new Models.Tag { Id = 3, Nome = "Beleza" },
            new Models.Tag { Id = 4, Nome = "Esporte" },
            new Models.Tag { Id = 5, Nome = "Finança" },
            new Models.Tag { Id = 6, Nome = "Música" });
        }
    }
}
