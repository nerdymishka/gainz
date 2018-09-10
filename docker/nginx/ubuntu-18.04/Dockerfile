FROM nerdymishka/ubuntu-18.04

ARG IMAGE_VERSION=0.1.0
ARG IMAGE_NAME=nerdymishka/nginx-ubuntu-18.04
ARG VCS_REF="none"

LABEL maintainer="Nerdy Mishka <mherndon@nerdymishka.com>" \
      readme.md="https://gitlab.com/nerdymishka/gainz/blob/master/docker/nginx/ubuntu-18.04/README.md" \
      description="This Dockerfile will install the latest version of nginx on ubuntu/18.04." \
      org.label-schema.url="https://gitlab.com/nerdymishka/gainz/blob/master/docker/nginx/ubuntu/18.04/README.md" \
      org.label-schema.vcs-url="https://gitlab.com/nerdymishka/gainz" \
      org.label-schema.name="ubuntu-18.04" \
      org.label-schema.vendor="nerdymishka" \
      org.label-schema.vcs-ref=${VCS_REF} \
      org.label-schema.version=${IMAGE_VERSION} \
      org.label-schema.schema-version="1.0" 

# Install Nginx.
RUN apt-get update && \
    apt-get install -y nginx && \
    rm -rf /var/lib/apt/lists/* && \
    mkdir /etc/nginx/certs && \
    chown -R www-data:www-data /etc/nginx/certs && \
    echo "\ndaemon off;" >> /etc/nginx/nginx.conf && \
    chown -R www-data:www-data /var/lib/nginx

ADD New-DefaultSiteConfig.ps1 /
ADD Resolve-Stache.ps1 /
ADD Set-DefaultSiteConfig.ps1 /
ADD New-DhParam.ps1 /
COPY etc/nginx/samples /etc/nginx/samples 

# Define mountable directories.
VOLUME ["/etc/nginx/sites-enabled", "/etc/nginx/certs", "/etc/nginx/conf.d", "/var/log/nginx", "/var/www/html"]

# Define working directory.
WORKDIR /etc/nginx


# Define default command.
CMD ["nginx"]

# Expose ports.
EXPOSE 80
EXPOSE 443