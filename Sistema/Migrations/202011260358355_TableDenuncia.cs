namespace Sistema.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TableDenuncia : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.den_denuncias",
                c => new
                {
                    den_codigo = c.Int(nullable: false, identity: true),
                    den_dataCadastro = c.String(nullable: false, unicode: false),
                    den_motivo = c.String(nullable: false, unicode: false),
                    den_status = c.String(nullable: false, unicode: false),
                    den_usuarioDenuncianteId = c.Int(nullable: false),
                    den_usuarioDenunciadoId = c.Int(nullable: false),
                    den_projetoDenunciadoId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.den_codigo)
                .ForeignKey("dbo.pro_projeto", t => t.den_projetoDenunciadoId, cascadeDelete: true)
                .ForeignKey("dbo.usu_usuario", t => t.den_usuarioDenunciadoId, cascadeDelete: true)
                .ForeignKey("dbo.usu_usuario", t => t.den_usuarioDenuncianteId, cascadeDelete: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.den_denuncias", "den_usuarioDenuncianteId", "dbo.usu_usuario");
            DropForeignKey("dbo.den_denuncias", "den_usuarioDenunciadoId", "dbo.usu_usuario");
            DropForeignKey("dbo.den_denuncias", "den_projetoDenunciadoId", "dbo.pro_projeto");
            DropTable("dbo.den_denuncias");
        }
    }
}
