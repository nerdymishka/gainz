FROM nerdymishka/ubuntu 


ENV SFTP_STANDARD_GROUP="standard_sftp_users" \
    SFTP_ADMIN="sftp_admin" \
    SFTP_ADMIN_KEY_URI="" \
    SFTP_AZURE_STORAGE_ACCOUNT=""\
    SFTP_AZURE_STORAGE_KEY=""\
    SFTP_MOUNT_KEY=""\
    SFTP_SUDO_PHRASE=""

RUN apt-get update && \
    apt-get install -y --no-install-recommends \ 
        cifs-utils \
        sudo \
        openssl \
        # s3fs \
        openssh-server && \
    apt-get dist-upgrade -y && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/* && \
    mkdir -p /var/run/sshd && \
    rm -f /etc/ssh/ssh_host_*key* && \
    mkdir -p /opt/powershell/posh-sftp && \
    touch /var/log/azure-mount.log   

VOLUME ["/etc/ssh", "/var/log"]


COPY etc/ /etc/
COPY posh-sftp/ /opt/powershell/posh-sftp 
RUN chmod +x /etc/rc.local && \
    chmod 0760 /etc/ssh/sshd_config && \
    chmod 0760 /etc/ssh/ssh_config 

ENTRYPOINT [ "pwsh", "/opt/powershell/posh-sftp/docker-entrypoint.ps1" ]

EXPOSE 22
CMD ["/usr/sbin/sshd", "-D", "-e"]