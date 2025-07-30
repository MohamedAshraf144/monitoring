# Dockerfile محسن - HTTP فقط
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# نسخ ملف المشروع واستعادة الحزم
COPY ["dotnet-app.csproj", "./"]
RUN dotnet restore "./dotnet-app.csproj"

# نسخ باقي الملفات وبناء المشروع
COPY . .
RUN dotnet build "./dotnet-app.csproj" -c Release -o /app/build

# مرحلة النشر
FROM build AS publish
RUN dotnet publish "./dotnet-app.csproj" -c Release -o /app/publish /p:UseAppHost=false

# المرحلة النهائية
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# تثبيت curl لفحص الصحة
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# نسخ التطبيق المنشور
COPY --from=publish /app/publish .

# تعريف المنافذ
EXPOSE 8080

# متغيرات البيئة - HTTP فقط
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_HTTPS_PORTS=""

# نقطة دخول التطبيق
ENTRYPOINT ["dotnet", "dotnet-app.dll"]