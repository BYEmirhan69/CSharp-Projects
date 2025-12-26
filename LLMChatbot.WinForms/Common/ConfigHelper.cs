namespace LLMChatbot.WinForms.Common;

/// <summary>
/// Yapılandırma ve ortam değişkenlerini yöneten yardımcı sınıf.
/// API anahtarları ve model ayarları buradan okunur.
/// </summary>
public static class ConfigHelper
{
    /// <summary>
    /// OpenAI API Key için ortam değişkeni adı
    /// </summary>
    private const string ApiKeyEnvVar = "OPENAI_API_KEY";

    /// <summary>
    /// OpenAI Responses API endpoint
    /// </summary>
    public const string ResponsesApiEndpoint = "https://api.openai.com/v1/responses";

    /// <summary>
    /// Varsayılan model
    /// </summary>
    private static string _model = "gpt-4.1";

    /// <summary>
    /// Kullanılacak model (kolayca değiştirilebilir)
    /// </summary>
    public static string Model
    {
        get => _model;
        set => _model = value;
    }

    /// <summary>
    /// Ortam değişkeninden OpenAI API anahtarını okur
    /// </summary>
    /// <returns>API anahtarı veya null</returns>
    public static string? GetApiKey()
    {
        return Environment.GetEnvironmentVariable(ApiKeyEnvVar);
    }

    /// <summary>
    /// API anahtarının tanımlı olup olmadığını kontrol eder
    /// </summary>
    /// <returns>API anahtarı varsa true</returns>
    public static bool IsApiKeyConfigured()
    {
        var apiKey = GetApiKey();
        return !string.IsNullOrWhiteSpace(apiKey);
    }

    /// <summary>
    /// API anahtarının eksik olduğunu bildiren hata mesajı
    /// </summary>
    public static string ApiKeyMissingMessage => 
        $"OpenAI API anahtarı bulunamadı.\n\n" +
        $"Lütfen '{ApiKeyEnvVar}' ortam değişkenini ayarlayın:\n\n" +
        $"Windows PowerShell:\n" +
        $"$env:{ApiKeyEnvVar} = \"your-api-key\"\n\n" +
        $"Windows CMD:\n" +
        $"set {ApiKeyEnvVar}=your-api-key\n\n" +
        $"Kalıcı olarak ayarlamak için Sistem Ortam Değişkenlerini kullanın.";
}
