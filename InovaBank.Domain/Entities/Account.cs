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
        if (!CanPerformTransactions)
            return Result.Failure("Conta não permite depósitos no status atual.");

        Balance += amount;

        return Result.Success();
    }

    public Result Withdraw(decimal amount)
    {
        if (!CanPerformTransactions)
            return Result.Failure("Conta não permite saques no status atual.");

        if (amount > Balance)
            return Result.Failure("Saldo insuficiente.");

        Balance -= amount;

        return Result.Success();
    }

    public Result ChangeStatus(AccountStatus newStatus)
    {
        if (Status == AccountStatus.Encerrada)
            return Result.Failure("Não é possível alterar o status de uma conta encerrada.");

        Status = newStatus;
        return Result.Success();
    }

    public Result Close()
    {
        if (Balance != 0)
            return Result.Failure("Só é possível encerrar contas com saldo zero.");

        Status = AccountStatus.Encerrada;
        return Result.Success();
    }
}
