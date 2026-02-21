using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.JSInterop;

namespace AlbumTracker.Services;

public class FirebaseJsInterop
{
    private readonly IJSRuntime _js;
    private bool _initialized;

    public FirebaseJsInterop(IJSRuntime js)
    {
        _js = js;
    }

    public async Task EnsureInitializedAsync(FirebaseConfig config)
    {
        if (_initialized) return;

        var json = JsonSerializer.Serialize(config);
        await _js.InvokeVoidAsync("firebaseInterop.initialize", json);
        _initialized = true;
    }

    public async Task<T?> GetAsync<T>(string path)
    {
        var json = await _js.InvokeAsync<string?>("firebaseInterop.getData", path);
        if (string.IsNullOrEmpty(json)) return default;
        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetAsync<T>(string path, T data)
    {
        var json = JsonSerializer.Serialize(data);
        await _js.InvokeVoidAsync("firebaseInterop.setData", path, json);
    }

    public async Task RemoveAsync(string path)
    {
        await _js.InvokeVoidAsync("firebaseInterop.removeData", path);
    }

    public async Task<string> PushAsync<T>(string path, T data)
    {
        var json = JsonSerializer.Serialize(data);
        return await _js.InvokeAsync<string>("firebaseInterop.pushData", path, json);
    }
}

public class FirebaseConfig
{
    [JsonPropertyName("apiKey")]
    public string ApiKey { get; set; } = string.Empty;

    [JsonPropertyName("authDomain")]
    public string AuthDomain { get; set; } = string.Empty;

    [JsonPropertyName("databaseURL")]
    public string DatabaseURL { get; set; } = string.Empty;

    [JsonPropertyName("projectId")]
    public string ProjectId { get; set; } = string.Empty;

    [JsonPropertyName("storageBucket")]
    public string StorageBucket { get; set; } = string.Empty;

    [JsonPropertyName("messagingSenderId")]
    public string MessagingSenderId { get; set; } = string.Empty;

    [JsonPropertyName("appId")]
    public string AppId { get; set; } = string.Empty;
}
