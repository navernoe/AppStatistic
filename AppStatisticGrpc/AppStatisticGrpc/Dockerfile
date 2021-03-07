#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["AppStatisticGrpc/AppStatisticGrpc.csproj", "AppStatisticGrpc/"]
RUN dotnet restore "AppStatisticGrpc/AppStatisticGrpc.csproj"
COPY . .
WORKDIR "/src/AppStatisticGrpc"
RUN dotnet build "AppStatisticGrpc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AppStatisticGrpc.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AppStatisticGrpc.dll"]