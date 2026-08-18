using System.Diagnostics.Metrics;

namespace InovaBank.Infrastructure.Telemetry;

public static class BankingMetrics
{
    public const string MeterName = "InovaBank";
    private static readonly Meter Meter = new(MeterName);

    public static readonly Counter<long> AccountsOpened =
        Meter.CreateCounter<long>("inovabank.accounts.opened", "accounts");

    public static readonly Counter<long> DepositsCompleted =
        Meter.CreateCounter<long>("inovabank.transactions.deposit", "transactions");

    public static readonly Counter<long> WithdrawalsCompleted =
        Meter.CreateCounter<long>("inovabank.transactions.withdraw", "transactions");

    public static readonly Counter<long> TransfersCompleted =
        Meter.CreateCounter<long>("inovabank.transfers.completed", "transfers");

    public static readonly Counter<long> TransfersFailed =
        Meter.CreateCounter<long>("inovabank.transfers.failed", "transfers");

    public static readonly Histogram<decimal> TransactionAmount =
        Meter.CreateHistogram<decimal>("inovabank.transaction.amount", "BRL");
}
