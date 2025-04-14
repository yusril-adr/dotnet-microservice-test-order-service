FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-backend
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# copy everything else and build
COPY . .
RUN dotnet publish -c Release -o out

# build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

RUN apt-get update \
  && apt-get install -y curl
COPY --from=build-backend /app/out .

CMD ["dotnet", "DotNetOrderService.dll"]

EXPOSE 8080
