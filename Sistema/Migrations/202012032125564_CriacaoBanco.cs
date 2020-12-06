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
                    arp_nome = c.String(nullable: false, unicode: false),
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
                    pro_descricao = c.String(nullable: false, maxLength: 500, storeType: "nvarchar"),
                    pro_logo = c.String(nullable: false, unicode: false),
                    pro_dataCadastro = c.DateTime(nullable: false, precision: 0),
                    pro_ativo = c.Boolean(nullable: false),
                    pro_punicao = c.DateTime(precision: 0),
                    pro_inativo = c.String(unicode: false),
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
                    inp_ativo = c.Boolean(nullable: false),
                    inp_inativo = c.String(unicode: false),
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
                    usu_dataNascimento = c.String(nullable: false, unicode: false),
                    usu_cpf = c.String(nullable: false, maxLength: 14, storeType: "nvarchar"),
                    usu_celular = c.String(maxLength: 15, storeType: "nvarchar"),
                    usu_email = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                    usu_emailRecuperacao = c.String(nullable: false, maxLength: 100, storeType: "nvarchar"),
                    usu_senha = c.String(nullable: false, unicode: false),
                    usu_foto = c.String(unicode: false),
                    usu_biografia = c.String(unicode: false),
                    usu_ativo = c.Boolean(nullable: false),
                    Hash = c.String(unicode: false),
                    usu_adm = c.Boolean(nullable: false),
                    usu_punicao = c.DateTime(precision: 0),
                    usu_inativo = c.String(unicode: false),
                })
                .PrimaryKey(t => t.usu_codigo);

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
                    tag_pesquisada = c.Int(),
                })
                .PrimaryKey(t => t.tag_codigo);

            CreateTable(
                "dbo.den_denuncias",
                c => new
                {
                    den_codigo = c.Int(nullable: false, identity: true),
                    den_dataCadastro = c.String(nullable: false, unicode: false),
                    den_motivo = c.String(nullable: false, unicode: false),
                    den_observacao = c.String(unicode: false),
                    den_status = c.String(nullable: false, unicode: false),
                    den_punicao = c.Int(),
                    den_dataPunicao = c.DateTime(precision: 0),
                    den_motivoPunicao = c.String(unicode: false),
                    den_usuarioDenuncianteId = c.Int(nullable: false),
                    den_usuarioDenunciadoId = c.Int(),
                    den_projetoDenunciadoId = c.Int(),
                    den_admId = c.Int(),
                })
                .PrimaryKey(t => t.den_codigo)
                .ForeignKey("dbo.usu_usuario", t => t.den_admId)
                .ForeignKey("dbo.pro_projeto", t => t.den_projetoDenunciadoId)
                .ForeignKey("dbo.usu_usuario", t => t.den_usuarioDenunciadoId)
                .ForeignKey("dbo.usu_usuario", t => t.den_usuarioDenuncianteId, cascadeDelete: true);

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
            DropForeignKey("dbo.prs_projetoSalvos", "usu_codigo", "dbo.usu_usuario");
            DropForeignKey("dbo.prs_projetoSalvos", "pro_codigo", "dbo.pro_projeto");
            DropForeignKey("dbo.den_denuncias", "den_usuarioDenuncianteId", "dbo.usu_usuario");
            DropForeignKey("dbo.den_denuncias", "den_usuarioDenunciadoId", "dbo.usu_usuario");
            DropForeignKey("dbo.den_denuncias", "den_projetoDenunciadoId", "dbo.pro_projeto");
            DropForeignKey("dbo.den_denuncias", "den_admId", "dbo.usu_usuario");
            DropForeignKey("dbo.prt_projetoTags", "tag_codigo", "dbo.tag_tag");
            DropForeignKey("dbo.prt_projetoTags", "pro_codigo", "dbo.pro_projeto");
            DropForeignKey("dbo.inp_integrantesProjeto", "usu_codigo", "dbo.usu_usuario");
            DropForeignKey("dbo.inp_integrantesProjeto", "pro_codigo", "dbo.pro_projeto");
            DropForeignKey("dbo.arp_arquivosProjeto", "pro_codigo", "dbo.pro_projeto");
            DropTable("dbo.ust_usuarioTag");
            DropTable("dbo.prs_projetoSalvos");
            DropTable("dbo.den_denuncias");
            DropTable("dbo.tag_tag");
            DropTable("dbo.prt_projetoTags");
            DropTable("dbo.usu_usuario");
            DropTable("dbo.inp_integrantesProjeto");
            DropTable("dbo.pro_projeto");
            DropTable("dbo.arp_arquivosProjeto");
        }
    }
}
