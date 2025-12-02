# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files first for better layer caching
COPY ["FriendShare.Web/FriendShare.Web.csproj", "FriendShare.Web/"]
COPY ["FriendShare.Core/FriendShare.Core.csproj", "FriendShare.Core/"]

# Restore dependencies
RUN dotnet restore "FriendShare.Web/FriendShare.Web.csproj"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/FriendShare.Web"
RUN dotnet build -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

EXPOSE 8080

COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "FriendShare.Web.dll"]
