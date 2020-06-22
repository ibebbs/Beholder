FROM unoplatform/wasm-build:latest AS build
WORKDIR /src

COPY ["src/Director.Client/Director.Client.csproj", "Director.Client/Director.Client.csproj"]
RUN dotnet restore "Director.Client/Director.Client.csproj"

COPY ["src/Lensman/Lensman.Shared/", "Lensman/Lensman.Shared/"]
COPY ["src/Lensman/Lensman.Wasm/Lensman.Wasm.csproj", "Lensman/Lensman.Wasm/Lensman.Wasm.csproj"]
RUN dotnet restore "Lensman/Lensman.Wasm/Lensman.Wasm.csproj"

COPY ["src/Director.Client/", "Director.Client"]
RUN dotnet build "Director.Client/Director.Client.csproj" -c Release

COPY ["src/Lensman/Lensman.Shared/", "Lensman/Lensman.Shared/"]
COPY ["src/Lensman/Lensman.Wasm/", "Lensman/Lensman.Wasm/"]
RUN dotnet build "Lensman/Lensman.Wasm/Lensman.Wasm.csproj" -c Release -o /app/build 

FROM build AS publish
RUN dotnet publish "Lensman/Lensman.Wasm/Lensman.Wasm.csproj" -c Release -o /app/publish

FROM ibebbs/wasm-serve:latest
EXPOSE 80
COPY --from=publish /app/publish /usr/share/nginx/html