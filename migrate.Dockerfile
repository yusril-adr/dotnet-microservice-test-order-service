FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-backend
WORKDIR /app

ENV PATH="$PATH:/root/.dotnet/tools"

COPY . .
RUN dotnet tool install --global dotnet-ef \
  && dotnet build
