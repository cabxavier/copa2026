// Wrapper reutilizável de Chart.js para integração via JSInterop.
// Chart.js (UMD) é carregado globalmente como `window.Chart` por <script> em App.razor.
// Mantém um registro de instâncias por id de canvas para permitir destruição.

const instancias = new Map();

/**
 * Renderiza (ou re-renderiza) um gráfico no canvas indicado.
 * @param {string} canvasId id do elemento <canvas>
 * @param {object} config configuração nativa do Chart.js ({ type, data, options })
 */
export function renderChart(canvasId, config) {
  const canvas = document.getElementById(canvasId);
  if (!canvas || !window.Chart) {
    return;
  }
  // Evita duplicar instâncias sobre o mesmo canvas.
  destroyChart(canvasId);
  instancias.set(canvasId, new window.Chart(canvas, config));
}

/**
 * Atalho para gráfico de barras a partir de rótulos e valores.
 * Reutilizável por qualquer visualização estatística do portal.
 */
export function renderBarChart(canvasId, labels, valores, rotulo, horizontal, cores) {
  // Cor padrão = azul do protótipo (#3b82f6). Cores por barra são opcionais.
  const backgroundColor = (cores && cores.length) ? cores : '#3b82f6';
  const config = {
    type: 'bar',
    data: {
      labels,
      datasets: [{
        label: rotulo ?? '',
        data: valores,
        backgroundColor,
        borderWidth: 0,
        borderRadius: 6,
      }],
    },
    options: {
      indexAxis: horizontal ? 'y' : 'x',
      responsive: true,
      maintainAspectRatio: false,
      plugins: { legend: { display: !!rotulo } },
      scales: { x: { beginAtZero: false } },
    },
  };
  renderChart(canvasId, config);
}

/** Destrói o gráfico associado ao canvas e libera recursos. */
export function destroyChart(canvasId) {
  const grafico = instancias.get(canvasId);
  if (grafico) {
    grafico.destroy();
    instancias.delete(canvasId);
  }
}
