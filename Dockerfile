FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Install node
RUN curl -sL https://deb.nodesource.com/setup_16.x | bash
RUN apt-get update && apt-get install -y nodejs && npm install --global yarn

WORKDIR /workspace
COPY . .
RUN dotnet tool restore
RUN dotnet run Bundle


FROM mcr.microsoft.com/dotnet/aspnet:6.0
COPY --from=build /workspace/deploy /app
WORKDIR /app
EXPOSE 8085
ENTRYPOINT [ "dotnet", "Server.dll" ]
