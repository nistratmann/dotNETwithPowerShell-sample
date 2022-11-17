using System.Management.Automation;
using System.Management.Automation.Runspaces;

/// Little program to demonstrate that running both PowerShell 5 and 7 from within .NET 6 is possible
/// 
/// Important is to create an OutOfProcessRunspace, thanks to https://stackoverflow.com/a/74317737
/// 
/// The motivation would be to communicate with legacy systems such as i.e. MS System Center products
///  using PS cmdlets which are not compatible with PowerShell 7

Console.WriteLine("Hello, PowerShell power user and dotnet developer!");

var script = "whoami; $PSVersionTable.PSVersion.ToString()";

Console.WriteLine("Testing powershell sdk to execute PowerShell Core 7...");

using var ps7instance = PowerShell.Create();

ps7instance.AddScript(script);
var ps7result = ps7instance.Invoke();

foreach (var result in ps7result)
{
    Console.WriteLine(result.BaseObject.ToString());
}


Console.WriteLine("Testing powershell sdk to execute Windows PowerShell 5.1...");

/// This is the crucial part!
/// Not using the default runspace, but rather create a separate process and use its runspace
/// Naturally, it will only work on Windows OS with Windows PowerShell 5.1 installed ;) 
using var ps5runspace = RunspaceFactory.CreateOutOfProcessRunspace(
    new TypeTable(Array.Empty<string>()),
    new PowerShellProcessInstance(new Version(5, 1), null, null, false)
);

ps5runspace.Open();

using var ps5instance = PowerShell.Create(ps5runspace);
 
ps5instance.AddScript(script);
var ps5result = ps5instance.Invoke();

foreach(var result in ps5result)
{
    Console.WriteLine(result.BaseObject.ToString());
}

ps5runspace.Close();

Console.WriteLine("Finished!! Press any key to exit");
Console.ReadKey();