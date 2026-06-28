FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["OrderManagementAPI.API/OrderManagementAPI.API.csproj", "OrderManagementAPI.API/"]
COPY ["OrderManagementAPI.Application/OrderManagementAPI.Application.csproj", "OrderManagementAPI.Application/"]
COPY ["OrderManagementAPI.Domain/OrderManagementAPI.Domain.csproj", "OrderManagementAPI.Domain/"]
COPY ["OrderManagementAPI.Infrastructure/OrderManagementAPI.Infrastructure.csproj", "OrderManagementAPI.Infrastructure/"]

RUN dotnet restore "OrderManagementAPI.API/OrderManagementAPI.API.csproj"

COPY . .
WORKDIR "/src/OrderManagementAPI.API"

RUN dotnet build "OrderManagementAPI.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OrderManagementAPI.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "OrderManagementAPI.API.dll"]