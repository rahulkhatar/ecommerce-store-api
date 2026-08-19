# Local development image - runs `dotnet watch` against the source bind-mounted
# by docker-compose.yml (./backend:/app), so this Dockerfile intentionally does
# NOT COPY the source in; it only needs the SDK. A slim multi-stage build for
# production images belongs in docker-compose.prod.yml's own Dockerfile.
FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app

EXPOSE 5000

ENTRYPOINT ["dotnet", "watch", "run", "--project", "ECommerce.API", "--no-launch-profile", "--urls", "http://0.0.0.0:5000"]
