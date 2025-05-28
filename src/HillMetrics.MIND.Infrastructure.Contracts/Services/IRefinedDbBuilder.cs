using FluentResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HillMetrics.MIND.Infrastructure.Contracts.Services
{
    public interface IRefinedDbBuilder
    {
        Task<Result<TContext>> CreateDbContextAsync<TContext>(int clientId, CancellationToken cancellationToken) where TContext : DbContext;
    }
}
