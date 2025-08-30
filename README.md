# Developer Evaluation Project

This repository contains the solution for the **Developer Evaluation Project**, built with **.NET 8**, applying **DDD (Domain-Driven Design)** and clean architecture principles.  

## 🚀 Tech Stack
- **.NET 8 / C#**
- **Entity Framework Core**
- **PostgreSQL & MongoDB**
- **Docker & Docker Compose**
- **xUnit** for unit testing
- **AutoMapper**, **Mediator Pattern**, **NSubstitute**

## 📦 How to Setup

### Using Docker (recommended)
```bash
docker-compose up --build
```
The API will be available at:  
👉 http://localhost:5000

### Using .NET CLI
1. Go to backend folder:
   ```bash
   cd template/backend/src/Ambev.DeveloperEvaluation.WebApi
   ```
2. Run the API:
   ```bash
   dotnet run
   ```

## 🧪 Running Tests
```bash
dotnet test
```

## 📖 Documentation
API documentation and requirements can be found in the `.doc/` folder.  
