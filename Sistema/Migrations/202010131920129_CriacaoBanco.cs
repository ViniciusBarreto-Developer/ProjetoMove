namespace Sistema.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CriacaoBanco : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.arp_arquivosProjeto",
                c => new
                    {
                        arp_codigo = c.Int(nullable: false, identity: true),
                        arp_arquivo = c.String(nullable: false, unicode: false),
                        arp_tipo = c.Int(nullable: false),
                        pro_codigo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.arp_codigo)
                .ForeignKey("dbo.pro_projeto", t => t.pro_codigo, cascadeDelete: true);
            
            CreateTable(
                "dbo.pro_projeto",
                c => new
                    {
                        pro_codigo = c.Int(nullable: false, identity: true),
                        pro_nome = c.String(nullable: false, maxLength: 50, storeType: "nvarchar"),
                        pro_descricao = c.String(maxLength: 500, storeType: "nvarchar"),
                        pro_logo = c.String(unicode: false),
                        pro_dataCadastro = c.DateTime(nullable: false, precision: 0),
                    })
                .PrimaryKey(t => t.pro_codigo);
            
            CreateTable(
                "dbo.inp_integrantesProjeto",
                c => new
                    {
                        inp_codigo = c.Int(nullable: false, identity: true),
                        usu_codigo = c.Int(nullable: false),
                        pro_codigo = c.Int(nullable: false),
                        inp_adm = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.inp_codigo)
                .ForeignKey("dbo.pro_projeto", t => t.pro_codigo, cascadeDelete: true)
                .ForeignKey("dbo.usu_usuario", t => t.usu_codigo, cascadeDelete: true);
            
            CreateTable(
                "dbo.usu_usuario",
                c => new
                    {
                        usu_codigo = c.Int(nullable: false, identity: true),
                        usu_nome = c.String(nullable: false, maxLength: 200, storeType: "nvarchar"),
                        usu_nomeSocial = c.String(maxLength: 200, storeType: "nvarchar"),
                        usu_dataNascimento = c.DateTime(nullable: false, precision: 0),
                        usu_cpf = c.Long(nullable: false),
                        usu_email = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        usu_emailRecuperacao = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                        usu_senha = c.String(nullable: false, unicode: false),
                        usu_foto = c.String(unicode: false),
                        usu_biografia = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.usu_codigo);
            
            CreateTable(
                "dbo.prs_projetoSalvos",
                c => new
                    {
                        prs_codigo = c.Int(nullable: false, identity: true),
                        pro_codigo = c.Int(nullable: false),
                        usu_codigo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.prs_codigo)
                .ForeignKey("dbo.pro_projeto", t => t.pro_codigo, cascadeDelete: true)
                .ForeignKey("dbo.usu_usuario", t => t.usu_codigo, cascadeDelete: true);
            
            CreateTable(
                "dbo.prt_projetoTags",
                c => new
                    {
                        prt_codigo = c.Int(nullable: false, identity: true),
                        pro_codigo = c.Int(nullable: false),
                        tag_codigo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.prt_codigo)
                .ForeignKey("dbo.pro_projeto", t => t.pro_codigo, cascadeDelete: true)
                .ForeignKey("dbo.tag_tag", t => t.tag_codigo, cascadeDelete: true);
            
            CreateTable(
                "dbo.tag_tag",
                c => new
                    {
                        tag_codigo = c.Int(nullable: false, identity: true),
                        tag_nome = c.String(nullable: false, unicode: false),
                    })
                .PrimaryKey(t => t.tag_codigo);
            
            CreateTable(
                "dbo.ust_usuarioTag",
                c => new
                    {
                        ust_codigo = c.Int(nullable: false, identity: true),
                        tag_codigo = c.Int(nullable: false),
                        usu_codigo = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ust_codigo)
                .ForeignKey("dbo.tag_tag", t => t.tag_codigo, cascadeDelete: true)
                .ForeignKey("dbo.usu_usuario", t => t.usu_codigo, cascadeDelete: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ust_usuarioTag", "usu_codigo", "dbo.usu_usuario");
            DropForeignKey("dbo.ust_usuarioTag", "tag_codigo", "dbo.tag_tag");
            DropForeignKey("dbo.prt_projetoTags", "tag_codigo", "dbo.tag_tag");
            DropForeignKey("dbo.prt_projetoTags", "pro_codigo", "dbo.pro_projeto");
            DropForeignKey("dbo.prs_projetoSalvos", "usu_codigo", "dbo.usu_usuario");
            DropForeignKey("dbo.prs_projetoSalvos", "pro_codigo", "dbo.pro_projeto");
            DropForeignKey("dbo.inp_integrantesProjeto", "usu_codigo", "dbo.usu_usuario");
            DropForeignKey("dbo.inp_integrantesProjeto", "pro_codigo", "dbo.pro_projeto");
            DropForeignKey("dbo.arp_arquivosProjeto", "pro_codigo", "dbo.pro_projeto");
            DropIndex("dbo.ust_usuarioTag", new[] { "usu_codigo" });
            DropIndex("dbo.ust_usuarioTag", new[] { "tag_codigo" });
            DropIndex("dbo.prt_projetoTags", new[] { "tag_codigo" });
            DropIndex("dbo.prt_projetoTags", new[] { "pro_codigo" });
            DropIndex("dbo.prs_projetoSalvos", new[] { "usu_codigo" });
            DropIndex("dbo.prs_projetoSalvos", new[] { "pro_codigo" });
            DropIndex("dbo.inp_integrantesProjeto", new[] { "pro_codigo" });
            DropIndex("dbo.inp_integrantesProjeto", new[] { "usu_codigo" });
            DropIndex("dbo.arp_arquivosProjeto", new[] { "pro_codigo" });
            DropTable("dbo.ust_usuarioTag");
            DropTable("dbo.tag_tag");
            DropTable("dbo.prt_projetoTags");
            DropTable("dbo.prs_projetoSalvos");
            DropTable("dbo.usu_usuario");
            DropTable("dbo.inp_integrantesProjeto");
            DropTable("dbo.pro_projeto");
            DropTable("dbo.arp_arquivosProjeto");
        }
    }
}
