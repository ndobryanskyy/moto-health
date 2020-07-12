#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["MotoHealth.Bot/MotoHealth.Bot.csproj", "MotoHealth.Bot/"]
COPY ["MotoHealth.Core/MotoHealth.Core.csproj", "MotoHealth.Core/"]
COPY ["MotoHealth.Telegram/MotoHealth.Telegram.csproj", "MotoHealth.Telegram/"]
COPY ["MotoHealth.Infrastructure/MotoHealth.Infrastructure.csproj", "MotoHealth.Infrastructure/"]
COPY ["MotoHealth.PubSub/MotoHealth.PubSub.csproj", "MotoHealth.PubSub/"]
RUN dotnet restore "MotoHealth.Bot/MotoHealth.Bot.csproj"
COPY . .
WORKDIR "/src/MotoHealth.Bot"
RUN dotnet build "MotoHealth.Bot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MotoHealth.Bot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MotoHealth.Bot.dll"]