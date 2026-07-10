# Build stage
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8 AS build

WORKDIR /src

COPY . .

RUN msbuild GoaSocietyRegistration.sln /p:Configuration=Release

# Runtime stage
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8

COPY --from=build /src/GoaSocietyRegistration/ /inetpub/wwwroot

EXPOSE 80