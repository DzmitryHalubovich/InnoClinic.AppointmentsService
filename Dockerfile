#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Appointments.API/Appointments.API.csproj", "src/Appointments.API/"]
COPY ["src/Appointments.Contracts/Appointments.Contracts.csproj", "src/Appointments.Contracts/"]
COPY ["src/Appointments.Domain/Appointments.Domain.csproj", "src/Appointments.Domain/"]
COPY ["src/Appointments.Infrastructure/Appointments.Infrastructure.csproj", "src/Appointments.Infrastructure/"]
COPY ["src/Appointments.Presentation/Appointments.Presentation.csproj", "src/Appointments.Presentation/"]
COPY ["src/Appointments.Services.Abstractions/Appointments.Services.Abstractions.csproj", "src/Appointments.Services.Abstractions/"]
COPY ["src/Appointments.RabbitMQ/Appointments.RabbitMQ.csproj", "src/Appointments.RabbitMQ/"]
COPY ["src/Appointments.Services/Appointments.Services.csproj", "src/Appointments.Services/"]
RUN dotnet restore "./src/Appointments.API/Appointments.API.csproj"
COPY . .
WORKDIR "/src/src/Appointments.API"
RUN dotnet build "./Appointments.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Appointments.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Appointments.API.dll"]