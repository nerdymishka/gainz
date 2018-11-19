ARG FROM_TAG=18.04

FROM ubuntu:${FROM_TAG}

ARG IMAGE_NAME=nerdymishka/ubuntu
ARG VCS_REF="none"
ARG DEBIAN_FRONTEND=noninteractive
ARG APT_KEY_DONT_WARN_ON_DANGEROUS_USAGE=DontWarn

LABEL maintainer="Nerdy Mishka <mherndon@nerdymishka.com>" \
      readme.md="https://gitlab.com/nerdymishka/gainz/blob/master/docker/os/ubuntu/README.md" \
      description="Ubuntu image with microsoft, universe, and backport feeds. Basic packages like curl, gpg, wget are installed" \
      org.label-schema.url="https://gitlab.com/nerdymishka/gainz/blob/master/docker/ubuntu/README.md" \
      org.label-schema.vcs-url="https://gitlab.com/nerdymishka/gainz/tree/master/docker/os/ubuntu" \
      org.label-schema.name="ubuntu" \
      org.label-schema.vendor="nerdymishka" \
      org.label-schema.vcs-ref=${VCS_REF} \
      org.label-schema.version=18.04 \
      org.label-schema.schema-version="1.0" \
      org.label-schema.docker.cmd="docker run ${IMAGE_NAME} bash -c 'PATH'" 

# setup terminal
ENV TERM xterm \
    LANG en_US.UTF-8 

# Install dependencies and clean up

# tech
RUN apt-get update --no-install-recommends --no-install-suggests \
    && apt-get install -y apt-utils 2> /dev/null \
    && apt-get install -y --no-install-recommends --no-install-suggests \
        software-properties-common \
        openssh-server \
        ca-certificates \
        curl \
        wget \ 
        apt-transport-https \
        locales \
        gnupg2 \
        gpg \
        unzip \
        tar \
    && locale-gen $LANG \ 
    && update-locale \
    && add-apt-repository universe \
    && apt-get dist-upgrade -y --no-install-recommends --no-install-suggests \
    && gpg --keyserver pgp.mit.edu --recv-keys 7638D0442B90D010 2>/dev/null \
    && gpg --keyserver pgp.mit.edu --recv-keys 8B48AD6246925553 2>/dev/null \ 
        && gpg --armor --export 7638D0442B90D010 2>/dev/null | apt-key add -  \
        && gpg --armor --export 8B48AD6246925553 2>/dev/null | apt-key add - \
        && echo 'deb http://deb.debian.org/debian jessie-backports main' > /etc/apt/sources.list.d/backports.list \
    && curl -L -O https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb \
    && rm -f packages-microsoft-prod.deb \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/* 


CMD ["bash"]