FROM ibebbs/wasm-build:latest AS build
WORKDIR /src
COPY ["src/global.json", "."]
COPY ["src/Director.Client/", "Director.Client/"]
COPY ["src/Lensman/Lensman.Shared/", "Lensman/Lensman.Shared/"]
COPY ["src/Lensman/Lensman.Wasm/", "Lensman/Lensman.Wasm/"]
RUN dotnet build "Lensman/Lensman.Wasm/Lensman.Wasm.csproj" -v diagnostic -c Release -o /app/build 

FROM build AS publish
RUN dotnet publish "Lensman.Wasm.csproj" -v diagnostic -c Release -o /app/publish

FROM ibebbs:wasm-serve
EXPOSE 80
COPY --from=publish /app/publish /usr/share/nginx/html