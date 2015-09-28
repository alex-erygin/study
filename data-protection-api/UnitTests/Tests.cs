using System;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace UnitTests
{
    public class Tests
    {
        [Fact]
        void ProtectedMemory_ShouldBeProtected()
        {
			//длина буфера должна быть кратна 16
			var data = Encoding.UTF8.GetBytes("Your secret pass");

            Console.WriteLine($"Original data: {Encoding.UTF8.GetString(data)}");

            ProtectedMemory.Protect(data, MemoryProtectionScope.SameProcess);
            Console.WriteLine($"Protected data: {Encoding.UTF8.GetString(data)}");

            ProtectedMemory.Unprotect(data, MemoryProtectionScope.SameProcess);
            Console.WriteLine($"Unprotected data: {Encoding.UTF8.GetString(data)}");
        }

		[Fact]
	    void ProtectedData_ShouldBeProtected()
		{
			//буффер может быть любой длины.
			var data = Encoding.UTF8.GetBytes("Содержимое секретного файла.");
			Console.WriteLine($"Original data: {Encoding.UTF8.GetString(data)}");

			var optionalEntropy = new byte[16];
			new RNGCryptoServiceProvider().GetNonZeroBytes(optionalEntropy);

			var protectedData = ProtectedData.Protect(data, optionalEntropy, DataProtectionScope.CurrentUser);
			Console.WriteLine($"Protected data: {Encoding.UTF8.GetString(protectedData)}");

			var unprotectedData = ProtectedData.Unprotect(protectedData, optionalEntropy, DataProtectionScope.CurrentUser);
			Console.WriteLine($"Unprotected data: {Encoding.UTF8.GetString(unprotectedData)}");
		}
    }
}