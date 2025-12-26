using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using LLMChatbot.WinForms.Common;
using LLMChatbot.WinForms.Core;

namespace LLMChatbot.WinForms.Services;

/// <summary>
/// OpenAI Responses API ile iletişimi yöneten servis sınıfı.
/// Tüm API çağrıları async olarak gerçekleştirilir.
/// </summary>
public class OpenAiService : IDisposable
{
    private readonly HttpClient _httpClient;
    private bool _disposed;

    /// <summary>
    /// OpenAiService oluşturur ve HttpClient yapılandırır
    /// </summary>
    public OpenAiService()
    {
        _httpClient = new HttpClient();
        ConfigureHttpClient();
    }

    /// <summary>
    /// HttpClient'ı Authorization header ile yapılandırır
    /// </summary>
    private void ConfigureHttpClient()
    {
        var apiKey = ConfigHelper.GetApiKey();
        if (!string.IsNullOrEmpty(apiKey))
        {
            _httpClient.DefaultRequestHeaders.Authorization = 
                new AuthenticationHeaderValue("Bearer", apiKey);
        }
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
    }

    /// <summary>
    /// Sohbet geçmişini kullanarak OpenAI'dan yanıt alır
    /// </summary>
    /// <param name="conversation">Mevcut sohbet geçmişi</param>
    /// <returns>Asistan yanıtı</returns>
    /// <exception cref="InvalidOperationException">API anahtarı eksikse</exception>
    /// <exception cref="HttpRequestException">API hatası durumunda</exception>
    public async Task<string> GetResponseAsync(Conversation conversation)
    {
        // API anahtarı kontrolü
        if (!ConfigHelper.IsApiKeyConfigured())
        {
            throw new InvalidOperationException(ConfigHelper.ApiKeyMissingMessage);
        }

        // İstek gövdesini oluştur
        var requestBody = new
        {
            model = ConfigHelper.Model,
            input = conversation.GetMessagesForApi()
        };

        var jsonContent = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // API isteği gönder
        var response = await _httpClient.PostAsync(ConfigHelper.ResponsesApiEndpoint, content);
        var responseContent = await response.Content.ReadAsStringAsync();

        // Hata kontrolü
        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = ParseErrorMessage(responseContent, response.StatusCode);
            throw new HttpRequestException(errorMessage);
        }

        // Yanıtı parse et
        return ParseResponse(responseContent);
    }

    /// <summary>
    /// API yanıtından metin içeriğini çıkarır
    /// Önce output_text, yoksa output.content.text alanlarını kontrol eder
    /// </summary>
    private string ParseResponse(string responseContent)
    {
        try
        {
            using var document = JsonDocument.Parse(responseContent);
            var root = document.RootElement;

            // Önce output_text alanını kontrol et
            if (root.TryGetProperty("output_text", out var outputText))
            {
                return outputText.GetString() ?? "Yanıt alınamadı.";
            }

            // output dizisini kontrol et
            if (root.TryGetProperty("output", out var output) && 
                output.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in output.EnumerateArray())
                {
                    // content dizisini kontrol et
                    if (item.TryGetProperty("content", out var contentArray) && 
                        contentArray.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var contentItem in contentArray.EnumerateArray())
                        {
                            if (contentItem.TryGetProperty("text", out var text))
                            {
                                return text.GetString() ?? "Yanıt alınamadı.";
                            }
                        }
                    }

                    // Doğrudan text alanını kontrol et
                    if (item.TryGetProperty("text", out var directText))
                    {
                        return directText.GetString() ?? "Yanıt alınamadı.";
                    }
                }
            }

            // choices dizisini kontrol et (eski format uyumluluğu)
            if (root.TryGetProperty("choices", out var choices) && 
                choices.ValueKind == JsonValueKind.Array)
            {
                foreach (var choice in choices.EnumerateArray())
                {
                    if (choice.TryGetProperty("message", out var message) &&
                        message.TryGetProperty("content", out var msgContent))
                    {
                        return msgContent.GetString() ?? "Yanıt alınamadı.";
                    }
                }
            }

            return "Yanıt formatı tanınamadı.";
        }
        catch (JsonException)
        {
            return "Yanıt parse edilemedi.";
        }
    }

    /// <summary>
    /// API hata mesajını parse eder
    /// </summary>
    private string ParseErrorMessage(string responseContent, System.Net.HttpStatusCode statusCode)
    {
        try
        {
            using var document = JsonDocument.Parse(responseContent);
            var root = document.RootElement;

            if (root.TryGetProperty("error", out var error))
            {
                if (error.TryGetProperty("message", out var message))
                {
                    return $"API Hatası ({(int)statusCode}): {message.GetString()}";
                }
            }
        }
        catch
        {
            // JSON parse edilemezse devam et
        }

        return $"API Hatası ({(int)statusCode}): {responseContent}";
    }

    /// <summary>
    /// Kaynakları serbest bırakır
    /// </summary>
    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient.Dispose();
            _disposed = true;
        }
        GC.SuppressFinalize(this);
    }
}
