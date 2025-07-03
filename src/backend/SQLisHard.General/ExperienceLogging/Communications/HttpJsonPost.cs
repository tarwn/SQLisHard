using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace SQLisHard.General.ExperienceLogging.Communications
{
	public class HttpJsonPost
	{
		private static readonly HttpClient _httpClient = CreateHttpClient();

		private string _messageContent;
		private NetworkCredential? _credentials;
		private bool _useJson;

		private static HttpClient CreateHttpClient()
		{
			var handler = new HttpClientHandler();
			var client = new HttpClient(handler)
			{
				Timeout = TimeSpan.FromSeconds(15)
			};
			return client;
		}

		public HttpJsonPost(string message, NetworkCredential? credentials = null, bool useJson = true)
		{
			_messageContent = message;
			_credentials = credentials;
			_useJson = useJson;
		}

		public HttpJsonPost(Dictionary<string, object> message, NetworkCredential? credentials = null, bool useJson = true)
		{
			_messageContent = ConvertToMessage(message, useJson);
			_credentials = credentials;
			_useJson = useJson;
		}

		private string ConvertToMessage(Dictionary<string, object> message, bool useJson)
		{
			if (useJson)
				return JsonSerializer.Serialize(message);
			else
				return string.Join(" ", message.Select(m => String.Format("{0}={1}", m.Key, m.Value?.ToString() ?? "")));
		}

		public async Task SendAsync(string url, HttpMethod method, Action<Result>? callback = null, bool processResponse = true)
		{
			Result result;
			try
			{
				using var requestMessage = new HttpRequestMessage(method, url);

				HttpContent content = new StringContent(_messageContent, Encoding.UTF8, _useJson ? "application/json" : "text/plain");
				requestMessage.Content = content;

				if (_credentials != null)
				{
					var byteArray = Encoding.ASCII.GetBytes($"{_credentials.UserName}:{_credentials.Password}");
					requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
				}

				using var response = await _httpClient.SendAsync(requestMessage);

				if (processResponse)
				{
					string rawContent = await response.Content.ReadAsStringAsync();
					result = new Result()
					{
						Success = response.IsSuccessStatusCode,
						RawContent = rawContent
					};
				}
				else
				{
					result = new Result() { Success = response.IsSuccessStatusCode };
				}

				if (!processResponse)
				{
					response.EnsureSuccessStatusCode();
				}

			}
			catch (HttpRequestException httpExc)
			{
				WriteTrace(httpExc, "HttpJsonPost.SendAsync");
				result = new ErrorResult(httpExc);
			}
			catch (TaskCanceledException timeoutExc)
			{
				WriteTrace(timeoutExc, "HttpJsonPost.SendAsync (Timeout)");
				result = new ErrorResult(timeoutExc);
			}
			catch (Exception exc)
			{
				Trace.WriteLine($"HttpJsonPost.SendAsync Exception: {exc.GetType().Name} - {exc.Message}");
				result = new ErrorResult(exc);
			}

			callback?.Invoke(result);
		}

		private void WriteTrace(Exception exc, string methodName)
		{
			string details = $"Exception: {exc.GetType().Name} - {exc.Message}";
			if (exc is HttpRequestException httpExc && httpExc.StatusCode.HasValue)
			{
				details += $", StatusCode: {httpExc.StatusCode.Value}";
			}
			else if (exc is WebException webExc)
			{
				var httpWebResp = webExc.Response as HttpWebResponse;
				string responseCode = "N/A";
				if (httpWebResp != null)
					responseCode = String.Format("{0}: {1}", (int)httpWebResp.StatusCode, httpWebResp.StatusDescription);
				details += $", WebExceptionStatus: {webExc.Status}, WebResponse: {responseCode}";
			}
			Trace.WriteLine($"{methodName} Failure\n{details}");
		}
	}
}