using System;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using DangerousLib;
using Microsoft.Win32;

namespace RunCodeInSandboxApp
{
	class Program : MarshalByRefObject
	{
		static void Main(string[] args)
		{
			//fails - no permissions
			//RunWithPermissions(Enumerable.Empty<IPermission>().ToArray());

			//fails - no registry permission
			//RunWithPermissions(new SecurityPermission(SecurityPermissionFlag.Execution));

			//ok - all required permissions
			RunWithPermissions(new SecurityPermission(SecurityPermissionFlag.Execution),
				new RegistryPermission(RegistryPermissionAccess.Read, Registry.CurrentConfig.Name));

			Console.ReadKey();
		}

		private static void RunWithPermissions(params IPermission[] permissions)
		{
			var setup = new AppDomainSetup { ApplicationBase = Environment.CurrentDirectory };
			var permissionSet = new PermissionSet(null);
			foreach (var permission in permissions)
			{
				permissionSet.AddPermission(permission);
			}

			var appDomain = AppDomain.CreateDomain("SANDBOX domain", null, setup, permissionSet);
			var p = (Program)(Activator.CreateInstance(appDomain, "RunCodeInSandboxApp", "RunCodeInSandboxApp.Program").Unwrap());
			p.RunDangerousMethod();
		}

		private void RunDangerousMethod()
		{
			new RegistryReader().ReadRegistry();
		}
	}
}