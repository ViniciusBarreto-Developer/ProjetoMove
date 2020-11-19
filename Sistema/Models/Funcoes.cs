using Newtonsoft.Json;
using Sistema.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

public class Funcoes
{
    /// <summary>
    /// Gera Hash SHA1, SHA256, SHA512
    /// </summary>
    /// <param name="texto"></param>
    /// <param name="nomeHash"></param>
    /// <returns></returns>
    public static string HashTexto(string texto, string nomeHash)
    {
        HashAlgorithm algoritmo = HashAlgorithm.Create(nomeHash);
        if (algoritmo == null)
        {
            throw new ArgumentException("Nome de hash incorreto", "nomeHash");
        }
        byte[] hash = algoritmo.ComputeHash(Encoding.UTF8.GetBytes(texto));
        return Convert.ToBase64String(hash);

    }
    public static string EnviarEmail(string emailDestinatario, string assunto, string corpomsg)
    {
        try
        {
            //Cria o endereço de email do remetente
            MailAddress de = new MailAddress("Fatec ADS <fatecgtaads@gmail.com>");
            //Cria o endereço de email do destinatário -->
            MailAddress para = new MailAddress(emailDestinatario);
            MailMessage mensagem = new MailMessage(de, para);
            mensagem.IsBodyHtml = true;
            //Assunto do email
            mensagem.Subject = assunto;
            //Conteúdo do email
            mensagem.Body = corpomsg;
            //Prioridade E-mail
            mensagem.Priority = MailPriority.Normal;
            //Cria o objeto que envia o e-mail
            SmtpClient cliente = new SmtpClient();
            //Envia o email
            cliente.Send(mensagem);
            return "success|E-mail enviado com sucesso";
        }
        catch { return "error|Erro ao enviar e-mail"; }
    }
    public static string Codifica(string texto)
    {
        byte[] stringBase64 = new byte[texto.Length];
        stringBase64 = Encoding.UTF8.GetBytes(texto);
        string codifica = Convert.ToBase64String(stringBase64);
        return codifica;
    }
    public static string Decodifica(string texto)
    {
        var encode = new UTF8Encoding();
        var utf8Decode = encode.GetDecoder();
        byte[] stringValor = Convert.FromBase64String(texto);
        int contador = utf8Decode.GetCharCount(stringValor, 0,
        stringValor.Length);
        char[] decodeChar = new char[contador];
        utf8Decode.GetChars(stringValor, 0, stringValor.Length, decodeChar, 0);
        string resultado = new String(decodeChar);
        return resultado;
    }
    public static CaptchaResponse ValidateCaptcha(string response)
    {
        string secret = WebConfigurationManager.AppSettings["recaptchaPrivateKey"];
        var client = new WebClient();
        var jsonResult = client.DownloadString(
            string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}",
            secret, response));
        return JsonConvert.DeserializeObject<CaptchaResponse>(jsonResult.ToString());

        //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
    }
    public static bool ValidateCPF(string cpfUsu)
    {
        int soma = 0, mult = 10;

        string cpf = Regex.Replace(cpfUsu, @"[^0-9a-zA-Z]+", "");

        for (int i = 0; i < cpf.Length - 2; i++)
        {
            soma += (Convert.ToInt32(cpf[i]) - 48) * mult;
            mult--;
        }
        if (soma % 11 == 0 || soma % 11 == 1)
        {
            if (Convert.ToInt32(cpf[9]) - 48 != 0)
            {
                return false;
            }
        }
        else
        {
            if (Convert.ToInt32(cpf[9]) - 48 != (11 - (soma % 11)))
            {
                return false;
            }
        }
        soma = 0;
        mult = 11;
        for (int i = 0; i < cpf.Length - 1; i++)
        {
            soma += (Convert.ToInt32(cpf[i]) - 48) * mult;
            mult--;
        }
        if (soma % 11 == 0 || soma % 11 == 1)
        {
            if (Convert.ToInt32(cpf[10]) - 48 != 0)
            {
                return false;
            }
        }
        else
        {
            if (Convert.ToInt32(cpf[10]) - 48 != (11 - (soma % 11)))
            {
                return false;
            }
        }
        return true;
    }
    public class Upload
    {
        public static bool CriarDiretorio()
        {
            string dir = HttpContext.Current.Request.PhysicalApplicationPath + "Uploads\\";
            if (!Directory.Exists(dir))
            {
                //Caso não exista devermos criar
                Directory.CreateDirectory(dir);
                return true;
            }
            else
                return false;
        }
        public static bool ExcluirArquivo(string arq)
        {
            if (File.Exists(arq))
            {
                File.Delete(arq);
                return true;
            }
            else
                return false;
        }
        public static string UploadImagem(HttpPostedFileBase flpUpload, string nome)
        {
            try
            {
                double permitido = 900;
                if (flpUpload != null)
                {
                    string arq = Path.GetFileName(flpUpload.FileName);
                    double tamanho = Convert.ToDouble(flpUpload.ContentLength) / 1024;
                    string extensao = Path.GetExtension(flpUpload.FileName).ToLower();
                    string diretorio = HttpContext.Current.Request.PhysicalApplicationPath + "Uploads\\" + nome;
                    if (tamanho > permitido)
                        return "O tamanho máximo permitido é de " + permitido + " kb!";
                    else if ((extensao != ".png" && extensao != ".jpg" && extensao != ".jpeg"))
                        return "Extensão inválida, só são permitidas .png, .jpg e .jpeg!";
                    else
                    {
                        if (!File.Exists(diretorio))
                        {
                            flpUpload.SaveAs(diretorio);
                            return "sucesso";
                        }
                        else
                            return "Já existe um arquivo com esse nome!";
                    }
                }
                else
                    return "Erro no Upload!";
            }
            catch { return "Erro no Upload"; }
        }
        public static string UploadPdf(HttpPostedFileBase flpUpload, string nome)
        {
            try
            {
                double permitido = 10000;
                if (flpUpload != null)
                {
                    string arq = Path.GetFileName(flpUpload.FileName);
                    double tamanho = Convert.ToDouble(flpUpload.ContentLength) / 1024;
                    string extensao = Path.GetExtension(flpUpload.FileName).ToLower();
                    string diretorio = HttpContext.Current.Request.PhysicalApplicationPath + "Uploads\\" + nome;
                    if (tamanho > permitido)
                        return "O tamanho máximo permitido é de " + permitido + " kb!";
                    else if (extensao != ".pdf" )
                        return "Extensão inválida, só é permitido .pdf";
                    else
                    {
                        if (!File.Exists(diretorio))
                        {
                            flpUpload.SaveAs(diretorio);
                            return "sucesso";
                        }
                        else
                            return "Já existe um arquivo com esse nome!";
                    }
                }
                else
                    return "Erro no Upload!";
            }
            catch { return "Erro no Upload"; }
        }
    }
}