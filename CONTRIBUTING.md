# Contributing Guide

This project follows a **clean, small, and test-first** workflow to keep changes easy to review and ship.

## Prerequisites
- .NET 8 SDK
- Docker & Docker Compose
- Git

## Run the project
- **Docker (recommended):**
  ```bash
  docker-compose up --build
  ```
  API: http://localhost:5000
- **.NET CLI:**
  ```bash
  cd template/backend/src/Ambev.DeveloperEvaluation.WebApi
  dotnet run
  ```

## Branching
- `main` – stable code.
- Feature branches: `feat/<short-name>`
- Fix branches: `fix/<short-name>`
- Chore/Docs/Refactor: `chore/<name>`, `docs/<name>`, `refactor/<name>`

## Commit messages (Conventional Commits)
Use:
- `feat: ...` new feature
- `fix: ...` bug fix
- `docs: ...` documentation
- `test: ...` tests only
- `refactor: ...` no new features/bugs
- `chore: ...` tooling/infra

Example:
```
feat(users): add pagination to list endpoint
```

## Code style
- Follow `.editorconfig` and C# conventions.
- Keep public APIs documented (XML docs where needed).
- Small PRs (≤ 300 LOC changed) when possible.

## Tests
- Write/adjust unit tests for each change.
- Run:
  ```bash
  dotnet test
  ```

## Pull Requests
- Link issue or describe the context.
- Checklist:
  - [ ] Build passes locally
  - [ ] Tests added/updated
  - [ ] API/README updated when needed
  - [ ] No secrets/keys committed
