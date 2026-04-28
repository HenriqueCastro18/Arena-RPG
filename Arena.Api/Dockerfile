# Usa o SDK do .NET para compilar o código
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia os arquivos do projeto e compila
COPY . ./
RUN dotnet publish -c Release -o out

# Usa a versão mais leve do .NET para rodar a aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Configura a porta que o Render exige
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

# Executa o servidor (Verifique se o nome da sua DLL gerada é esse mesmo)
ENTRYPOINT ["dotnet", "Arena.Api.dll"]