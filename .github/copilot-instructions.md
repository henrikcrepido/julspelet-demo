# GitHub Copilot Instructions for Julspelet Demo

## Project Overview

This is the Julspelet Demo project, a .NET application. When assisting with this repository, please follow these guidelines.

## Code Style and Conventions

### General Guidelines
- Follow C# naming conventions (PascalCase for classes and methods, camelCase for local variables)
- Use clear, descriptive variable and method names
- Add XML documentation comments for public APIs
- Keep methods focused and single-purpose
- Prefer LINQ for collection operations where appropriate

### .NET Specific
- Target modern .NET features (async/await, pattern matching, records, etc.)
- Use nullable reference types to prevent null reference exceptions
- Implement proper dependency injection patterns
- Follow SOLID principles
- Use the latest C# language features when appropriate

## Architecture Patterns

- Prefer dependency injection for loose coupling
- Separate concerns (business logic, data access, presentation)
- Use interfaces for abstraction
- Implement proper error handling and logging
- Write unit tests for business logic

## Testing

- Write unit tests using  NUnit
- Use descriptive test method names that explain the scenario
- Follow AAA pattern (Arrange, Act, Assert)
- Mock external dependencies
- Aim for meaningful test coverage

## Documentation

- Add XML documentation for public classes, methods, and properties
- Include examples in documentation when helpful
- Keep README.md updated with setup and usage instructions
- Document any non-obvious design decisions

## Security

- Never commit sensitive data (API keys, passwords, connection strings)
- Use environment variables for configuration
- Validate all user inputs
- Follow security best practices for web applications

## Git Workflow

- Write clear, descriptive commit messages
- Keep commits focused on single changes
- Use feature branches for new development
- Squash commits when appropriate before merging

## Helpful Context

- This is a demo project showcasing best practices
- Prioritize code clarity and maintainability
- Suggest improvements when you see opportunities
- Help identify potential bugs or security issues

## When Suggesting Code

- Provide complete, working examples
- Explain why you're suggesting a particular approach
- Consider performance implications
- Ensure code is testable and maintainable
- Include necessary using statements and dependencies

## Questions to Consider

When helping with code, consider:
- Is this the most maintainable solution?
- Are there any edge cases to handle?
- Should this include error handling?
- Would this benefit from unit tests?
- Is the naming clear and self-documenting?
