ARG fromTag=18.04

FROM ubuntu:${fromTag}

ARG IMAGE_VERSION=0.1.1
ARG PS_VERSION=6.1.0~rc.1
ARG PS_VERSION_POSTFIX=-1.ubuntu.18.04
ARG IMAGE_NAME=nerdymishka/ubuntu-18.04
ARG VCS_REF="none"

LABEL maintainer="Nerdy Mishka <mherndon@nerdymishka.com>" \
      readme.md="https://gitlab.com/nerdymishka/gainz/blob/master/docker/ubuntu/18.04/README.md" \
      description="This Dockerfile will install the latest version of powershell on ubuntu/18.04." \
      org.label-schema.url="https://gitlab.com/nerdymishka/gainz/blob/master/docker/ubuntu/18.04/README.md" \
      org.label-schema.vcs-url="https://gitlab.com/nerdymishka/gainz" \
      org.label-schema.name="ubuntu-18.04" \
      org.label-schema.vendor="nerdymishka" \
      org.label-schema.vcs-ref=${VCS_REF} \
      org.label-schema.version=${IMAGE_VERSION} \
      org.label-schema.schema-version="1.0" \
      org.label-schema.docker.cmd="docker run ${IMAGE_NAME} pwsh -c '$psversiontable'" \
      org.label-schema.docker.cmd.devel="docker run ${IMAGE_NAME}" \
      org.label-schema.docker.cmd.test="docker run ${IMAGE_NAME} pwsh -c Invoke-Pester" \
      org.label-schema.docker.cmd.help="docker run ${IMAGE_NAME} pwsh -c Get-Help"

# setup terminal
ENV TERM xterm



# Install dependencies and clean up
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        git \
        software-properties-common \
        build-essential \ 
        openssh-client \
        apt-utils \
        ca-certificates \
        curl \
        wget \ 
        cifs-utils \
        apt-transport-https \
        locales \
        gnupg2 \
        gpg \
        unzip \
        libsecret-tools \
    && apt-get dist-upgrade -y \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

RUN  gpg --keyserver pgp.mit.edu --recv-keys \
        7638D0442B90D010 8B48AD6246925553 \ 
        && gpg --armor --export 7638D0442B90D010 | apt-key add - \
        && gpg --armor --export 8B48AD6246925553 | apt-key add - \
        &&  echo 'deb http://deb.debian.org/debian jessie-backports main' > /etc/apt/sources.list.d/backports.list

# Setup the locale
ENV LANG en_US.UTF-8
RUN locale-gen $LANG && update-locale

# Download the Microsoft repository GPG keys
RUN curl -L -O https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb

# Register the Microsoft repository GPG keys
RUN dpkg -i packages-microsoft-prod.deb

# Install powershell from Microsoft Repo
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        powershell-preview=${PS_VERSION}${PS_VERSION_POSTFIX} \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/* \
    && ln -sf /opt/microsoft/powershell/6-preview/pwsh /usr/bin/pwsh

CMD ["pwsh-preview"]