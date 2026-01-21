FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ToDoApp.Api/ToDoApp.Api.csproj", "ToDoApp.Api/"]
COPY ["ToDoApp.Application/ToDoApp.Application.csproj", "ToDoApp.Application/"]
COPY ["ToDoApp.Domain/ToDoApp.Domain.csproj", "ToDoApp.Domain/"]
COPY ["ToDoApp.Infrastructure/ToDoApp.Infrastructure.csproj", "ToDoApp.Infrastructure/"]
RUN dotnet restore "ToDoApp.Api/ToDoApp.Api.csproj"
COPY . .
WORKDIR "/src/ToDoApp.Api"
RUN dotnet build "ToDoApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ToDoApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ToDoApp.Api.dll"]
