using System;
using DAL.DbModels.Enums;

namespace DAL.DbModels
{
    public sealed class Transaction
    {
        public Guid Id { get; private set; }
        public DateTimeOffset Date { get; private set; }
        public TransactionType Type { get; private set; }
        public decimal Sum { get; private set; }
        public Guid ClientId { get; private set; }
        public Account Client { get; private set; }
        public Guid BankId { get; private set; }
        public Bank Bank { get; private set; }
        public Guid ServiceId { get; private set; } // Deposite or Credit Id

        public Transaction(DateTimeOffset date, TransactionType type, decimal sum, Guid clientId, Guid bankId, Guid serviceId)
        {
            Id = Guid.NewGuid();
            Date = date;
            Type = type;
            Sum = sum;
            ClientId = clientId;
            BankId = bankId;
            ServiceId = serviceId;
        }
    }
}
