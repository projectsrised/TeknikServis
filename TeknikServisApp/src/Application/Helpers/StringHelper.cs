namespace TeknikServisApp.Application.Helpers;

public static class StringHelper
{
    /// <summary>
    /// İki string arasındaki benzerlik oranını hesaplar (0-1 arası)
    /// Levenshtein distance kullanır
    /// </summary>
    public static double CalculateSimilarity(string source, string target)
    {
        if (string.IsNullOrEmpty(source) && string.IsNullOrEmpty(target))
            return 1.0;

        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            return 0.0;

        // Normalize: küçük harf, trim, türkçe karakterleri dönüştür
        source = NormalizeString(source);
        target = NormalizeString(target);

        // Eğer aynıysa %100 benzer
        if (source == target)
            return 1.0;

        // Biri diğerini içeriyorsa yüksek benzerlik
        if (source.Contains(target) || target.Contains(source))
        {
            var shorter = Math.Min(source.Length, target.Length);
            var longer = Math.Max(source.Length, target.Length);
            return (double)shorter / longer;
        }

        int distance = LevenshteinDistance(source, target);
        int maxLength = Math.Max(source.Length, target.Length);

        return 1.0 - ((double)distance / maxLength);
    }

    /// <summary>
    /// String'i normalize eder (küçük harf, trim, türkçe karakter dönüşümü)
    /// </summary>
    public static string NormalizeString(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        input = input.Trim().ToLowerInvariant();

        // Türkçe karakter dönüşümü
        input = input
            .Replace('ı', 'i')
            .Replace('ğ', 'g')
            .Replace('ü', 'u')
            .Replace('ş', 's')
            .Replace('ö', 'o')
            .Replace('ç', 'c')
            .Replace('İ', 'i')
            .Replace('Ğ', 'g')
            .Replace('Ü', 'u')
            .Replace('Ş', 's')
            .Replace('Ö', 'o')
            .Replace('Ç', 'c');

        return input;
    }

    /// <summary>
    /// Levenshtein mesafesi hesaplar
    /// </summary>
    private static int LevenshteinDistance(string source, string target)
    {
        int sourceLength = source.Length;
        int targetLength = target.Length;

        var matrix = new int[sourceLength + 1, targetLength + 1];

        for (int i = 0; i <= sourceLength; i++)
            matrix[i, 0] = i;

        for (int j = 0; j <= targetLength; j++)
            matrix[0, j] = j;

        for (int i = 1; i <= sourceLength; i++)
        {
            for (int j = 1; j <= targetLength; j++)
            {
                int cost = target[j - 1] == source[i - 1] ? 0 : 1;

                matrix[i, j] = Math.Min(
                    Math.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }

        return matrix[sourceLength, targetLength];
    }

    /// <summary>
    /// İki string'in belirli bir oranda benzer olup olmadığını kontrol eder
    /// </summary>
    public static bool IsSimilar(string source, string target, double threshold = 0.7)
    {
        return CalculateSimilarity(source, target) >= threshold;
    }
}
