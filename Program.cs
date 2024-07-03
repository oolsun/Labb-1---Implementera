using System;
using System.Collections.Generic;

// Designmönster: Factory Method, Singleton, Strategy
// Applikationen kan skapa konton, ta ut och sätta in pengar, räkna ut ränta på pengar.

// Singleton
public class Bank
{
    private static Bank _instance;
    private Dictionary<int, BankAccount> _accounts;

    private Bank()
    {
        _accounts = new Dictionary<int, BankAccount>();
    }

    public static Bank Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Bank();
            }
            return _instance;
        }
    }

    public void AddAccount(BankAccount account)
    {
        _accounts[account.AccountNumber] = account;
    }

    public BankAccount GetAccount(int accountNumber)
    {
        _accounts.TryGetValue(accountNumber, out var account);
        return account;
    }
}

// Factory Method
public abstract class BankAccount
{
    public int AccountNumber { get; set; }
    public decimal Balance { get; set; }
    public IInterestCalculator InterestCalculator { get; set; }

    public abstract void Deposit(decimal amount);
    public abstract void Withdraw(decimal amount);

    public void CalculateInterest()
    {
        decimal interest = InterestCalculator.CalculateInterest(Balance);
        Balance += interest;
        Console.WriteLine($"Ränta har betalats ut med {interest}kr till kontonummer {AccountNumber}. Nytt saldo: {Balance}kr.");
    }
}

public class SavingsAccount : BankAccount
{
    public override void Deposit(decimal amount)
    {
        Balance += amount;
        Console.WriteLine($"{amount}kr insatt på sparkonto {AccountNumber}. Nytt saldo: {Balance}kr.");
    }

    public override void Withdraw(decimal amount)
    {
        if (Balance >= amount)
        {
            Balance -= amount;
            Console.WriteLine($"{amount}kr uttaget från sparkonto {AccountNumber}. Nytt saldo: {Balance}kr.");
        }
        else
        {
            Console.WriteLine("Du har inte tillräckligt med pengar på ditt konto.");
        }
    }
}

public class CheckingAccount : BankAccount
{
    public override void Deposit(decimal amount)
    {
        Balance += amount;
        Console.WriteLine($"{amount}kr insatt på lönekonto {AccountNumber}. Nytt saldo: {Balance}kr.");
    }

    public override void Withdraw(decimal amount)
    {
        if (Balance >= amount)
        {
            Balance -= amount;
            Console.WriteLine($"{amount}kr uttaget från lönekonto {AccountNumber}. Nytt saldo: {Balance}kr.");
        }
        else
        {
            Console.WriteLine("Du har inte tillräckligt med pengar på ditt konto.");
        }
    }
}

public static class BankAccountFactory
{
    public static BankAccount CreateAccount(string type, int accountNumber)
    {
        if(type == "savings")
        {
            return new SavingsAccount { AccountNumber = accountNumber, Balance = 0, InterestCalculator = new SavingsInterestCalculator() };
        }
        else if(type == "checking")
        {
            return new CheckingAccount { AccountNumber = accountNumber, Balance = 0, InterestCalculator = new CheckingInterestCalculator() };
        }
        else
        {
            throw new Exception("Fel kontotyp");
        }
    }
}

// Strategy
public interface IInterestCalculator
{
    decimal CalculateInterest(decimal balance);
}

public class SavingsInterestCalculator : IInterestCalculator
{
    public decimal CalculateInterest(decimal balance)
    {
        return balance * 0.05m;
    }
}

public class CheckingInterestCalculator : IInterestCalculator
{
    public decimal CalculateInterest(decimal balance)
    {
        return balance * 0.01m;
    }
}


class Program
{
    static void Main(string[] args)
    {
        // Singleton
        var bank = Bank.Instance;

        // Factory Method
        var savingsAccount = BankAccountFactory.CreateAccount("savings", 1234567890);
        var checkingAccount = BankAccountFactory.CreateAccount("checking", 1237894560);      
        bank.AddAccount(savingsAccount);
        bank.AddAccount(checkingAccount);
        Console.WriteLine($"Sparkonto skapat med kontonummer: {savingsAccount.AccountNumber}");
        Console.WriteLine($"Lönekonto skapat med kontonummer: {checkingAccount.AccountNumber}");
        Console.ReadLine();

        Console.WriteLine("Gör en insättning på sparkonto. Hur mycket vill du sätta in?");
        int savingsDeposit = Convert.ToInt32(Console.ReadLine());
        savingsAccount.Deposit(savingsDeposit);
        Console.ReadLine();

        Console.WriteLine("Gör en insättning på lönekonto. Hur mycket vill du sätta in?");
        int checkingDeposit = Convert.ToInt32(Console.ReadLine());
        checkingAccount.Deposit(checkingDeposit);
        Console.ReadLine();

        Console.WriteLine($"Gör ett uttag från sparkonto (Saldo: {savingsAccount.Balance}kr). Hur mycket vill du ta ut?");
        int savingsWhitdrawal = Convert.ToInt32(Console.ReadLine());
        savingsAccount.Withdraw(savingsWhitdrawal);
        Console.ReadLine();

        Console.WriteLine($"Gör ett uttag från lönekonto (Saldo: {checkingAccount.Balance}kr). Hur mycket vill du ta ut?");
        int checkingWhitdrawal = Convert.ToInt32(Console.ReadLine());
        checkingAccount.Withdraw(checkingWhitdrawal);
        Console.ReadLine();

        // Strategy pattern usage
        savingsAccount.CalculateInterest();
        checkingAccount.CalculateInterest();
        Console.ReadLine();
    }
}
