FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS build
WORKDIR /src
COPY ["BambooRest/BambooRest.csproj", "BambooRest/"]
RUN dotnet restore "BambooRest/BambooRest.csproj"
COPY . .
WORKDIR "/src/BambooRest"
RUN dotnet build "BambooRest.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BambooRest.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BambooRest.dll"]