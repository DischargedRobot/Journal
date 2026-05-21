# используем официальный образ .NET SDK для сборки приложения
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# копируем файл проекта 
COPY ["AuthService.csproj", "./"]
# восстанавливаем зависимости
RUN dotnet restore "AuthService.csproj"
COPY . .
# публикуем и кидаем сразу в папку /app, 
# чтобы не тащить весь исходный код в финальный образ
RUN dotnet publish "AuthService.csproj" -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
# настраиваем переменную окружения для прослушивания на порту 8000
COPY --from=build /app .а
# запускаем приложение
ENTRYPOINT ["dotnet", "AuthService.dll"]
