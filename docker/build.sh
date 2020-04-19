curl -s https://api.github.com/repos/restoremonarchy/marketplace-web/releases/latest \
| grep "browser_download_url" \
| cut -d : -f 2,3 \
| tr -d \" \
| wget -qi -

mkdir app
unzip Marketplace-Web-1.0.4.zip -d app