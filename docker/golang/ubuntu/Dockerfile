FROM nerdymishka/ubuntu

ARG IMAGE_VERSION=0.10.0
ARG IMAGE_NAME=nerdymishka/golang-ubuntu
ARG VCS_REF="none"

LABEL maintainer="Nerdy Mishka <mherndon@nerdymishka.com>" \
      readme.md="https://gitlab.com/nerdymishka/gainz/blob/master/docker/golang/ubuntu/README.md" \
      description="This Dockerfile will install the latest version of golang on ubuntu latest." \
      org.label-schema.url="https://gitlab.com/nerdymishka/gainz/blob/master/docker/golang/ubuntu/README.md" \
      org.label-schema.vcs-url="https://gitlab.com/nerdymishka/gainz" \
      org.label-schema.name="golang-ubuntu" \
      org.label-schema.vendor="nerdymishka" \
      org.label-schema.vcs-ref=${VCS_REF} \
      org.label-schema.version=${IMAGE_VERSION} \
      org.label-schema.schema-version="1.0" 

ENV DEBIAN_FRONTEND noninteractive
ENV INITRD No
ENV LANG en_US.UTF-8
ENV GOVERSION 1.10
ENV GOPATH /opt/go

# Install GO.
RUN add-apt-repository ppa:gophers/archive && \
    apt-get update && \
    apt-get install -y \
        golang-1.10-go && \
    rm -rf /var/lib/apt/lists/* 

ENV GOPATH /opt/go
ENV PATH $GOPATH/bin:/usr/lib/go-1.10/bin:$PATH

RUN mkdir -p "$GOPATH/src" "$GOPATH/bin" && chmod -R 777 "$GOPATH"
WORKDIR $GOPATH