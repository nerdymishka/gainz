FROM nerdymishka/ubuntu-18.04

ARG IMAGE_NAME=php7.2-fpm-nginx-ubuntu-18.04
ARG IMAGE_VERSION=0.1.0
ARG VCS_REF="none"


LABEL maintainer="Nerdy Mishka <mherndon@nerdymishka.com>" \
      readme.md="https://gitlab.com/nerdymishka/gainz/blob/master/docker/php/ubuntu-18.04/README.md" \
      description="This Dockerfile will install the php7.2-fpm on ubuntu/18.04." \
      org.label-schema.url="https://gitlab.com/nerdymishka/gainz/blob/master/docker/ubuntu/18.04/README.md" \
      org.label-schema.vcs-url="https://gitlab.com/nerdymishka/gainz" \
      org.label-schema.name="php7.2-fpm-nginx-ubuntu-18.04" \
      org.label-schema.vendor="nerdymishka" \
      org.label-schema.vcs-ref=${VCS_REF} \
      org.label-schema.version=${IMAGE_VERSION} \
      org.label-schema.schema-version="1.0" 


RUN add-apt-repository ppa:ondrej/php -y 

RUN export DEBIAN_FRONTEND=noninteractive && \
     apt-get update \
      && apt-get install -y --no-install-recommends \
            php7.2 \
            php7.2-fpm \
            php7.2-common \
            php7.2-curl \
            php7.2-gd \
            php7.2-enchant \
            php7.2-json \
            php7.2-mbstring \ 
            php7.2-mysql \
            php7.2-pgsql \
            php7.2-soap \
            php7.2-sqlite3 \
            php7.2-tidy \
            php7.2-xml \
            php7.2-xmlrpc \
            php7.2-zip \
      && apt-get dist-upgrade -y \
      && apt-get clean \
      && rm -rf /var/lib/apt/lists/*