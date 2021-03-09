using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using MediatR;
using Serilog;
using UOStudio.Common.Contracts;
using UOStudio.Common.Core;
using UOStudio.Server.Data;

namespace UOStudio.Server.Requests
{
    [UsedImplicitly]
    internal sealed class ClientLoginRequestHandler : IRequestHandler<ClientLoginRequest, Result<ClientLoginResult>>
    {
        private readonly ILogger _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordVerifier _passwordVerifier;
        private readonly ILiteDatabaseFactory _liteDatabaseFactory;

        public ClientLoginRequestHandler(
            ILogger logger,
            IMapper mapper,
            IPasswordVerifier passwordVerifier,
            ILiteDatabaseFactory liteDatabaseFactory)
        {
            _logger = logger.ForContext<ClientLoginRequestHandler>();
            _mapper = mapper;
            _passwordVerifier = passwordVerifier;
            _liteDatabaseFactory = liteDatabaseFactory;
        }

        public async Task<Result<ClientLoginResult>> Handle(ClientLoginRequest request, CancellationToken cancellationToken)
        {
            _logger.Debug("{@TypeName}", GetType().Name);
            using var db = await _liteDatabaseFactory.OpenDatabaseAsync()
                .ConfigureAwait(false);

            var user = await db.GetCollection<User>()
                .FindOneAsync(u => u.Name == request.UserName)
                .ConfigureAwait(false);
            if (user == null || !_passwordVerifier.Verify(request.Password, user.Password))
            {
                _logger.Error("Invalid user {@UserName}", request.UserName);
                return Result.Failure<ClientLoginResult>("Invalid User");
            }

            if (!user.HasPermission(Permission.CanConnect))
            {
                _logger.Error("User {@UserName} is not allowed to connect", user.Name);
                return Result.Failure<ClientLoginResult>("Not allowed to connect");
            }

            var projects = await db.GetCollection<Project>()
                .FindAllAsync()
                .ConfigureAwait(false);

            var allowedProjects = projects.Where(p => (p.CreatedBy == user || p.IsPublic) || (!p.IsPublic && p.AllowedUsers.Contains(user))).ToList();

            var projectsDtos = _mapper.Map<IList<ProjectDto>>(allowedProjects);
            _logger.Debug("User {@Client} logged in", user.Name);
            return Result.Success(new ClientLoginResult(user.Id, projectsDtos));
        }
    }
}
