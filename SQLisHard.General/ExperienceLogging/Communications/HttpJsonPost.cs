using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using ServiceStack.Text;
using System.Diagnostics;

namespace SQLisHard.General.ExperienceLogging.Communications
{
	public class HttpJsonPost
	{

		string _message;
		NetworkCredential _credentials;
		bool _useJson;

		public HttpJsonPost(string message, NetworkCredential credentials = null, bool useJson = true)
		{
			_message = message;
			_credentials = credentials;
			_useJson = useJson;
		}

		public HttpJsonPost(Dictionary<string, object> message, NetworkCredential credentials = null, bool useJson = true)
		{
			_message = ConvertToMessage(message, useJson);
			_credentials = credentials;
			_useJson = useJson;
		}

		private string ConvertToMessage(Dictionary<string, object> message, bool useJson)
		{
			if (useJson)
				return JsonSerializer.SerializeToString(message);
			else
				return string.Join(" ", message.Select(m => String.Format("{0}={1}", m.Key, m.Value.ToString())));
		}

		private HttpWebRequest InitializeRequest(string url, string method)
		{
			var request = (HttpWebRequest)HttpWebRequest.Create(url);
			request.Method = method;
			request.Timeout = 15000;
			request.ReadWriteTimeout = 15000;
			request.KeepAlive = false;
			if (_credentials != null)
			{
				request.Credentials = _credentials;
				request.PreAuthenticate = true;
			}

			if (_useJson)
				request.ContentType = "application/json";

			return request;
		}

		public void Send(string url, string method, Action<Result> callback, bool processResponse = true)
		{
			var request = InitializeRequest(url, method);

			using (var stream = request.GetRequestStream())
			{
				WriteMessage(stream);
			}

			var state = new RequestState()
			{ 
				Callback = callback,
				RequestRetryCount = 0,	/* won't retry dead conns in .Net http pool */
				Request = request,
				ProcessResponse = processResponse
			};

			if (processResponse)
				ProcessResponse(() => request.GetResponse(), state);
			else if (callback != null)
				callback(new Result() { Success = true });
		}

		public void SendAsync(string url, string method, Action<Result> callback, bool processResponse = true)
		{
			var request = InitializeRequest(url, method);

			var state = new RequestState() {
				Request = request,
				Callback = callback,
				ProcessResponse = processResponse,
				RequestRetryCount = 1
			};
			request.BeginGetRequestStream(new AsyncCallback(GetRequestStream), state);
		}

		private void GetRequestStream(IAsyncResult result)
		{
			var state = (RequestState)result.AsyncState;
			try
			{
				using (var postStream = state.Request.EndGetRequestStream(result))
				{
					WriteMessage(postStream);
				}

				if (state.ProcessResponse)
					state.Request.BeginGetResponse(GetResponseStream, state);
				else
				{
					state.Request.Abort();
					if (state.Callback != null)
						state.Callback(new Result() { Success = true });
				}
			}
			catch (WebException wexc)
			{
				WriteTrace(wexc, "HttpJsonPost.GetRequestStream");
				if (state.Callback != null)
					state.Callback(new ErrorResult(wexc));
			}
		}

		private void WriteTrace(WebException wexc, string methodName)
		{
			var httpWebResp = wexc.Response as HttpWebResponse;
			string responseCode = "N/A";
			if (httpWebResp != null)
				responseCode = String.Format("{0}: {1}", (int)httpWebResp.StatusCode, httpWebResp.StatusDescription);
			Trace.WriteLine(String.Format("{0} Exception\nException: {1} - {2}\nWebResponse: {3}\nHttpWebResponse: {4}", methodName, wexc.GetType().Name, wexc.Message, wexc.Status, responseCode));
		}

		private void GetResponseStream(IAsyncResult result)
		{
			var state = (RequestState)result.AsyncState;
			ProcessResponse(() => state.Request.EndGetResponse(result), state);
		}

		private void ProcessResponse(Func<WebResponse> getResponse, RequestState state)
		{
			try
			{
				using (var response = getResponse())
				{
					if (state.Callback != null)
					{
						var responseResult = new Result() { Success = true };
						using (var reader = new StreamReader(response.GetResponseStream()))
						{
							responseResult.RawContent = reader.ReadToEnd();
						}
						state.Callback(responseResult);
					}
				}
			}
			catch (WebException wexc)
			{
				if(state.RequestRetryCount > 0 && wexc.Status == WebExceptionStatus.ConnectionClosed)
				{
					WriteTrace(wexc, "HttpJsonPost.GetRequestStream (will retry)");
					state.RequestRetryCount--;
					state.Request.BeginGetResponse(GetResponseStream, state);
				}
				else
				{
					WriteTrace(wexc, "HttpJsonPost.GetRequestStream");
					if (state.Callback != null)
						state.Callback(new ErrorResult(wexc));
				}
			}
			catch (Exception exc)
			{
				if (state.Callback != null)
					state.Callback(new ErrorResult(exc));
			}
		}

		private void WriteMessage(Stream stream)
		{
			//if (_useJson) {
			//    JsonSerializer.SerializeToStream(_message, stream);
			//}
			//else {
			byte[] data = new System.Text.UTF8Encoding().GetBytes(_message + "\r\n");
			stream.Write(data, 0, data.Length);
			stream.Flush();
			//}
		}
	}

}