#!/bin/bash

# ignoredups for history
cat <<EOF >> ~/.zshrc
setopt HIST_EXPIRE_DUPS_FIRST
setopt HIST_IGNORE_DUPS
setopt HIST_IGNORE_ALL_DUPS
setopt HIST_IGNORE_SPACE
setopt HIST_FIND_NO_DUPS
setopt HIST_SAVE_NO_DUPS
EOF

# install dotnet-ef
dotnet tool install --global dotnet-ef
dotnet tool install Nuke.GlobalTool --global

# install npm packages
npm install && npm run prepare

# add dotnet tools to path
cat <<EOF >> ~/.zshrc
path+=(
    "\$HOME/.dotnet/tools"
)
EOF

# add nuke complete to zsh
cat <<EOF >> ~/.zshrc
_nuke_zsh_complete()
{
    local completions=("\$(nuke :complete "\$words")")
    reply=( "\${(ps:\n:)completions}" )
}
compctl -K _nuke_zsh_complete nuke
EOF
