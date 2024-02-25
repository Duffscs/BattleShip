FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish

COPY . .

RUN dotnet restore

RUN dotnet publish ./BattleShip.App/BattleShip.App.csproj -c Release -o ./publish

FROM nginx:alpine AS final

COPY --from=publish /publish/wwwroot /usr/local/webapp/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 80