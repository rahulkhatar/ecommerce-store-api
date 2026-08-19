# Local development image - runs `dotnet watch` against the source bind-mounted
# by docker-compose.yml (./backend:/app), so this Dockerfile intentionally does
# NOT COPY the source in; it only needs the SDK. A slim multi-stage build for
# production images belongs in docker-compose.prod.yml's own Dockerfile.
FROM mcr.microsoft.com/dotnet/sdk:10.0

WORKDIR /app

EXPOSE 5000

# --non-interactive: without it, a change dotnet watch can't hot-reload
# (e.g. adding a method to an interface) makes it prompt "Restart? y/n/a/v"
# on stdin - which nothing is attached to in a detached container, so it
# just hangs forever instead of restarting. This makes it always restart
# automatically.
ENTRYPOINT ["dotnet", "watch", "run", "--project", "ECommerce.API", "--no-launch-profile", "--non-interactive", "--urls", "http://0.0.0.0:5000"]
