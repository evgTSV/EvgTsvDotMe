### What's inside

- F# (Oxpecker)
- Oxpecker ViewEngine
- HTMX
- Tailwind CSS
- Custom Type Providers

### Quick Start with Windows

0. **Install** [.NET SDK 10](https://dotnet.microsoft.com/en-us/download)


1. **Build and prepare the environment**

```bash
./build.ps1
```
2. **Run in Docker**

```bash
docker compose up
```

### Quick Start with Unix

0. **Install** [.NET SDK 10](https://dotnet.microsoft.com/en-us/download)


1. **Build and prepare the environment**

```bash
./build.sh
```

2. **Run in Docker**

```bash
docker compose up
```

### Generate GitHub workflows

```bash
dotnet fsi ./Scripts/buildAndDeploy.fsx
```

### Code formatting

```bash
dotnet fantomas .
```
