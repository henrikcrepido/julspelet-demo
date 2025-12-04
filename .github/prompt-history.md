# Prompt History

This file tracks the key prompts and requirements given during the development of the Julspelet Demo project.

## Initial Setup (December 4, 2025)

### 1. Initial Instructions File
**Request**: "write initial instruction md file for repo"
- Created initial README.md with project setup instructions

### 2. Copilot Instructions
**Request**: "create a copilot instruction file for repo"
- Created `.github/copilot-instructions.md` with coding guidelines

### 3. Workflow Protocol
**Request**: "update copilot instruction to always present a plan with phases and give option to implement them. no coding until approved by developer. when performing phases do completion reports"
- Added Plan-First Approach workflow
- Implemented phased implementation with approval gates
- Added completion report requirements

### 4. Learning Context
**Request**: "i want to learn to develop .net core 9 blazor and im new to it. include this in instruction"
- Added Developer Context section explaining learning journey
- Included educational focus in instructions

**Follow-up**: "im not new in .net core development but used to backend"
- Refined Developer Context to reflect backend experience
- Focus on Blazor-specific concepts rather than general .NET Core

### 5. Technology Stack
**Request**: "i want this app to be a blazor server app using mudblazor"
- Updated project description to Blazor Server
- Added MudBlazor guidelines section
- Included MudBlazor component preferences

### 6. Application Requirements
**Request**: "add section for the requirements for the app that is a game for 2 or several people. save the prompts that i given in a md file history"
- Added Application Requirements section specifying multiplayer game
- Created this prompt-history.md file to track all requests

### 7. Player Join Mechanism
**Request**: "users will join by name to the game."
- Added requirement that users join by entering their name
- Updated application requirements in copilot instructions

### 8. Game Type and Rules
**Request**: "the game is a yatzy tournament. only 1 rounds. 3 throw of dice for each user. resulting in a scoreboard"
- Specified game type as Yatzy tournament
- Single round format
- 3 throws of dice per player per turn
- Scoreboard to display results
- Updated application requirements with game rules

### 9. UI Theme
**Request**: "i want a christmas theme"
- Added Christmas theme requirement for UI and styling
- Updated MudBlazor guidelines to include festive colors and holiday styling

---

## Requirements Summary

### Application Type
- Yatzy tournament game for 2 or more players
- Single round format
- Users join by entering their name
- Each player gets 3 throws of dice per turn
- Scoreboard displays results
- Christmas theme for UI and styling
- Built with .NET Core 9 Blazor Server
- UI using MudBlazor component library

### Developer Profile
- Experienced with .NET Core backend development
- Learning Blazor frontend patterns
- Prefers phased implementation with approval gates

### Workflow
- Plan-first approach with no coding before approval
- Phased implementation with completion reports
- Educational explanations for Blazor concepts
