using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.DbModels
{
    public sealed class Credit
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
        [Required]
        public int PercentagePerMonth { get; private set; }
        [Required]
        public int Months { get; private set; }
        [Required]
        public decimal CurrentSum{ get; private set; }
        [NotMapped]
        public decimal MonthPayment => (StartSum + (StartSum * PercentagePerMonth / 100 * Months)) / Months;

        public Credit(Guid clientId, Guid bankId, DateTimeOffset startDate, decimal startSum, int percentagePerMonth, int months)
        {
            Id = Guid.NewGuid();
            ClientId = clientId;
            BankId = bankId;
            StartDate = startDate;
            StartSum = startSum;
            PercentagePerMonth = percentagePerMonth;
            CurrentSum = StartSum;
            Months = months;
        }
    }
}
