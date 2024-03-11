using Gesd.Api.Features.Tools;
using Gesd.Entite.Responses;

using MediatR;

namespace Gesd.Api.Features.Communs
{
    public abstract class BaseComputeCmd : IRequest<RequestResponse>
    {
        public TypeDeRequette Operation { get; set; }
    }
}
