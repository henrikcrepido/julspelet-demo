# GitHub Copilot Instructions for Julspelet Demo

## Project Overview

This is the Julspelet Demo project, a .NET Core 9 Blazor application. When assisting with this repository, please follow these guidelines.

### Developer Context
**Learning Journey**: The developer is experienced with .NET Core backend development but new to Blazor. When providing assistance:
- Focus on explaining Blazor-specific concepts (frontend patterns differ from backend)
- Break down complex Blazor patterns into understandable pieces
- Provide links to official documentation when introducing new Blazor concepts
- Include comments explaining Blazor-specific code (component lifecycle, rendering, state management)
- Highlight differences between backend patterns and Blazor frontend patterns
- Compare approaches when multiple solutions exist, explaining pros and cons
- Leverage existing .NET Core knowledge while teaching Blazor fundamentals

## Workflow Protocol

### Plan-First Approach
**CRITICAL**: Never write or modify code without explicit developer approval.

When receiving any coding request:

1. **Present a Phased Implementation Plan**
   - Break down the work into logical phases
   - Number each phase clearly (Phase 1, Phase 2, etc.)
   - Describe what will be accomplished in each phase
   - List specific files that will be created or modified
   - Estimate complexity (Simple/Medium/Complex)

2. **Wait for Approval**
   - Present the plan and ask: "Would you like me to proceed with this plan?"
   - Do NOT implement anything until the developer explicitly approves
   - Be ready to adjust the plan based on feedback

3. **Execute Phases Sequentially**
   - Implement one phase at a time
   - After completing each phase, provide a **Completion Report** that includes:
     - ✅ Phase number and title
     - ✅ What was implemented
     - ✅ Files created/modified
     - ✅ Any issues encountered
     - ✅ Next steps
   - Wait for acknowledgment before proceeding to the next phase

### Example Plan Format

```
## Implementation Plan for [Feature Name]

**Phase 1: Project Setup** (Simple)
- Create project structure
- Add necessary NuGet packages
- Files: *.csproj, Program.cs

**Phase 2: Core Implementation** (Medium)
- Implement main business logic
- Add interfaces and models
- Files: Services/, Models/

**Phase 3: Testing** (Medium)
- Write unit tests
- Add integration tests
- Files: Tests/

Would you like me to proceed with this plan?
```

### Completion Report Format

```
## ✅ Phase [X] Completion Report

**Completed**: [Phase Title]
**Status**: ✅ Complete

**What was done**:
- Item 1
- Item 2

**Files modified**:
- path/to/file1.cs
- path/to/file2.cs

**Next**: Ready to proceed with Phase [X+1] - [Title]
```

## Code Style and Conventions

### General Guidelines
- Follow C# naming conventions (PascalCase for classes and methods, camelCase for local variables)
- Use clear, descriptive variable and method names
- Add XML documentation comments for public APIs
- Keep methods focused and single-purpose
- Prefer LINQ for collection operations where appropriate

### .NET Core 9 & Blazor Specific
- Target .NET 9 features and explain new capabilities when using them
- Use nullable reference types to prevent null reference exceptions
- Implement proper dependency injection patterns (explain service lifetimes)
- Follow SOLID principles
- Use the latest C# language features when appropriate and explain their benefits

### Blazor Best Practices
- Explain component lifecycle methods (@code, OnInitialized, OnParametersSet, etc.)
- Clarify render modes (Static SSR, Interactive Server, Interactive WebAssembly, Auto)
- Demonstrate proper state management (cascading parameters, state containers)
- Show form validation patterns and data binding (@bind, @bind-value)
- Include JavaScript interop examples with explanations when needed
- Explain component communication (parameters, EventCallback, cascading values)
- Use Blazor's built-in components when possible (NavLink, Router, etc.)
- Add comments explaining Razor syntax and directives (@page, @inject, @using, etc.)

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
