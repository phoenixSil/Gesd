using AutoMapper;

using Gesd.Api.Repositories.Contrats;
using Gesd.Entite.Responses;
using MediatR;

namespace Gesd.Api.Features.Communs
{
    public abstract class BaseComputeCmdHandler<T> : IRequestHandler<T, RequestResponse>
        where T : BaseComputeCmd
    {
        protected readonly IMediator _mediator;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly ILogger<BaseComputeCmdHandler<T>> _logger;

        protected BaseComputeCmdHandler(IMediator mediator, IUnitOfWork unitOfWork, IMapper mapper, ILogger<BaseComputeCmdHandler<T>> logger)
        {
            _mediator = mediator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public abstract Task<RequestResponse> Handle(T request, CancellationToken cancellationToken);
    }
}
