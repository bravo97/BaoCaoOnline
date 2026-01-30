using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.SqlClient;
using System;

namespace Infrastructure.Factories
{
    public class DatabaseConnectionFactory : IDatabaseConnectionFactory
    {
        private readonly IDataProtector _protector;

        public DatabaseConnectionFactory(IDataProtectionProvider dataProtectionProvider)
        {
            // Must match the purpose string used in FileCustomerRepository
            _protector = dataProtectionProvider.CreateProtector("FileCustomerRepository.Password");
        }

        public string CreateConnectionString(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            string plainPassword = customer.Password;
            if (!string.IsNullOrEmpty(customer.Password))
            {
                try
                {
                    // Attempt to decrypt
                    plainPassword = _protector.Unprotect(customer.Password);
                }
                catch
                {
                    // If decryption fails, assume it's already plain text (fall back)
                    plainPassword = customer.Password;
                }
            }

            // Manually construct connection string to avoid SqlConnectionStringBuilder length validation issues
            // with encrypted or very long passwords (if they happen to be used).
            // Also ensures consistency across the application.
            var connStr = $"Data Source={customer.ServerName};Initial Catalog={customer.DatabaseName};User ID={customer.UserName};Password={plainPassword};MultipleActiveResultSets=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;";

            return connStr;
        }
    }
}
