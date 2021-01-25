using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;

namespace WebApp.Mvc.Services.gRPC
{
    public class GrpcServiceInterceptor : Interceptor
    {
        private readonly IHttpContextAccessor _httpContextAccesor;

        public GrpcServiceInterceptor(IHttpContextAccessor httpContextAccesor) 
            => _httpContextAccesor = httpContextAccesor;

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(
            TRequest request,
            ClientInterceptorContext<TRequest, TResponse> context,
            AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
        {
            //var token = _httpContextAccesor.HttpContext.Request.Headers["Authorization"];
            var claim = _httpContextAccesor.HttpContext.User.FindFirst("JWT");
            string token = claim?.Value;
                        
            var headers = new Metadata
            {
                {"Authorization", $"Bearer {token}"}
            };

            var options = context.Options.WithHeaders(headers);
            context = new ClientInterceptorContext<TRequest, TResponse>(context.Method, context.Host, options);

            return base.AsyncUnaryCall(request, context, continuation);
        }
    }
}
