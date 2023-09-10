# LeitourApi

A Api utilizada pelo aplicativo mobile Leitour:

**Como Executar**
## Modo 1: VS Code
Requisitos:
  - MySql WorkBench  
  - Visual Studio Code
Etapas:
  1° Criar o banco de dados usando o init.sql em: raiz_do_projeto/LeitourApi
  2° Possuir um usuário MySql chamado root com senha '12345678', e ter uma conexão com MySql na porta 3306
  3° Dentro da raiz_do_projeto/LeitourApi, abra o terminal e digite: dotnet run
  4° Acessar a aplicação pelo navegador em
    http://localhost:<porta indicada pelo terminal>/swagger/index.html

**Como Executar**
## Modo 2: Docker
Requisitos: 
  - Docker
  - Docker Compose
Etapas:
  1° Garantir que o MySql ou outro programa não esteja usando a porta 3306
  2° Dentro da raiz_do_projeto/LeitourApi, abra o terminal e digite:
    docker-compose rm -vf
    docker-compose up --build --remove-orphans
  3° Acessar no navegador
    http://localhost:80/swagger/index.html
    ou
    http://<IP privado do host>:80/swagger/index.html
