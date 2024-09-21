FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DreckTrack_API/DreckTrack_API.csproj", "DreckTrack_API/"]
RUN dotnet restore "DreckTrack_API/DreckTrack_API.csproj"
COPY . .
WORKDIR "/src/DreckTrack_API"
RUN dotnet build "DreckTrack_API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DreckTrack_API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Copy the migration script into the image
COPY ./migrate-and-start.sh .

# Give execution permissions to the script
RUN chmod +x ./migrate-and-start.sh

# Use the script as the entry point
ENTRYPOINT ["./migrate-and-start.sh"]
