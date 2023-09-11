FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
ENV DOCKER_RUNNING "true"
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["LeitourApi.csproj", "LeitourApi/"]
RUN dotnet restore "LeitourApi/LeitourApi.csproj"
COPY . .
WORKDIR "/src/LeitourApi"

WORKDIR "/src/LeitourApi"
COPY . .

RUN dotnet build "LeitourApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LeitourApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LeitourApi.dll"]
