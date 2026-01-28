# Device Monitor Agent

![.NET](https://img.shields.io/badge/.NET-9.0-blueviolet)
![Database](https://img.shields.io/badge/Database-LiteDB-informational)

A cross-platform monitoring agent built with .NET following Clean Architecture principles. This project simulates a device monitoring and management system.

## Architecture 

The project follows **Clean Architecture** with clear separation:
- **Domain** - Business models 
- **Application** - Business logic and interfaces
- **Infrastructure** - Provider implementations
- **Worker** - Service host and entry point

## Idea

A monitoring agent that:
- Runs as a background service on Windows/Linux
- Registers with a central platform on first run
- Collects device status every 5 minutes
- Polls for tasks every 1 minute
- Routes tasks to appropriate device providers
- Supports pluggable providers for different device types

## Technologies

- .NET 
- Worker Services
- Clean Architecture
- Provider Pattern
- Dependency Injection


**Note:** This is a learning project built to understand patterns used in production monitoring systems like C34C.
