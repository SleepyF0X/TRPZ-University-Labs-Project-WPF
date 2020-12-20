using System;
using System.ComponentModel.DataAnnotations;

namespace DAL.DbModels
{
    public sealed class Deposite
    {
        public Guid Id { get; private set; }
        [Required]
        public Guid ClientId { get; private set; }
        public Account Client { get; private set; }
        [Required]
        public Guid BankId { get; private set; }
        public Bank Bank { get; private set; }
        [Required]
        public DateTimeOffset StartDate { get; private set; }
        [Required]
        public decimal StartSum { get; private set; }
        public int PercentagePerYear { get; private set; }
        public int Years { get; private set; }

        public Deposite(Guid clientId, Guid bankId, DateTimeOffset startDate, decimal startSum, int percentagePerYear, int years)
        {
            Id = Guid.NewGuid();
            ClientId = clientId;
            BankId = bankId;
            StartDate = startDate;
            StartSum = startSum;
            PercentagePerYear = percentagePerYear;
            Years = years;
        }
    }
}
