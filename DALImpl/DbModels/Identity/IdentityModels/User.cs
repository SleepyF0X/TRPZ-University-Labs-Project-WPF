using System;
using System.Collections.Generic;

namespace DAL.DbModels.Identity.IdentityModels
{
    public sealed class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string HashPassword { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public int Age { get; private set; }
        public decimal Balance { get; set; }
        private List<Account> _accounts;
        public IReadOnlyCollection<Account> Accounts => _accounts.AsReadOnly();

        public User(string email, string name, string surname, int age)
        {
            Id = Guid.NewGuid();
            Email = email;
            Name = name;
            Surname = surname;
            Age = age;
        }

        public void SetPassword(string password)
        {
            HashPassword = password;
        }
    }
}
