# 📦 How to add NuGet source

To install packages from the `Nevermindjq` GitHub feed, you need to add the custom NuGet source to your system.

There are two main ways to do this: via the **.NET CLI** or by editing the **NuGet configuration file**.

***

## ✅ Option 1: Add Source via .NET CLI

Run the following command in your terminal:

```bash
dotnet nuget add source --name GitHub/Nevermindjq "https://nuget.pkg.github.com/nevermindjq/index.json"
```

***

## ⚙️ Option 2: Add Source via `NuGet.config`

Edit your local or global `NuGet.config` file and add the source manually:

```xml
<configuration>
  <packageSources>
    <add key="GitHub/Nevermindjq" value="https://nuget.pkg.github.com/Nevermindjq/index.json" />
  </packageSources>
</configuration>
```

You can find `NuGet.config` in:

* Project root (local config)
* `%AppData%\NuGet\NuGet.config` (Windows, global config)
* `~/.nuget/NuGet/NuGet.Config` (Linux/macOS)

***

## ✅ Test the Source

You can verify the custom source is working by listing available packages:

```bash
dotnet nuget list source
```

Or try installing a package:

```bash
dotnet add package Nevermindjq.Telegram.Bot --source GitHub/Nevermindjq
```
