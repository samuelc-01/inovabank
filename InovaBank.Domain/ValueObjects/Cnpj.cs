using System.Text.RegularExpressions;

namespace InovaBank.Domain.ValueObjects;

/// <summary>
/// Referência técnica para algoritmo de validação de CNPJ: https://www.macoratti.net/alg_cnpj.htm
/// </summary>
public sealed record Cnpj
{
    public string Number { get; }

    public Cnpj(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("CNPJ não pode ser vazio.");

        var cleanedNumber = Clean(number);

        if (!IsValid(cleanedNumber))
            throw new ArgumentException("CNPJ inválido.");

        Number = cleanedNumber;
    }

    private static string Clean(string number) =>
        Regex.Replace(number, @"[^\d]", "");

    public static bool IsValid(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj))
            return false;

        cnpj = Clean(cnpj);

        if (cnpj.Length != 14) return false;

        if (new string(cnpj[0], 14) == cnpj) return false;

        int[] multiplier1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        int[] multiplier2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        string tempCnpj = cnpj[..12];
        int sum = 0;

        for (int i = 0; i < 12; i++)
            sum += (tempCnpj[i] - '0') * multiplier1[i];

        int remainder = sum % 11;
        int digit1 = remainder < 2 ? 0 : 11 - remainder;

        tempCnpj += digit1;
        sum = 0;

        for (int i = 0; i < 13; i++)
            sum += (tempCnpj[i] - '0') * multiplier2[i];

        remainder = sum % 11;
        int digit2 = remainder < 2 ? 0 : 11 - remainder;

        return cnpj.EndsWith($"{digit1}{digit2}");
    }

    public static implicit operator string(Cnpj cnpj) => cnpj.Number;
    public override string ToString() => Number;
}
