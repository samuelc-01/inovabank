using InovaBank.Domain.Enums;
using InovaBank.Domain.Primitives;
using InovaBank.Domain.ValueObjects;

namespace InovaBank.Domain.Entities;

public class Account : Entity
{
    public Cnpj Cnpj { get; private set; }
    public string RazaoSocial { get; private set; }
    public string Agencia { get; private set; }
    public string ImagemDocumentoPath { get; private set; }
    public decimal Balance { get; private set; }
    public AccountStatus Status { get; private set; }

    public bool CanPerformTransactions => Status == AccountStatus.Ativa;

    private readonly List<Transaction> _transactions = [];
    public IReadOnlyCollection<Transaction> Transactions => _transactions;

    public Account(Cnpj cnpj, string razaoSocial, string agencia, string imagemDocumentoPath)
    {
        Id = Guid.NewGuid();
        Cnpj = cnpj;
        RazaoSocial = razaoSocial;
        Agencia = agencia;
        ImagemDocumentoPath = imagemDocumentoPath;
        Balance = 0;
        Status = AccountStatus.Ativa;
    }

    private Account() { }

    public Result Deposit(decimal amount)
    {
        if (amount <= 0)
            return Result.Failure("Valor do depósito deve ser maior que zero", 422);

        if (!CanPerformTransactions)
            return Result.Failure("Conta não permite depósitos no status atual.", 422);

        Balance += amount;

        return Result.Success();
    }

    public Result Withdraw(decimal amount)
    {
        if (amount <= 0)
            return Result.Failure("Valor do saque deve ser maior que zero", 422);

        if (!CanPerformTransactions)
            return Result.Failure("Conta não permite saques no status atual.", 422);

        if (amount > Balance)
            return Result.Failure("Saldo insuficiente.", 422);

        Balance -= amount;

        return Result.Success();
    }

    public Result Credit(decimal amount, string currency, string description)
    {
        if (!CanPerformTransactions)
            return Result.Failure("Conta bloqueada ou encerrada.", 422);

        Balance += amount;
        _transactions.Add(new Transaction(Id, amount, currency, TransactionType.Deposito, description));

        return Result.Success();
    }

    public Result Debit(decimal amount, string currency, string description)
    {
        if (!CanPerformTransactions)
            return Result.Failure("Conta bloqueada ou encerrada.", 422);

        if (Balance < amount)
            return Result.Failure("Saldo insuficiente.", 422);

        Balance -= amount;
        _transactions.Add(new Transaction(Id, amount, currency, TransactionType.Saque, description));

        return Result.Success();
    }

    public Result ChangeStatus(AccountStatus newStatus)
    {
        if (Status == AccountStatus.Encerrada)
            return Result.Failure("Não é possível alterar o status de uma conta encerrada.", 422);

        Status = newStatus;
        return Result.Success();
    }

    public Result Close()
    {
        if (Status == AccountStatus.Encerrada)
            return Result.Failure("Conta já encerrada.", 422);

        if (Balance != 0)
            return Result.Failure("Só é possível encerrar contas com saldo zero.", 422);

        Status = AccountStatus.Encerrada;
        return Result.Success();
    }
}
