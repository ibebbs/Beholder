FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
# install DlibDotNetNative dependencies
RUN apt-get update && apt-get install -y \
    libopenblas-dev \
    liblapack-dev \
    libx11-6 \
    libgdiplus \
    && apt-get clean && rm -rf /var/lib/apt/lists/*
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src


COPY ["Director.Client/Director.Client.csproj", "Director.Client/"]
RUN dotnet restore "Director.Client/Director.Client.csproj"
COPY ["Beholder/Beholder.csproj", "Beholder/"]
COPY ["Beholder/nuget.config", "Beholder/"]
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
