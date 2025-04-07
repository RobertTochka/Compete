# Используйте базовый образ для выполнения приложения
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 444

# Используйте образ SDK для сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Копируйте файлы проекта и восстановите зависимости для Compete-POCO-Models
COPY ["Compete-POCO-Models/Compete POCO Models.csproj", "Compete-POCO-Models/"]
COPY ["compete-platform/compete-platform.csproj", "compete-platform/"]
RUN dotnet restore "compete-platform/compete-platform.csproj"

# Копируйте остальные файлы и билдите приложение
COPY . .
WORKDIR "/src/compete-platform"
RUN dotnet build "compete-platform.csproj" -c Release -o /app/build

# Публикация приложения
FROM build AS publish
RUN dotnet publish "compete-platform.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Финальный образ для запуска приложения
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "compete-platform.dll"]
