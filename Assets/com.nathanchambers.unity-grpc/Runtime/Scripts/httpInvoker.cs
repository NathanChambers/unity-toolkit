using System;
using System.Net.Http;
using Grpc.Core;
using UnityEngine;

namespace Grpc.Unity {
    public class HttpInvoker : CallInvoker {
        private string url = string.Empty;
        public HttpInvoker(string url) {
            this.url = url;
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options) {
            throw new NotImplementedException();
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options) {
            throw new NotImplementedException();
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request) {
            throw new NotImplementedException();
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request) {
            return null;
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request) {
            var content = new ByteArrayContent(method.RequestMarshaller.Serializer.Invoke(request));
            content.Headers.Add("Path", method.FullName);
            content.Headers.Add("Service-Name", method.ServiceName);
            content.Headers.Add("Content-Type", "application/grpc");

            using(var httpClient = new HttpClient()) {
                var response = httpClient.PutAsync(url, content).Result;
                var responseContent = response.Content;
                if (response.IsSuccessStatusCode == false) {
                    throw new UnityException($"[GRPC {method.FullName}] Request failed: {responseContent.ReadAsStringAsync().Result}");
                }
                return method.ResponseMarshaller.Deserializer.Invoke(responseContent.ReadAsByteArrayAsync().Result);
            }
        }
    }

}