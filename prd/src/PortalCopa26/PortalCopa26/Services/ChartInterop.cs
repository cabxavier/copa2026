using Microsoft.JSInterop;

namespace PortalCopa26.Services;

/// <summary>
/// Abstração .NET sobre o módulo `wwwroot/js/charts.js`. Carrega o módulo ES sob
/// demanda e expõe métodos tipados para renderizar/destruir gráficos Chart.js.
/// Reutilizável por qualquer visualização estatística do portal.
/// </summary>
public sealed class ChartInterop : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;

    public ChartInterop(IJSRuntime js)
    {
        _js = js;
        _moduleTask = new Lazy<Task<IJSObjectReference>>(() =>
            _js.InvokeAsync<IJSObjectReference>("import", "./js/charts.js").AsTask());
    }

    /// <summary>Renderiza um gráfico de barras a partir de rótulos e valores.</summary>
    public async Task RenderBarChartAsync(
        string canvasId,
        IEnumerable<string> labels,
        IEnumerable<decimal> valores,
        string? rotulo = null,
        bool horizontal = false,
        IEnumerable<string>? cores = null)
    {
        var module = await _moduleTask.Value;
        await module.InvokeVoidAsync("renderBarChart", canvasId, labels, valores, rotulo, horizontal, cores);
    }

    /// <summary>Destrói o gráfico associado ao canvas, liberando recursos JS.</summary>
    public async Task DestroyChartAsync(string canvasId)
    {
        if (!_moduleTask.IsValueCreated)
        {
            return;
        }

        try
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("destroyChart", canvasId);
        }
        catch (JSDisconnectedException)
        {
            // Circuito já encerrado/desconectado; não há mais o que liberar no JS.
        }
        catch (OperationCanceledException)
        {
            // Chamada cancelada durante o teardown do circuito.
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (!_moduleTask.IsValueCreated)
        {
            return;
        }

        try
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
            // Circuito já encerrado/desconectado; o módulo JS já foi liberado.
        }
        catch (OperationCanceledException)
        {
            // Chamada cancelada durante o teardown do circuito.
        }
    }
}
