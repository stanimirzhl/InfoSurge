using InfoSurge.Core.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace InfoSurge.Core.Implementations
{
	public class TranslationService : ITranslationService
	{
		private readonly HttpClient httpClient;
		private readonly IMemoryCache cache;
		private readonly ILogger<TranslationService> logger;

		public TranslationService(HttpClient httpClient,
			IMemoryCache cache,
			ILogger<TranslationService> logger)
		{
			this.httpClient = httpClient;
			this.httpClient.BaseAddress = new Uri("https://lingva.ml/");
			this.cache = cache;
			this.logger = logger;
		}

		public async Task<List<string>> TranslateListAsync(List<string> texts)
		{
			if (texts.Count == 0)
				return texts;

			try
			{
				const string delimiter = "\n###\n";
				string combinedText = string.Join(delimiter, texts);

				if (cache.TryGetValue(combinedText, out List<string> cachedTranslations))
					return cachedTranslations;

				string textToTranslate = combinedText.Length > 2000 ? combinedText.Substring(0, 2000) : combinedText;
				string url = $"api/v1/auto/bg/{Uri.EscapeDataString(textToTranslate)}";

				HttpResponseMessage response = await httpClient.GetAsync(url);

				if (!response.IsSuccessStatusCode)
				{
					string errorContent = await response.Content.ReadAsStringAsync();
					logger.LogError($"Error {response.StatusCode}: {errorContent}, {response.Headers}");

					return texts;
				}

				string json = await response.Content.ReadAsStringAsync();
				using var doc = JsonDocument.Parse(json);

				string translated = doc.RootElement.GetProperty("translation").GetString() ?? combinedText;

				List<string> result = translated.Split(delimiter).ToList();

				while (result.Count < texts.Count)
					result.Add(texts[result.Count]);

				cache.Set(translated, result, TimeSpan.FromHours(12));
				return result;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Проблем при превеждане на текст");

				return texts;
			}

		}
	}
}
