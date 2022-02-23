FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY Sales/Sales.csproj Sales/Sales.csproj
COPY Messages/Messages.csproj Messages/Messages.csproj

RUN dotnet restore "Sales/Sales.csproj"
COPY . .
RUN dotnet build "Sales/Sales.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Sales.dll"]

