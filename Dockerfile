# Stage 1: Build & Publish
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy gRPC definitions and project files
COPY ["grpc/", "grpc/"]
COPY ["All Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/V-Eval-Identity_Service.API.csproj", "All Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/"]
COPY ["All Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/V-Eval-Identity_Service.Application.csproj", "All Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Application/"]
COPY ["All Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/V-Eval-Identity_Service.Domain.csproj", "All Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Domain/"]
COPY ["All Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/V-Eval-Identity_Service.Infrastructure.csproj", "All Services/V-Eval-Identity_Service/V-Eval-Identity_Service.Infrastructure/"]
RUN dotnet restore "All Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API/V-Eval-Identity_Service.API.csproj"

# Copy full source and publish Release artifact
COPY ["All Services/V-Eval-Identity_Service/", "All Services/V-Eval-Identity_Service/"]
WORKDIR "/src/All Services/V-Eval-Identity_Service/V-Eval-Identity_Service.API"
RUN dotnet publish "V-Eval-Identity_Service.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: ASP.NET Core Runtime Image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 5001
ENV ASPNETCORE_URLS=http://+:5001
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "V-Eval-Identity_Service.API.dll"]
