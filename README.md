# Julspelet Demo

Welcome to the Julspelet Demo repository! This project is a demonstration application built with .NET.

## Getting Started

### Prerequisites

- [.NET SDK](https://dotnet.microsoft.com/download) (version 6.0 or later recommended)
- A code editor (Visual Studio, Visual Studio Code, or Rider)
- Git for version control

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/henrikcrepido/julspelet-demo.git
   cd julspelet-demo
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

### Running the Application

To run the application locally:

```bash
dotnet run
```

### Development

#### Project Structure

```
julspelet-demo/
├── .gitignore          # Git ignore rules
└── README.md           # This file
```

#### Building

To build the project in Release mode:

```bash
dotnet build --configuration Release
```

#### Testing

Run tests with:

```bash
dotnet test
```

### Contributing

1. Create a new branch for your feature or bugfix
2. Make your changes
3. Test your changes thoroughly
4. Submit a pull request with a clear description of your changes

### Environment Variables

If your application requires environment variables, create a `.env` file in the root directory. This file is already included in `.gitignore` for security.

### Troubleshooting

- **Build errors**: Ensure you have the correct .NET SDK version installed
- **Dependency issues**: Try running `dotnet restore --force` to force re-download of packages
- **Port conflicts**: Check if another application is using the same port

### License

Please add your license information here.

### Contact

For questions or support, please open an issue in the GitHub repository.

---

**Note**: This is a demo project. Please update this README with specific details about your application as development progresses.
