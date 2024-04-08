using Core.Helpers;
using Core.Models;
using Core.Utilities.Results;
using Polly;
using RestSharp;

namespace Core.GeneralHelpers
{
    public interface IRestSharpHelper
    {
        IDataResult<RestResponse> Generic(GenericReqModel model);
    }

    public class RestSharpHelper : IRestSharpHelper
    {
        public IDataResult<RestResponse> Generic(GenericReqModel model)
        {
            Task<IDataResult<RestResponse>> request() => Task.Run(() => GenericRequest(model));

            return Policy.HandleResult<IDataResult<RestResponse>>(response => response.Success is false).WaitAndRetryAsync(
                 retryCount: model.retry?.Count ?? 1, sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(model.retry?.Sleep ?? 3),
                 onRetry: (res, timeSpan, retryCount, context) =>
                 {
                     GeneralStaticHelper.LogWrite(RequestErrorLog(model.url, res.Result));
                     GeneralStaticHelper.LogWrite($"http request trying again... ({retryCount})", LogType.ERROR);
                 }).ExecuteAsync(request).GetAwaiter().GetResult();
        }

        private string RequestErrorLog(string url, IDataResult<RestResponse> res)
            => $"{url} => status: {res.Data?.StatusCode}-({res.Data?.ResponseStatus})-({res.Data?.StatusDescription})\ncontent: {res.Data?.Content}" +
                    $"\nerrorMessage: {res.Data?.ErrorMessage}\nexception: {res.Data?.ErrorException}";

        private IDataResult<RestResponse> GenericRequest(GenericReqModel model)
        {
            try
            {
                var request = new RestRequest { Method = model.method };

                if (model.headers is not null)
                    model.headers?.ToList().ForEach((x) =>
                    {
                        if (x.Key is not null && x.Value is not null)
                            request.AddHeader(x.Key.ToString(), x.Value.ToString());
                    });

                if (model.parameters is not null)
                    model.parameters?.ToList().ForEach((x) => request.AddParameter(x.Key.ToString(), x.Value.ToString()));

                if (model.body is not null)
                    request.AddParameter("application/json", model.body, ParameterType.RequestBody);

                if (model.isForm is true)
                    request.AlwaysMultipartFormData = true;

                //if (model.isSsl is true)
                //    client.RemoteCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

                if (model.byte_file is not null)
                {
                    if (model.byte_file.file_path is not null)
                        request.AddFile(model.byte_file.Param, model.byte_file.file_path);
                    else
                        request.AddFile(model.byte_file.Param, model.byte_file.bytes, model.byte_file.FileName);
                }

                var response = new RestClient(model.url).Execute(request);
                return new SuccessDataResult<RestResponse>(response);
            }
            catch (Exception e)
            {
                GeneralStaticHelper.LogWrite($"GenericSync() => ", LogType.ERROR, e);

                return new ErrorDataResult<RestResponse>(e.Message);
            }
        }
    }
}