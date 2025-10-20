# Copilot Instructions for RengaDyn

## Project Overview
RengaDyn is a set of C# nodes (functional methods) for Dynamo Core, designed to interact with the Renga CAD environment via the Renga API. The project enables geometry extraction, property management, and object selection, mirroring Renga API capabilities. The codebase is organized for parallel work with Renga processes and model elements.

## Architecture & Key Components
- **src/**: Main C# source code, including `RengaDynMain.csproj` and core logic.
- **Control/**: UI and web control components, models, and services.
- **docs/**: Comprehensive documentation, including API guides (`api/`), workflows (`workflows/`), troubleshooting (`troubleshooting/`), and developer notes (`notes/`).
- **examples/**, **dyn/**: Example scripts and usage scenarios for Dynamo and Renga integration.
- **packages/**: External dependencies, including Dynamo and .NET libraries.

## Essential Workflows
- **Build**: Use the following command (PowerShell syntax) to build the main project:
  ```pwsh
  & "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" "src\RengaDynMain.csproj" /p:Configuration=Debug /p:Platform=x64 /verbosity:minimal
  ```
- **Debugging**: Do NOT output debug info to the console log. Instead, output to the second output (DebugInfo) of the node.
- **API Integration**: Always use [Context7](https://context7.com/websites/help_rengabim_api) for Renga API work.
- **Wrapper Management**: Always add new wrappers to the project before building.

## Project-Specific Conventions
- **Documentation**: Place API references in `docs/api/`, workflows in `docs/workflows/`, troubleshooting in `docs/troubleshooting/`, and notes in `docs/notes/`. Update `docs/README.md` for major changes.
- **Debug Output**: Never use console for debug output; use node's DebugInfo output.
- **External SDK**: RengaSDK is required for building and running; obtain from [RengaBIM](https://rengabim.com/sdk/).
- **Dynamo Integration**: For development, import `RengaDynMain.dll` into Dynamo and configure project to start with `DynamoSandbox.exe`.

## Troubleshooting & Common Issues
- See `docs/troubleshooting/` for solutions to common problems (e.g., COM errors, null outputs, baseline issues).
- Reference workflow guides in `docs/workflows/` for step-by-step implementation help.

## Examples & Patterns
- Example scripts are in `dyn/` and `examples/`.
- Service and model patterns are in `Control/Services/` and `Control/Models/`.

## Quick Start for AI Agents
- Start by reading `README.md` and `docs/README.md` for orientation.
- Follow build and debug conventions above.
- Use documentation in `docs/` for API, workflows, and troubleshooting.
- Adhere to project-specific output and wrapper conventions.

---
For questions or unclear conventions, review the latest documentation in `docs/` or ask for clarification from maintainers.
