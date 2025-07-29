# Dockerfile محسن لنظام المراقبة
# يعمل مع Visual Studio و Docker Compose

# مرحلة Base للتشغيل
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# تثبيت curl لفحص الصحة (Health Check)
USER root
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# إنشاء مستخدم التطبيق
ARG APP_UID=1001
RUN adduser --disabled-password --gecos '' --uid $APP_UID appuser
USER $APP_UID

# مرحلة البناء
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# نسخ ملف المشروع واستعادة الحزم
COPY ["dotnet-app.csproj", "./"]
RUN dotnet restore "./dotnet-app.csproj"

# نسخ باقي الملفات وبناء المشروع
COPY . .
RUN dotnet build "./dotnet-app.csproj" -c $BUILD_CONFIGURATION -o /app/build

# مرحلة النشر
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./dotnet-app.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# المرحلة النهائية
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# متغيرات البيئة لنظام المراقبة
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=8080

# نقطة دخول التطبيق
ENTRYPOINT ["dotnet", "dotnet-app.dll"]

# معلومات الصورة
LABEL maintainer="monitoring-team"
LABEL description="تطبيق .NET 8 مع مراقبة Prometheus"
LABEL version="1.0.0"