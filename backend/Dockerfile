# Render builds .NET apps from a Dockerfile (it has no native .NET runtime,
# unlike Node/Python/Ruby/Go) - this is only used by Render's own build
# servers. Local dev never runs this; run the API directly with `dotnet run`.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ECommerce.slnx ./
COPY ECommerce.Domain/*.csproj ECommerce.Domain/
COPY ECommerce.Application/*.csproj ECommerce.Application/
COPY ECommerce.Infrastructure/*.csproj ECommerce.Infrastructure/
COPY ECommerce.Persistence/*.csproj ECommerce.Persistence/
COPY ECommerce.API/*.csproj ECommerce.API/
RUN dotnet restore ECommerce.API/ECommerce.API.csproj

COPY . .
RUN dotnet publish ECommerce.API/ECommerce.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "ECommerce.API.dll"]
