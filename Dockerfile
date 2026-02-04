# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy toàn bộ solution API (giữ nguyên Clean Architecture)
COPY API ./API

# Restore & publish WebAPI project
WORKDIR /src/API/API
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy output từ build stage
COPY --from=build /app/publish .

# Render expose port qua biến PORT
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# 🔥 ĐỔI TÊN DLL nếu project bạn không phải "API"
ENTRYPOINT ["dotnet", "API.dll"]
