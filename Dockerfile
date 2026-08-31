# Stage 1: SDK environment (Used for building and testing)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["AcasSimulator.csproj", "./"]
RUN dotnet restore

# Copy remaining source code
COPY . .

# Run Unit Tests inside the Linux container
FROM build AS testrunner
CMD ["dotnet", "test", "--logger:console"]

# Default stage: Publish built DLL
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish