# LeitourApi

A Api utilizada pelo aplicativo mobile  <a href="https://github.com/LucasRabaquim/Leitour">Leitour</a>:

## Como Executar
### Modo 1: VS Code
Requisitos:
  - MySql WorkBench   <br>
  - Visual Studio Code <br>
Etapas:
  1° Criar o banco de dados usando o init.sql em: raiz_do_projeto/LeitourApi <br>
  2° Possuir um usuário MySql chamado root com senha '12345678', e ter uma conexão com MySql na porta 3306 <br>
  3° Garantir que o MySql server esteja rodando na porta 3306 <br>
  4° Dentro da raiz_do_projeto/LeitourApi, abra o terminal e digite: dotnet run <br>
  5° Acessar a aplicação pelo navegador em <br>
    http://localhost:< porta indicada pelo terminal >/swagger/index.html <br>
    ou <br>
    https://localhost:< porta indicada pelo terminal >/swagger/index.html <br>
 <br> <br>
## Como Executar
### Modo 2: Visual Studio
Requisitos: <br>
  - MySql WorkBench   <br>
  - Visual Studio <br>
Etapas: <br>
  1° Criar o banco de dados usando o init.sql em: raiz_do_projeto/LeitourApi <br>
  2° Possuir um usuário MySql chamado root com senha '12345678', e ter uma conexão com MySql na porta 3306 <br>
  3° Garantir que o MySql server esteja rodando na porta 3306 <br>
  4° Dentro da raiz_do_projeto, abra o projeto LeitourApi.sln no Visual Studio e execute <br>
  5° Acessar a aplicação pelo navegador em <br>
    http://localhost:< porta indicada pelo terminal >/swagger/index.html <br>
    ou <br>
    https://localhost:< porta indicada pelo terminal >/swagger/index.html <br>
 <br> <br>
## Como Executar
### Modo 3: Docker
Requisitos:  <br>
  - Docker <br>
  - Docker Compose <br>
Etapas: <br>
  1° Garantir que o MySql ou outro programa **não** esteja usando a porta 3306 <br>
  2° Dentro da raiz_do_projeto/LeitourApi, abra o terminal e digite: <br>
    docker-compose rm -vf <br>
    docker-compose up --build --remove-orphans <br>
  3° Acessar no navegador <br>
    http://localhost:80/swagger/index.html <br>
    ou <br>
    http://< IP privado do host >:80/swagger/index.html <br>
  *Na execução do container da Aplicação Asp.Net Core o script de criação do banco é executado sozinho. Caso contrário tente os métodos anteriores <br>
