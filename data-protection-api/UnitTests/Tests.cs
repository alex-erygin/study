using System;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace UnitTests
{
    public class Tests
    {
        [Fact]
        void ProtectInMemoryData_ShouldBeProtected()
        {
            var data = Encoding.UTF8.GetBytes("Your secret pass");

            Console.WriteLine($"Original data: {Encoding.UTF8.GetString(data)}");

            ProtectedMemory.Protect(data, MemoryProtectionScope.SameProcess);
            Console.WriteLine($"Protected data: {Encoding.UTF8.GetString(data)}");

            ProtectedMemory.Unprotect(data, MemoryProtectionScope.SameProcess);
            Console.WriteLine($"Unprotected data: {Encoding.UTF8.GetString(data)}");
        }
    }
}