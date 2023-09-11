# LeitourApi

A Api utilizada pelo aplicativo mobile  <a href="https://github.com/LucasRabaquim/Leitour">Leitour</a>:

**##Como Executar**
### Modo 1: VS Code
Requisitos:
  - MySql WorkBench  
  - Visual Studio Code
Etapas:
  1° Criar o banco de dados usando o init.sql em: raiz_do_projeto/LeitourApi
  2° Possuir um usuário MySql chamado root com senha '12345678', e ter uma conexão com MySql na porta 3306
  3° Garantir que o MySql server esteja rodando na porta 3306
  4° Dentro da raiz_do_projeto/LeitourApi, abra o terminal e digite: dotnet run
  5° Acessar a aplicação pelo navegador em
    http://localhost:<porta indicada pelo terminal>/swagger/index.html
    ou
    https://localhost:<porta indicada pelo terminal>/swagger/index.html

**##Como Executar**
### Modo 2: Visual Studio
Requisitos:
  - MySql WorkBench  
  - Visual Studio
Etapas:
  1° Criar o banco de dados usando o init.sql em: raiz_do_projeto/LeitourApi
  2° Possuir um usuário MySql chamado root com senha '12345678', e ter uma conexão com MySql na porta 3306
  3° Garantir que o MySql server esteja rodando na porta 3306
  4° Dentro da raiz_do_projeto, abra o projeto LeitourApi.sln no Visual Studio e execute
  5° Acessar a aplicação pelo navegador em
    http://localhost:<porta indicada pelo terminal>/swagger/index.html
    ou
    https://localhost:<porta indicada pelo terminal>/swagger/index.html

**Como Executar**
### Modo 3: Docker
Requisitos: 
  - Docker
  - Docker Compose
Etapas:
  1° Garantir que o MySql ou outro programa **não** esteja usando a porta 3306
  2° Dentro da raiz_do_projeto/LeitourApi, abra o terminal e digite:
    docker-compose rm -vf
    docker-compose up --build --remove-orphans
  3° Acessar no navegador
    http://localhost:80/swagger/index.html
    ou
    http://<IP privado do host>:80/swagger/index.html
  *Na execução do container da Aplicação Asp.Net Core o script de criação do banco é executado sozinho. Caso contrário tente os métodos anteriores
