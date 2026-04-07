# AppMonitor Dockerfile
# Multi-stage build for smaller image size

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first for better layer caching
COPY JavaMonitor.sln ./
COPY JavaMonitor.Core/JavaMonitor.Core.csproj ./JavaMonitor.Core/
COPY JavaMonitor.Web/JavaMonitor.Web.csproj ./JavaMonitor.Web/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY JavaMonitor.Core/ ./JavaMonitor.Core/
COPY JavaMonitor.Web/ ./JavaMonitor.Web/

# Build and publish
WORKDIR /src/JavaMonitor.Web
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Set timezone to Shanghai for consistent timestamps
ENV TZ=Asia/Shanghai
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

# Create data directory for SQLite
RUN mkdir -p /app/data

# Copy published files
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:8080/ || exit 1

# Run as non-root user for security
RUN addgroup --system --gid 1000 appmonitor && \
    adduser --system --uid 1000 --ingroup appmonitor --shell /bin/bash appmonitor
USER appmonitor

# Entry point
ENTRYPOINT ["dotnet", "JavaMonitor.Web.dll"]
