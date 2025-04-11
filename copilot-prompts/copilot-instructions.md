# Project-Specific Coding Guidelines

## Project Overview

This is a .NET 8.0 service project using:

- MSSQL 2019 database
- Entity Framework Core 8

## Code Architecture

### Directory Structure

- `Models/` - Data models
- `Domain/Repositories/` - Database repositories use for database operations
- `Domain/Services/` - Business logic services
- `Domain/Dtos/` - Data transfer objects
- `Http/API/Version1/` - HTTP-related implementations for each domain controller
- `Infrastructure/Shareds/` - Shared services and utilities

### Models

- Base model is a class that contains the common properties for all models. The properties are `Id`, `CreatedAt`, `UpdatedAt`, `DeletedAt`
- All models should inherit from the base model

### Dtos

- Dto file are separated file for each functionality. Example: `{Domain}QueryDto.cs`, `{Domain}CreateDto.cs`, `{Domain}UpdateDto.cs`, `{Domain}ResultDto.cs`.
- Dto file for query param should be named as `{Domain}QueryDto`.
- Dto query should inherit from `QueryDto` class located in `Infrastructure/Shareds/QueryDto.cs`.
- Dto file for body request should be named as `{Domain}CreateDto` or `{Domain}UpdateDto` as the case may be. And must have validation attributes using `System.ComponentModel.DataAnnotations` namespace.
- Dto file for result should be named as `{Domain}ResultDto`. This file should contain the properties that will be returned to the client
- Add class for mapping all Dto except `{Domain}QueryDto`. Ex: `{Domain}ResultDto`, `{Domain}CreateDto`, `{Domain}UpdateDto` in the same file as Dto file.

### Repositories

- Repository file are separated file for each functionality, Query and Store. Named `{Domain}QueryRepository` and `{Domain}StoreRepository` respectively
- Query repository should only contain methods for read related query data from the database
- Store repository should only contain methods for write related query data from the database
- No need error handling in repository
- Instead using `Get` for find data, prefer using `Find` for function name
- For efficient query use `AnyAsync` for checking data exist or not

### Pagination Repository

- For CRUD features should have pagination for data list with filter from `{Domain}QueryRepository`
- Function name is Pagination, located in `{Domain}QueryRepository`
- Filtering use
  - `QuerySearch();` for search
  - `QueryFilter();` for filter
  - `QuerySort();` for sort
- Return data should be using `PaginationResult` class located in `Infrastructure/Shareds/PaginationResult.cs`

### Services

- Service file are separated file for each functionality, Named `{Domain}Service`
- Service should only contain business logic and error handling
- Service should return Dto class based on the operation. Example: `{Domain}CreateDto`, `{Domain}UpdateDto`, `{Domain}ResultDto`.
- Service should not return API response, it should return the data to the controller

### Controllers

- Controller file are separated file for each functionality, Named `{Domain}Controller`
- Endpoint name should be named as `{Domain}` with plural. Example: `users`
- The return must be using `ApiResponseData` class and `ApiResponsePagination` for pagination data. Located in `Infrastructure/Shareds/ApiResponse.cs` and the abstract class must be filled with `DtoResult`. Don't use `return Ok()`
  - Example: `return new ApiResponseData<UserResultDto>(HttpStatusCode.OK, data);`
- No need define `[Authorize]` attribute in controller, it should be defined in the route configuration

### Error Handling

Exceptions:

- Use `BusinessException` for business logic errors
- Uaw `DataNotFoundException` for missing data or null data
- Use `FileNotFoundException` for missing files
- Use `UnautenticatedException` for unauthorized access
- Use `UnauthorizedException` for unauthorized operations
- Handle storage provider errors appropriately

### Best Practices

1. Use async/await for I/O operations
2. Implement proper error handling
3. Use dependency injection
4. No need define interface for repository and service classes
5. Follow existing naming patterns:
   - PascalCase for classes and methods
   - camelCase for variables
   - snake_case for JSON properties
6. Use constants from Constants directory
7. Include proper logging
8. Implement proper file cleanup
9. Use configuration-based settings
