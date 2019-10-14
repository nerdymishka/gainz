# https://github.com/PowerShell/PowerShell-Docker/blob/master/release/stable/ubuntu18.04/docker/Dockerfile

ARG IMAGE_BASE_TAG=18.04
ARG IMAGE_BASE=ubuntu

ARG IMAGE_REPO=nerdymishka
ARG IMAGE_NAME=ubuntu
ARG IMAGE_TAG=bionic
ARG IMAGE_FULL_NAME="${IMAGE_REPO}/${IMAGE_NAME}"


#ARG APT_KEY_DONT_WARN_ON_DANGEROUS_USAGE=DontWarn

FROM ${IMAGE_BASE}:${IMAGE_BASE_TAG} AS installer-env 

ARG PS_VERSION=6.2.3
ARG PS_PACKAGE=powershell_${PS_VERSION}-1.ubuntu.18.04_amd64.deb
ARG PS_PACKAGE_URL=https://github.com/PowerShell/PowerShell/releases/download/v${PS_VERSION}/${PS_PACKAGE}

RUN echo ${PS_PACKAGE_URL}
# Download the Linux package and save it
ADD ${PS_PACKAGE_URL} /tmp/powershell.deb


# setup terminal
ENV DEBIAN_FRONTEND=noninteractive \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL="en_US.UTF-8" \
    LANG="en_US.UTF-8" \
    # set a fixed location for the Module analysis cache
    PSModuleAnalysisCachePath=/var/cache/microsoft/powershell/PSModuleAnalysisCache/ModuleAnalysisCache

# tech
RUN apt-get update  --no-install-suggests \
    # use the default to avoid a warning. 
    && export LC_ALL='' \
    && apt-get install -y apt-utils locales 2> /dev/null \
    # generate language and LC_ALL back to language, then update locale
    && locale-gen $LANG && export LC_ALL=${LANG} && update-locale \
    && apt-get install -y /tmp/powershell.deb \
    && apt-get install -y --no-install-recommends --no-install-suggests \
        software-properties-common \
        gpg \
        unzip \
        zip \
        tar \
        curl \
        wget \ 
        apt-transport-https \
        # Required for help in powershell
        less \ 
        #required for openssl
        ca-certificates \
        gss-ntlmssp \
    && apt-get dist-upgrade -y \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/* \
    # remove powershell package
    && rm /tmp/powershell.deb \
    # intialize powershell module cache
    && pwsh \
        -NoLogo \
        -NoProfile \
        -Command " \
          \$ErrorActionPreference = 'Stop' ; \
          \$ProgressPreference = 'SilentlyContinue' ; \
          while(!(Test-Path -Path \$env:PSModuleAnalysisCachePath)) {  \
            Write-Host "'Waiting for $env:PSModuleAnalysisCachePath'" ; \
            Start-Sleep -Seconds 6 ; \
          }"

ARG VCS_REF="none"
LABEL maintainer="Nerdy Mishka <mherndon@nerdymishka.com>" \
      readme.md="https://gitlab.com/nerdymishka/gainz/blob/master/docker/os/ubuntu/bionic/README.md" \
      description="Ubuntu image with powershell and packages like curl, gpg, wget are installed." \
      org.label-schema.url="https://gitlab.com/nerdymishka/gainz/blob/master/docker/ubuntu/bionic/README.md" \
      org.label-schema.vcs-url="https://gitlab.com/nerdymishka/gainz/tree/master/docker/os/ubuntu/bionic" \
      org.label-schema.name=${IMAGE_NAME} \
      org.label-schema.vendor="nerdymishka" \
      org.label-schema.vcs-ref=${VCS_REF} \
      org.label-schema.version=${IMAGE_TAG} \
      org.label-schema.schema-version="1.0" \
      org.label-schema.docker.cmd="docker run ${IMAGE_FULL_NAME} bash -c 'PATH'" 

CMD ["pwsh"]