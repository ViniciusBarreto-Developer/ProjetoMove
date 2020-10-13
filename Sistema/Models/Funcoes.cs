using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;

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
}