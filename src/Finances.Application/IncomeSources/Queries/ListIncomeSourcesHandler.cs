using Finances.Application.IncomeSources.Abstractions;
using Finances.Application.IncomeSources.Contracts;
using Finances.Application.IncomeSources.Mappings;

namespace Finances.Application.IncomeSources.Queries;

public static class ListIncomeSourcesHandler
{
    public static async Task<IReadOnlyList<IncomeSourceResponse>> Handle(
        ListIncomeSourcesQuery query,
        IIncomeSourceRepository repository,
        CancellationToken cancellationToken)
    {
        var incomeSources = await repository.ListByHouseholdAsync(query.HouseholdId, cancellationToken);

        return incomeSources
            .Select(incomeSource => incomeSource.ToResponse())
            .ToArray();
    }
}
