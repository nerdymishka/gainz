FROM nerdymishka/golang-ubuntu


ARG IMAGE_VERSION=0.10.0
ARG IMAGE_NAME=nerdymishka/cfssl-ubuntu
ARG VCS_REF="none"

LABEL maintainer="Nerdy Mishka <mherndon@nerdymishka.com>" \
      readme.md="https://gitlab.com/nerdymishka/gainz/blob/master/docker/cfssl/ubuntu/README.md" \
      description="This Dockerfile will cfssl as server." \
      org.label-schema.url="https://gitlab.com/nerdymishka/gainz/blob/master/docker/cfssl/ubuntu/README.md" \
      org.label-schema.vcs-url="https://gitlab.com/nerdymishka/gainz" \
      org.label-schema.name="cfssl-ubuntu" \
      org.label-schema.vendor="nerdymishka" \
      org.label-schema.vcs-ref=${VCS_REF} \
      org.label-schema.version=${IMAGE_VERSION} \
      org.label-schema.schema-version="1.0" 


ENV DEBIAN_FRONTEND noninteractive \ 
    INITRD No \ 
    LANG en_US.UTF-8 

ENV CFSSL_URI="http://localhost"
ENV CFSSL_PORT = 8080
ENV CFSSL_CRL_PORT = 8888
ENV CFSSL_OSCP_PORT = 8889 
ENV CFSSL_DEFAULT_COUNTRY = "US"
ENV CFSSL_DEFAULT_STATE = "DC"
ENV CFSSL_DEFAULT_LOCALITY = "DC"
ENV CFSSL_DEFAULT_ORG_UNIT = "DevOps"
ENV CFSSL_DEAFULT_ORG = "ACME"
ENV CFSSL_DEFAULT_CN = "localhost"
ENV CFSSL_DB_INIT=0
ENV CFSSL_OSCP=0 





RUN go get github.com/cloudflare/cfssl_trust/... && \
    # Install trusted certs
    # taking cue from cfssl docker, curious if only the ca_certificates.crt is 
    # needed.
    mkdir -p /etc/cfssl && \
	cp -R "${GOPATH}/src/github.com/cloudflare/cfssl_trust" /etc/cfssl 
 
# Install goose to run database migrations 
RUN go get bitbucket.org/liamstask/goose/cmd/goose && \
    go get github.com/GeertJohan/go.rice/rice && \
    # Install cfssl: clone and build only the required binaries
    git clone --depth=1 "https://github.com/cloudflare/cfssl.git" "${GOPATH}/src/github.com/cloudflare/cfssl" && \
    cd "${GOPATH}/src/github.com/cloudflare/cfssl" && \
	go build -o $GOPATH/bin/cfssl ./cmd/cfssl && \
	go build -o $GOPATH/bin/cfssljson ./cmd/cfssljson && \
	go build -o $GOPATH/bin/mkbundle ./cmd/mkbundle && \
	go build -o $GOPATH/bin/multirootca ./cmd/multirootca && \
    rice embed-go -i=./cli/serve 

COPY posh-cfssl/  /opt/powershell/posh-cfssl/
COPY etc/cfssl /etc/cfssl/ 

WORKDIR "/etc/cfssl/"


ENTRYPOINT ["pwsh","/opt/powershell/posh-cfssl/docker-entrypoint.ps1"]

CMD ["ls", "/etc/cfssl/"]