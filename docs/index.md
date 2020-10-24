# Welcome to UOStudio

![UO Studio](https://raw.githubusercontent.com/deccer/UOStudio/main/assets/client/splashscreen.png)

!!! note
    UO Studio is not yet usable.

## Setup Development Environment

=== "Windows"

    - Install [git](https://git-scm.com/download/win)
    - Install [dotnet-sdk]()

    - Chose between the following IDEs then install

    === "Visual Studio 2019"

        [Download & Install](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&rel=16)


    === "Visual Studio Code"

        [Download & Install](https://code.visualstudio.com/docs/?dv=win)

    === "Rider"

        [Download & Install](https://www.jetbrains.com/rider/)

        !!! note

        That one costs money

    Clone repo
    ```bash
    git clone --recursive https://github.com/deccer/UOStudio
    git submodule update --init --recursive --force
    ```

=== "Ubuntu"

    Install tooling
    ```bash
    sudo apt install git dirmngr gnupg apt-transport-https ca-certificates software-properties-common lsb-release
    ```

    Register mono repositories
    ```bash
    sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
    sudo apt-add-repository "deb https://download.mono-project.com/repo/ubuntu stable-`lsb_release -sc` main"
    ```

    Install mono
    ```bash
    sudo apt install mono-complete
    ```

    Install .netcore
    ```bash
    wget https://packages.microsoft.com/config/ubuntu/`grep -oP '(?<=^VERSION_ID=).+' /etc/os-release | tr -d '"'`/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    sudo apt update
    sudo apt install -y dotnet-sdk-3.1
    ```

    !!! note

    Ubuntu 20.04 and 18.04 are the only officially supported ubuntu distributions.

    Clone repo
    ```bash
    git clone --recursive https://github.com/deccer/UOStudio
    git submodule update --init --recursive --force
    ```

    Build UO Studio
    ```bash
    cd UOStudio
    dotnet build
    ```


=== "Fedora"

    === "Fedora 32"

    Install dotnet sdk
    ```bash
    sudo dnf install dotnet-sdk-3.1
    ```

    Clone repo
    ```bash
    git clone --recursive https://github.com/deccer/UOStudio
    git submodule update --init --recursive --force
    ```

    Build UO Studio
    ```bash
    cd UOStudio
    dotnet build
    ```

    === "Fedora 31"

    Register dotnet repositories
    ```bash
    sudo rpm --import https://packages.microsoft.com/keys/microsoft.asc
    sudo wget -O /etc/yum.repos.d/microsoft-prod.repo https://packages.microsoft.com/config/fedora/31/prod.repo
    ```

    Install dotnet sdk
    ```bash
    sudo dnf install dotnet-sdk-3.1
    ```

    Clone repo
    ```bash
    git clone --recursive https://github.com/deccer/UOStudio
    git submodule update --init --recursive --force
    ```

    Build UO Studio
    ```bash
    cd UOStudio
    dotnet build
    ```

=== "ArchLinux"

    Install dotnet sdk
    ```bash
    sudo pacman -S dotnet-sdk-3.1
    ```

    !!! info

        More details in the [arch-wiki](https://wiki.archlinux.org/index.php/.NET_Core)

    Install mono
    ```bash
    sudo pacman -S mono
    ```

    Clone repo
    ```bash
    git clone --recursive https://github.com/deccer/UOStudio
    git submodule update --init --recursive --force
    ```

    Build UO Studio
    ```bash
    cd UOStudio
    dotnet build
    ```

=== "OSX"

    Install git
    ```bash
    brew install git
    ```

    Install [mono](https://www.mono-project.com/download/stable/)
    Run the .pkg to install mono.

    Install libgdiplus
    ```bash
    brew install mono-libgdiplus
    ```

    Install [dotnet sdk](https://dotnet.microsoft.com/download/dotnet-core/thank-you/sdk-3.1.403-macos-x64-installer)

    Clone repo
    ```bash
    git clone --recursive https://github.com/deccer/UOStudio
    git submodule update --init --recursive --force
    ```

    Build UO Studio
    ```bash
    cd UOStudio
    dotnet build
    ```

## Report a bug

[Report bugs here](https://github.com/deccer/NCentrED/issues/new?labels=untriaged&template=bug_report.md)

## Request a feature

[Request features here](https://github.com/deccer/UOStudio/issues/new?labels=feature-request&template=feature-request.md)
