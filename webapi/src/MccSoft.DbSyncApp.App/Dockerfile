FROM mcr.microsoft.com/dotnet/aspnet:6.0
USER root

# we need gettext-base for envsubst to work
RUN \
  apt-get update \
  && apt-get -y install gettext-base

ENV ASPNETCORE_URLS="http://*:5000"
ENV ASPNETCORE_ENVIRONMENT="Docker"
COPY . /webapi

RUN useradd dotnetuser --create-home --uid 1000 && \
    chown dotnetuser /webapi && \
    chown -R dotnetuser /webapi/wwwroot && \
    chmod +x /webapi/inject-environment-variables-to-spa.sh

WORKDIR /webapi
USER dotnetuser
ENTRYPOINT ./inject-environment-variables-to-spa.sh && dotnet MccSoft.DbSyncApp.App.dll
EXPOSE 5000
