using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.DbModels
{
    public sealed class Bank
    {
        [Key]
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public decimal Balance { get; set; }
        private List<Account> _clients;
        public IReadOnlyCollection<Account> Clients => _clients.AsReadOnly();

        public Bank(string name, decimal balance)
        {
            Id = Guid.NewGuid();
            Name = name;
            Balance = balance;
        }

    }
}
