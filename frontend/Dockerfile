# Local development image - runs the Vite dev server against the source
# bind-mounted by docker-compose.yml (./frontend:/app), with node_modules
# kept in its own anonymous volume so the container's Linux install doesn't
# get clobbered by whatever's in the host's (possibly different-OS) node_modules.
FROM node:24-alpine

WORKDIR /app

EXPOSE 5173

CMD ["sh", "-c", "npm install && npm run dev -- --host 0.0.0.0"]
