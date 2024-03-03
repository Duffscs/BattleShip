#!/bin/sh

echo "Configuration en cours..."

# Début du fichier JSON
echo "{" > /usr/local/webapp/nginx/html/appsettings.json

# Initialisation du flag à faux
nested=false

# Stockage et traitement de chaque variable d'environnement
printenv | sort | while IFS='=' read -r key value; do
    # Vérification si la clé contient un point
    if echo "$key" | grep -q "__"; then
        if [ "$nested" = false ]; then
            subobject_name=$(echo "$key" | awk -F"__" '{print $1}')
            echo "  \"$subobject_name\": {" >> /usr/local/webapp/nginx/html/appsettings.json
        fi
        subkey=$(echo "$key" | awk -F"__" '{print substr($0, index($0,$2))}')
        echo "    \"$subkey\": \"$value\"," >> /usr/local/webapp/nginx/html/appsettings.json
        nested=true
    else
        if [ "$nested" = true ]; then
            echo '    "trailling": "end"' >> /usr/local/webapp/nginx/html/appsettings.json
            echo "  }," >> /usr/local/webapp/nginx/html/appsettings.json
        fi
        echo "  \"$key\": \"$value\"," >> /usr/local/webapp/nginx/html/appsettings.json
        nested=false
    fi
done

if [ "$nested" = true ]; then
    echo "  }" >> /usr/local/webapp/nginx/html/appsettings.json
fi

echo '  "trailling": "end"' >> /usr/local/webapp/nginx/html/appsettings.json

echo "}" >> /usr/local/webapp/nginx/html/appsettings.json

echo "Configuration terminée."
