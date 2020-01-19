FROM mcr.microsoft.com/dotnet/core/sdk:3.1.100-alpine3.10 as builder

COPY ./ /src/
WORKDIR /src/

RUN dotnet publish -c release -o /out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.0-alpine3.10

ENV ASPNETCORE_ENVIRONMENT=Production

WORKDIR /app
COPY --from=builder /out/ /app/

EXPOSE 5000

ENTRYPOINT ["dotnet", "reporting-service.dll"]
