/*
 * Workvivo MCP Server - Model Context Protocol server for Workvivo API
 * Copyright (C) 2026  [Joakim Westin/Joakim Westin AB]
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 */
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModelContextProtocol;
using Workvivo.Shared.Configuration;
using Workvivo.Shared.Services;

// Build configuration from appsettings.json, user-secrets, and environment variables
var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true)
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables(prefix: "WORKVIVO_")
    .Build();

// Bind and validate settings
var settings = new WorkvivoSettings();
configuration.Bind(settings);

try
{
    settings.Validate();
}
catch (InvalidOperationException ex)
{
    // Log to stderr (CRITICAL for STDIO transport - never write to stdout)
    Console.Error.WriteLine($"Configuration Error: {ex.Message}");
    Console.Error.WriteLine();
    Console.Error.WriteLine("Please configure your credentials using one of these methods:");
    Console.Error.WriteLine("1. Environment variables: WORKVIVO_APITOKEN, WORKVIVO_ORGANIZATIONID, WORKVIVO_BASEURL");
    Console.Error.WriteLine("2. User secrets: dotnet user-secrets set \"ApiToken\" \"your-token\"");
    Console.Error.WriteLine("3. appsettings.json file (not recommended for credentials)");
    return 1;
}

// Configure the MCP server
var builder = Host.CreateEmptyApplicationBuilder(settings: null);

builder.Services.AddSingleton(settings);
builder.Services.AddHttpClient<IWorkvivoApiClient, WorkvivoApiClient>();

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

var app = builder.Build();

// Run the MCP server
await app.RunAsync();

return 0;
