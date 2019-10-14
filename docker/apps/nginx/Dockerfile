# Based on the nginx buster image
# https://github.com/nginxinc/docker-nginx/blob/master/stable/buster/Dockerfile  
# modified to use ubuntu/powershell

ARG FROM_TAG=bionic
FROM nerdymishka/ubuntu:${FROM_TAG}

ARG IMAGE_VERSION=1.16.1
ARG IMAGE_VENDOR=nerdymishka
ARG IMAGE_NAME=nginx
ARG IMAGE_FULL_NAME="${IMAGE_VENDOR}/${IMAGE_NAME}"
ARG VCS_REF="none"

LABEL maintainer="Nerdy Mishka <mherndon@nerdymishka.com>" \
      readme.md="https://gitlab.com/nerdymishka/gainz/blob/master/docker/apps/nginx/" \
      description="Nginx on ubuntu as reverse-proxy server" \
      org.label-schema.url="https://gitlab.com/nerdymishka/gainz/blob/master/docker/nginx/README.md" \
      org.label-schema.vcs-url="https://gitlab.com/nerdymishka/gainz" \
      org.label-schema.name=${IMAGE_NAME} \
      org.label-schema.vendor=${IMAGE_VENDOR} \
      org.label-schema.vcs-ref=${VCS_REF} \
      org.label-schema.version=${IMAGE_VERSION} \
      org.label-schema.schema-version="1.0" 

ENV APT_KEY_DONT_WARN_ON_DANGEROUS_USAGE=DontWarn \
	NGINX_VERSION=${IMAGE_VERSION} \
    NJS_VERSION=0.3.5 \
	PKG_RELEASE=1~bionic \
	DEBIAN_FRONTEND=noninteractive \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false 

RUN apt-get update -y \
    && apt-get install -y --no-install-recommends --no-install-suggests  apt-transport-https gnupg2 \
    && apt-get remove --purge --auto-remove -y gnupg2 && rm -rf /var/lib/apt/lists/* \
    && dpkgArch="$(dpkg --print-architecture)" \
    && nginxPackages=" \
        nginx=${NGINX_VERSION}-${PKG_RELEASE} \
        nginx-module-xslt=${NGINX_VERSION}-${PKG_RELEASE} \
        nginx-module-geoip=${NGINX_VERSION}-${PKG_RELEASE} \
        nginx-module-image-filter=${NGINX_VERSION}-${PKG_RELEASE} \
        nginx-module-njs=${NGINX_VERSION}.${NJS_VERSION}-${PKG_RELEASE} \
    " \
    && case "$dpkgArch" in \
        amd64|i386) \
# arches officialy built by upstream
            echo "deb http://nginx.org/packages/ubuntu `lsb_release -cs` nginx" | tee /etc/apt/sources.list.d/nginx.list \
		    && curl -fsSL https://nginx.org/keys/nginx_signing.key | apt-key add - \
            && apt-get update \
            ;; \
        *) \
# we're on an architecture upstream doesn't officially build for
# let's build binaries from the published source packages
            echo "deb http://nginx.org/packages/ubuntu `lsb_release -cs` nginx" | tee /etc/apt/sources.list.d/nginx.list \
		    && curl -fsSL https://nginx.org/keys/nginx_signing.key |  apt-key add - \
            \
# new directory for storing sources and .deb files
            && tempDir="$(mktemp -d)" \
            && chmod 777 "$tempDir" \
# (777 to ensure APT's "_apt" user can access it too)
            \
# save list of currently-installed packages so build dependencies can be cleanly removed later
            && savedAptMark="$(apt-mark showmanual)" \
            \
# build .deb files from upstream's source packages (which are verified by apt-get)
            && apt-get update \
            && apt-get build-dep -y $nginxPackages \
            && ( \
                cd "$tempDir" \
                && DEB_BUILD_OPTIONS="nocheck parallel=$(nproc)" \
                    apt-get source --compile $nginxPackages \
            ) \
# we don't remove APT lists here because they get re-downloaded and removed later
            \
# reset apt-mark's "manual" list so that "purge --auto-remove" will remove all build dependencies
# (which is done after we install the built packages so we don't have to redownload any overlapping dependencies)
            && apt-mark showmanual | xargs apt-mark auto > /dev/null \
            && { [ -z "$savedAptMark" ] || apt-mark manual $savedAptMark; } \
            \
# create a temporary local APT repo to install from (so that dependency resolution can be handled by APT, as it should be)
            && ls -lAFh "$tempDir" \
            && ( cd "$tempDir" && dpkg-scanpackages . > Packages ) \
            && grep '^Package: ' "$tempDir/Packages" \
            && echo "deb [ trusted=yes ] file://$tempDir ./" > /etc/apt/sources.list.d/temp.list \
# work around the following APT issue by using "Acquire::GzipIndexes=false" (overriding "/etc/apt/apt.conf.d/docker-gzip-indexes")
#   Could not open file /var/lib/apt/lists/partial/_tmp_tmp.ODWljpQfkE_._Packages - open (13: Permission denied)
#   ...
#   E: Failed to fetch store:/var/lib/apt/lists/partial/_tmp_tmp.ODWljpQfkE_._Packages  Could not open file /var/lib/apt/lists/partial/_tmp_tmp.ODWljpQfkE_._Packages - open (13: Permission denied)
            && apt-get -o Acquire::GzipIndexes=false update \
            ;; \
    esac \
    \
    && apt-get install --no-install-recommends --no-install-suggests -y \
                        $nginxPackages \
                        gettext-base \
    ## skipping the removal of ca-certificates && apt-get remove --purge --auto-remove -y ca-certificates 
	&& rm -rf /var/lib/apt/lists/* /etc/apt/sources.list.d/nginx.list \
    \
# if we have leftovers from building, let's purge them (including extra, unnecessary build deps)
    && if [ -n "$tempDir" ]; then \
        apt-get purge -y --auto-remove \
        && rm -rf "$tempDir" /etc/apt/sources.list.d/temp.list; \
    fi 

# forward request and error logs to docker log collector
RUN ln -sf /dev/stdout /var/log/nginx/access.log \
    && ln -sf /dev/stderr /var/log/nginx/error.log

VOLUME ["/etc/nginx/sites-enabled", "/etc/nginx/certs", "/etc/nginx/conf.d", "/etc/nginx/snippets", "/var/log/nginx", "/var/www/html"]

EXPOSE 80
EXPOSE 443

STOPSIGNAL SIGTERM

CMD ["nginx", "-g", "daemon off;"]