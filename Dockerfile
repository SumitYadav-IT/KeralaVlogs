FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["KerlaVlogs.csproj", "./"]
RUN dotnet restore "KerlaVlogs.csproj"

COPY . .
RUN dotnet publish "KerlaVlogs.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000

EXPOSE 10000

ENTRYPOINT ["dotnet", "KerlaVlogs.dll"]