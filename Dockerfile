FROM node:18 as build-vue
WORKDIR /app/frontend
COPY ./Frontend/package*.json ./
RUN npm install

COPY ./Frontend/ .
RUN npm run build

# Backend

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

COPY Backend/*.sln ./
COPY Backend/webapi/*.csproj ./webapi/
COPY Backend/webapi.DAL/*.csproj ./webapi.DAL/
COPY Backend/webapi.DL/*.csproj ./webapi.DL/
COPY Backend/ThatsTime.Tests/*.csproj ./ThatsTime.Tests/
RUN dotnet restore

COPY Backend/ ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .
COPY --from=build-vue /app/frontend/dist ./wwwroot


EXPOSE 80
EXPOSE 443
ENTRYPOINT ["dotnet", "webapi.dll", "http://+:80"]