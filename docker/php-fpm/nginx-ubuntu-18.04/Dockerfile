FROM nerdymishka/ubuntu-18.04

ARG IMAGE_NAME=php-fpm-nginx-ubuntu-18.04
ARG IMAGE_VERSION=0.1.0
ARG VCS_REF="none"


LABEL maintainer="Nerdy Mishka <mherndon@nerdymishka.com>" \
      readme.md="https://gitlab.com/nerdymishka/gainz/blob/master/docker/php/ubuntu-18.04/README.md" \
      description="This Dockerfile will install the php7.2-fpm on ubuntu/18.04." \
      org.label-schema.url="https://gitlab.com/nerdymishka/gainz/blob/master/docker/ubuntu/18.04/README.md" \
      org.label-schema.vcs-url="https://gitlab.com/nerdymishka/gainz" \
      org.label-schema.name="php-fpm-nginx-ubuntu-18.04" \
      org.label-schema.vendor="nerdymishka" \
      org.label-schema.vcs-ref=${VCS_REF} \
      org.label-schema.version=${IMAGE_VERSION} \
      org.label-schema.schema-version="1.0" 


RUN add-apt-repository ppa:ondrej/php -y && \ 
      add-apt-repository universe

RUN export DEBIAN_FRONTEND=noninteractive && \
      apt-get update && \
      apt-get install -y --no-install-recommends \
            sendmail \
            php \
            php-fpm \
            php-curl \
            php-gd \
            php-enchant \
            php-mbstring \
            php-mysql \
            php7.2-opcache \
            php-pgsql \
            php-pspell \
            php-soap \
            php-sqlite3 \
            php-tidy \
            php-xml \
            php-xmlrpc \
            php-zip \
            php-apcu \
            php-amqp \
            php-imagick \
            php-pear \ 
            php-dev \
            php-sodium \
            php-mail \
            php-markdown \
            php-memcache \
            php-redis \
            php-xdebug \
            php-oauth \
            php-http \
            php-ssh2 \
            php-yaml \
            php-uuid \
            php-mongodb \
            php-zmq && \
      yes 'Y' | sendmailconfig && \
      curl -sS https://getcomposer.org/installer | php -- --install-dir=/usr/local/bin --filename=composer && \
      apt-get dist-upgrade -y && \
      apt-get clean && \
      rm -rf /var/lib/apt/lists/*