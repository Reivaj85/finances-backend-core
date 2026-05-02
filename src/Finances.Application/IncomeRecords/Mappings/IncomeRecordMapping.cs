using Finances.Application.IncomeRecords.Contracts;
using Finances.Domain.IncomeRecords;

namespace Finances.Application.IncomeRecords.Mappings;

internal static class IncomeRecordMapping
{
    public static IncomeRecordResponse ToResponse(this IncomeRecord incomeRecord)
    {
        return new IncomeRecordResponse(
            incomeRecord.Id,
            incomeRecord.IncomeSourceId,
            incomeRecord.Amount.Amount,
            incomeRecord.Amount.Currency.Code,
            incomeRecord.ReceivedOn);
    }
}
