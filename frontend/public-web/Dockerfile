# Build stage
FROM node:20-alpine AS builder
WORKDIR /app

# Enable pnpm
RUN corepack enable && corepack prepare pnpm@latest --activate

# Copy package files
COPY package.json pnpm-lock.yaml pnpm-workspace.yaml ./
COPY frontend/public-web/package.json frontend/public-web/
COPY frontend/public-web/pnpm-lock.yaml frontend/public-web/

# Install dependencies
RUN pnpm install --frozen-lockfile

# Copy source
COPY . .

# Build the public-web app
WORKDIR /app/frontend/public-web
RUN pnpm run build

# Runtime stage
FROM node:20-alpine AS runner
WORKDIR /app

ENV NODE_ENV=production

# Copy built app
COPY --from=builder /app/frontend/public-web/out ./public-web/out

# Serve with a simple static server
EXPOSE 3000

CMD ["npx", "serve", "-s", "public-web/out", "-l", "3000"]