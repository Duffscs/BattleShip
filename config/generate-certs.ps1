dotnet dev-certs https -ep ./certs/api-certificate.pfx -p 1234  --trust --format pfx
dotnet dev-certs https -ep ./certs/app-certificate.crt -np --trust --format pem