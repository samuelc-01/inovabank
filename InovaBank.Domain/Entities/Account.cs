using InovaBank.Domain.Enums;
using InovaBank.Domain.ValueObjects;

namespace InovaBank.Domain.Entities;

public class Account : Entity
{
    public Cnpj Cnpj { get; private set; }
    public string RazaoSocial { get; private set; }
    public string Agencia { get; private set; }
    public decimal Balance { get; private set; }
    public AccountStatus Status { get; private set; }

    public bool CanPerformTransactions => Status == AccountStatus.Ativa;

    public Account(Cnpj cnpj, string razaoSocial, string agencia)
    {
        Id = Guid.NewGuid();
        Cnpj = cnpj;
        RazaoSocial = razaoSocial;
        Agencia = agencia;
        Balance = 0;
        Status = AccountStatus.Ativa;
    }

    private Account() { }

    public void Deposit(decimal amount)
    {
        if (!CanPerformTransactions)
            throw new InvalidOperationException("Conta não permite depósitos no status atual.");

        Balance += amount;
    }

    public void Withdraw(decimal amount)
    {
        if (!CanPerformTransactions)
            throw new InvalidOperationException("Conta não permite saques no status atual.");

        if (amount > Balance)
            throw new InvalidOperationException("Saldo insuficiente.");

        Balance -= amount;
    }

    public void ChangeStatus(AccountStatus newStatus)
    {
        if (Status == AccountStatus.Encerrada)
            throw new InvalidOperationException("Não é possível alterar o status de uma conta encerrada.");

        Status = newStatus;
    }

    public void Close()
    {
        if (Balance != 0)
            throw new InvalidOperationException("Só é possível encerrar contas com saldo zero.");

        Status = AccountStatus.Encerrada;
    }
}
