FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["HotelVivaBueno.Api/HotelVivaBueno.Api.csproj", "HotelVivaBueno.Api/"]
COPY ["HotelVivaBueno.Data/HotelVivaBueno.Data.csproj", "HotelVivaBueno.Data/"]

# Restore dependencies
RUN dotnet restore "HotelVivaBueno.Api/HotelVivaBueno.Api.csproj"

# Copy all source code
COPY . .

# Build
WORKDIR "/src/HotelVivaBueno.Api"
RUN dotnet build "HotelVivaBueno.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HotelVivaBueno.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HotelVivaBueno.Api.dll"]
