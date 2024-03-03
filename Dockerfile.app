FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish

COPY . .

RUN dotnet tool install Excubo.WebCompiler --global

RUN dotnet restore

RUN dotnet publish ./BattleShip.App/BattleShip.App.csproj -c Release -o ./publish

FROM nginx:alpine AS final

COPY config/setup_env.sh /docker-entrypoint.d/35-setup_env.sh
COPY --from=publish /publish/wwwroot /usr/local/webapp/nginx/html

EXPOSE 80
EXPOSE 443