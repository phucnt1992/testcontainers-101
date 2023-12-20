FROM mcr.microsoft.com/devcontainers/dotnet:8.0-bookworm

# Install Azure Functions Core Tools v4
RUN wget -q https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb \
    && sudo dpkg -i packages-microsoft-prod.deb \
    && rm packages-microsoft-prod.deb \
    && sudo apt update; sudo apt full-upgrade -y \
    && sudo apt install azure-functions-core-tools-4 -y \
    && sudo apt autoremove -y && sudo apt clean;

