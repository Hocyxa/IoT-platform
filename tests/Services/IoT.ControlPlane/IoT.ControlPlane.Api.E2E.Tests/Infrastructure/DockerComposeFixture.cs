
using System.Diagnostics;

namespace IoT.ControlPlane.Api.E2E.Tests.Infrastructure;

public sealed class DockerComposeFixture : IAsyncLifetime
{
    private const string ComposeFile = "docker-compose.e2e.yml";

    private readonly string _repositoryRoot;
    private readonly string _projectName;

    public HttpClient Client { get; }

    public DockerComposeFixture()
    {
        _repositoryRoot = FindRepositoryRoot();

        // Уникальное имя изолирует ресурсы Compose-проекта.
        _projectName = $"iot-controlplane-e2e-{Guid.NewGuid():N}";

        Client = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:18080"),
            Timeout = TimeSpan.FromSeconds(10)
        };
    }

    public async Task InitializeAsync()
    {
        try
        {
            await RunComposeAsync("up", "--build", "-d");
            await WaitForApiAsync();
        }
        catch
        {
            // Не оставляем контейнеры после неудачного запуска.
            await RunComposeAsync(
                "down", "-v", "--remove-orphans");

            throw;
        }
    }

    public async Task DisposeAsync()
    {
        try
        {
            await RunComposeAsync(
                "down", "-v", "--remove-orphans");
        }
        finally
        {
            Client.Dispose();
        }
    }

    private async Task WaitForApiAsync()
    {
        using var healthClient = new HttpClient
        {
            BaseAddress = Client.BaseAddress,
            Timeout = TimeSpan.FromSeconds(3)
        };

        var deadline = DateTime.UtcNow.AddMinutes(2);

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                using var response =
                    await healthClient.GetAsync("/health");

                if (response.IsSuccessStatusCode)
                {
                    return;
                }
            }
            catch (HttpRequestException)
            {
                // API ещё не принимает подключения.
            }
            catch (TaskCanceledException)
            {
                // Истёк таймаут отдельного запроса.
            }

            await Task.Delay(TimeSpan.FromSeconds(2));
        }

        throw new TimeoutException(
            "IoT.ControlPlane API did not become healthy.");
    }

    private async Task RunComposeAsync(params string[] arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "docker",
            WorkingDirectory = _repositoryRoot,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        startInfo.ArgumentList.Add("compose");
        startInfo.ArgumentList.Add("-p");
        startInfo.ArgumentList.Add(_projectName);
        startInfo.ArgumentList.Add("-f");
        startInfo.ArgumentList.Add(
            Path.Combine(_repositoryRoot, ComposeFile));

        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var process = Process.Start(startInfo)
            ?? throw new InvalidOperationException(
                "Failed to start Docker Compose.");

        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        var stdout = await stdoutTask;
        var stderr = await stderrTask;

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"""
                Docker Compose command failed.

                Arguments: {string.Join(" ", arguments)}

                STDOUT:
                {stdout}

                STDERR:
                {stderr}
                """);
        }
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(
            AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                Path.Combine(directory.FullName, ComposeFile)))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            $"Could not find {ComposeFile}.");
    }
}
