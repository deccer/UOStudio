using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using Serilog;
using UOStudio.Server.Data;

namespace UOStudio.Server.Network.PacketHandlers
{
    public sealed class CreateProjectRequestHandler : IPacketHandler<CreateProjectRequest, CreateProjectResult>
    {
        private readonly ILogger _logger;
        private readonly IDbContextFactory<UOStudioDbContext> _contextFactory;

        public CreateProjectRequestHandler(ILogger logger, IDbContextFactory<UOStudioDbContext> contextFactory)
        {
            _logger = logger.ForContext<CreateProjectRequestHandler>();
            _contextFactory = contextFactory;
        }

        public async Task<Result<CreateProjectResult>> Handle(CreateProjectRequest createProjectRequest)
        {
            await using var context = _contextFactory.CreateDbContext();

            var creator = await context.Accounts.FindAsync(createProjectRequest.AccountId);
            if (creator == null)
            {
                return Result.Failure<CreateProjectResult>("Creator account does not exist.");
            }

            var project = await context.Projects
                .FirstOrDefaultAsync(p => p.Name.ToLower() == createProjectRequest.Name.ToLower()
                                          && p.ClientVersion == createProjectRequest.ClientVersion);
            if (project == null)
            {
                project = new Project
                {
                    Name = createProjectRequest.Name,
                    Description = createProjectRequest.Description,
                    ClientVersion = createProjectRequest.ClientVersion,
                    CreatedBy = creator,
                    CreatedAt = DateTimeOffset.Now
                };
                await context.Projects.AddAsync(project);
                await context.SaveChangesAsync();
            }
            else
            {
                return Result.Failure<CreateProjectResult>("Project already exists.");
            }

            return Result.Success(new CreateProjectResult(project.Id));
        }
    }
}
