#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
# install package to build
RUN apt-get update && apt-get install -y \
    libopenblas-dev \
    liblapack-dev \
    libx11-6 \
    libgdiplus \
    && apt-get clean && rm -rf /var/lib/apt/lists/*
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Beholder/nuget.config", "Beholder/"]
COPY ["Beholder/Beholder.csproj", "Beholder/"]
RUN dotnet restore "Beholder/Beholder.csproj"
COPY . .
WORKDIR "/src/Beholder"
RUN dotnet build "Beholder.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Beholder.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Beholder.dll"]