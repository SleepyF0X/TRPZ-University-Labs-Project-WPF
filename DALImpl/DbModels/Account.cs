using System;
using System.Collections.Generic;
using DAL.DbModels.Identity.IdentityModels;

namespace DAL.DbModels
{
    public sealed class Account
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public User User { get; private set; }
        public decimal Balance { get; set; }
        public decimal CreditLine { get; private set; }
        public Guid BankId { get; private set; }
        public Bank Bank { get; private set; }
        private List<Credit> _credits;
        private List<Deposite> _deposites;
        private List<Transaction> _transactions;
        public IReadOnlyCollection<Credit> Credits => _credits.AsReadOnly();
        public IReadOnlyCollection<Deposite> Deposites => _deposites.AsReadOnly();
        public IReadOnlyCollection<Transaction> Transactions => _transactions.AsReadOnly();

        public Account(Guid userId, decimal balance, decimal creditLine, Guid bankId)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Balance = balance;
            CreditLine = creditLine;
            BankId = bankId;
        }
    }
}
