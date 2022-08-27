using System.Net;

namespace Financer.SpeechRecognition
{
    public class SubscriptionKeyHandler : DelegatingHandler
    {
        private readonly string Header = "Ocp-Apim-Subscription-Key";

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!request.Headers.Contains(Header))
            {
                request.Headers.Add(Header, "0f8a4761532244f6ad453b41a1c60bc7");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
