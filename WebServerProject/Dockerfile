# 1단계: 빌드
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY . ./
RUN dotnet publish -c Release -o out

# 2단계: 실행
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app

COPY --from=build /app/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "WebServerProject.dll"]