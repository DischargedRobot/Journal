# Используем оптимизированный multi-stage Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Сначала копируем только файлы проекта и восстанавливаем пакеты —
# это ускоряет кэширование слоёв, если код меняется часто.
COPY ["AuthService.csproj", "./"]
# восстанавливаем зависимости
RUN dotnet restore "AuthService.csproj"
COPY . .
# публикуем и кидаем сразу в папку /app,
# выполним restore ещё раз, чтобы гарантировать загрузку анализаторов/пакетов,
# затем выполним publish
RUN dotnet restore "AuthService.csproj" \
	&& dotnet publish "AuthService.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Копируем опубликованные артефакты
COPY --from=build /app .

EXPOSE 8000
# запускаем приложение
ENTRYPOINT ["dotnet", "AuthService.dll"]
