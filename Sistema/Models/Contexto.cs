using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Sistema.Models
{
    public class Contexto : DbContext
    {
        public Contexto() : base(nameOrConnectionString: "StringConexao") { }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Projeto> Projeto { get; set; }
        public DbSet<Tag> Tag { get; set; }
        public DbSet<ArquivosProjeto> ArquivosProjeto { get; set; }
        public DbSet<IntegrantesProjeto> IntegrantesProjeto { get; set; }
        public DbSet<ProjetoTags> ProjetoTags { get; set; }
        public DbSet<ProjetosSalvos> ProjetosSalvos { get; set; }
        public DbSet<UsuarioTag> UsuarioTag { get; set; }
        public DbSet<Denuncias> Denuncias { get; set; }
        protected override void OnModelCreating(DbModelBuilder mb)
        {
            var usu = mb.Entity<Usuario>();
            usu.ToTable("usu_usuario");
            usu.Property(x => x.Id).HasColumnName("usu_codigo");
            usu.Property(x => x.Nome).HasColumnName("usu_nome");            
            usu.Property(x => x.DataNascimento).HasColumnName("usu_dataNascimento");
            usu.Property(x => x.Cpf).HasColumnName("usu_cpf");
            usu.Property(x => x.Celular).HasColumnName("usu_celular");
            usu.Property(x => x.Foto).HasColumnName("usu_foto");
            usu.Property(x => x.Email).HasColumnName("usu_email");
            usu.Property(x => x.EmailRecuperacao).HasColumnName("usu_emailRecuperacao");
            usu.Property(x => x.Senha).HasColumnName("usu_senha");
            usu.Property(x => x.Biografia).HasColumnName("usu_biografia");
            usu.Property(x => x.Ativo).HasColumnName("usu_ativo");
            usu.Property(x => x.Adm).HasColumnName("usu_adm");
            usu.Property(x => x.Punicao).HasColumnName("usu_punicao");
            usu.Property(x => x.Inativo).HasColumnName("usu_inativo");

            var pro = mb.Entity<Projeto>();
            pro.ToTable("pro_projeto");
            pro.Property(x => x.Id).HasColumnName("pro_codigo");
            pro.Property(x => x.Nome).HasColumnName("pro_nome");
            pro.Property(x => x.Descricao).HasColumnName("pro_descricao");
            pro.Property(x => x.Logo).HasColumnName("pro_logo");
            pro.Property(x => x.DataCadastro).HasColumnName("pro_dataCadastro");
            pro.Property(x => x.Ativo).HasColumnName("pro_ativo");
            pro.Property(x => x.Punicao).HasColumnName("pro_punicao");
            pro.Property(x => x.Inativo).HasColumnName("pro_inativo");

            var tag = mb.Entity<Tag>();
            tag.ToTable("tag_tag");
            tag.Property(x => x.Id).HasColumnName("tag_codigo");
            tag.Property(x => x.Nome).HasColumnName("tag_nome");
            tag.Property(x => x.Pesquisada).HasColumnName("tag_pesquisada");

            var arp = mb.Entity<ArquivosProjeto>();
            arp.ToTable("arp_arquivosProjeto");
            arp.Property(x => x.Id).HasColumnName("arp_codigo");
            arp.Property(x => x.Arquivo).HasColumnName("arp_arquivo");
            arp.Property(x => x.Nome).HasColumnName("arp_nome");
            arp.Property(x => x.ProjetoId).HasColumnName("pro_codigo");

            var inp = mb.Entity<IntegrantesProjeto>();
            inp.ToTable("inp_integrantesProjeto");
            inp.Property(x => x.Id).HasColumnName("inp_codigo");
            inp.Property(x => x.ProjetoId).HasColumnName("pro_codigo");
            inp.Property(x => x.UsuarioID).HasColumnName("usu_codigo");
            inp.Property(x => x.Adm).HasColumnName("inp_adm");
            inp.Property(x => x.Ativo).HasColumnName("inp_ativo");
            inp.Property(x => x.Inativo).HasColumnName("inp_inativo");

            var prt = mb.Entity<ProjetoTags>();
            prt.ToTable("prt_projetoTags");
            prt.Property(x => x.Id).HasColumnName("prt_codigo");
            prt.Property(x => x.ProjetoId).HasColumnName("pro_codigo");
            prt.Property(x => x.TagId).HasColumnName("tag_codigo");

            var prs = mb.Entity<ProjetosSalvos>();
            prs.ToTable("prs_projetoSalvos");
            prs.Property(x => x.Id).HasColumnName("prs_codigo");
            prs.Property(x => x.ProjetoId).HasColumnName("pro_codigo");
            prs.Property(x => x.UsuarioId).HasColumnName("usu_codigo");

            var ust = mb.Entity<UsuarioTag>();
            ust.ToTable("ust_usuarioTag");
            ust.Property(x => x.Id).HasColumnName("ust_codigo");
            ust.Property(x => x.TagId).HasColumnName("tag_codigo");
            ust.Property(x => x.UsuarioId).HasColumnName("usu_codigo");

            var den = mb.Entity<Denuncias>();
            den.ToTable("den_denuncias");
            den.Property(x => x.Id).HasColumnName("den_codigo");
            den.Property(x => x.DataCadastro).HasColumnName("den_dataCadastro");
            den.Property(x => x.Motivo).HasColumnName("den_motivo");
            den.Property(x => x.Observacao).HasColumnName("den_observacao");
            den.Property(x => x.Status).HasColumnName("den_status");
            den.Property(x => x.Punicao).HasColumnName("den_punicao");
            den.Property(x => x.Desativado).HasColumnName("den_desativado");
            den.Property(x => x.DataPunicao).HasColumnName("den_dataPunicao");
            den.Property(x => x.MotivoPunicao).HasColumnName("den_motivoPunicao");
            den.Property(x => x.AdmId).HasColumnName("den_admId");
            den.Property(x => x.UsuarioDenuncianteId).HasColumnName("den_usuarioDenuncianteId");
            den.Property(x => x.UsuarioDenunciadoId).HasColumnName("den_usuarioDenunciadoId");
            den.Property(x => x.ProjetoDenunciadoId).HasColumnName("den_projetoDenunciadoId");

        }
        
    }

}