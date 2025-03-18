# Используем официальный SDK-образ для сборки
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Копируем все файлы проекта
COPY . ./

# Публикуем приложение
RUN dotnet publish -c Release -o out

# Используем Runtime-образ для запуска
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out .

# Порт, который будет слушать Render
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Запуск
ENTRYPOINT ["dotnet", "DrawTogether.dll"]
