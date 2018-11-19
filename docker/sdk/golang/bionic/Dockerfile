ARG FROM_TAG=18.04
ARG GO_TAG=1.10
ARG GO_ROOT=/opt/go

FROM nerdymishka/ubuntu:${FROM_TAG}

ARG IMAGE_VERSION=${GO_VERSION}
ARG IMAGE_NAME=nerdymishka/golang
ARG VCS_REF="none"
ARG DEBIAN_FRONTEND=noninteractive

LABEL maintainer="Nerdy Mishka <mherndon@nerdymishka.com>" \
      readme.md="https://gitlab.com/nerdymishka/gainz/blob/master/docker/golang/ubuntu/README.md" \
      description="Go lang" \
      org.label-schema.url="https://gitlab.com/nerdymishka/gainz/blob/master/docker/golang/ubuntu/README.md" \
      org.label-schema.vcs-url="https://gitlab.com/nerdymishka/gainz" \
      org.label-schema.name="golang" \
      org.label-schema.vendor="nerdymishka" \
      org.label-schema.vcs-ref=${VCS_REF} \
      org.label-schema.version=${IMAGE_VERSION} \
      org.label-schema.schema-version="1.0" 

ENV INITRD No
ENV GO_PATH ${GO_ROOT}
ENV GO_VERSION ${GO_TAG}

# Install GO.
RUN add-apt-repository ppa:gophers/archive && \
    apt-get update && \
    apt-get install -y && \
    rm -rf /var/lib/apt/lists/* 

ENV PATH ${GO_PATH}/bin:/usr/lib/go-${GO_VERSION}/bin:$PATH

RUN mkdir -p "$GO_PATH/src" "$GO_PATH/bin" && chmod -R 777 "$GO_PATH"
WORKDIR $GO_PATH