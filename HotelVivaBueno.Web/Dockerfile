FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["HotelVivaBueno.Web/HotelVivaBueno.Web.csproj", "HotelVivaBueno.Web/"]
COPY ["HotelVivaBueno.Data/HotelVivaBueno.Data.csproj", "HotelVivaBueno.Data/"]

# Restore dependencies
RUN dotnet restore "HotelVivaBueno.Web/HotelVivaBueno.Web.csproj"

# Copy all source code
COPY . .

# Build
WORKDIR "/src/HotelVivaBueno.Web"
RUN dotnet build "HotelVivaBueno.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HotelVivaBueno.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HotelVivaBueno.Web.dll"]
