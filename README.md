# Catalog API

API RESTful para gerenciamento de produtos com autenticação JWT, desenvolvida em **ASP.NET Core** com **Entity Framework Core**, **PostgreSQL** e **Docker**.



---
## Endpoints da API
A API disponibiliza endpoints para autenticação de usuários e gerenciamento de produtos, seguindo boas práticas REST.

### Autenticação
- Registro de usuários
- Login com geração de token JWT

### Produtos
- Criação de produtos
- Listagem paginada
- Ordenação dinâmica
- Consulta por ID
- Atualização
- Remoção

Todos os endpoints de produtos exigem autenticação via JWT.

---

## Tecnologias Utilizadas

* **.NET 8**
* **Entity Framework Core**
* **PostgreSQL**
* **JWT Authentication**
* **Docker & Docker Compose**
* **Swagger / OpenAPI**
* **FluentValidation**

---

## Passo a Passo Para Executar o Projeto com Docker 

### Pré-requisitos

* Docker Desktop instalado e em execução
* Docker Compose habilitado


**1️.** Clonar o repositório
Clone o repositório e entre na raiz do projeto.

```bash
git clone https://github.com/o-Davi/catalog-api.git
cd catalog-api
```


**2️.** Criar o arquivo `.env`

Dentro da pasta do projeto, crie um arquivo chamado `.env` e insira o seguinte conteúdo:

```env
POSTGRES_DB=catalogdb
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres

JWT_KEY=INSIRA-SUA-CHAVE-AQUI(32-caracteres) 
JWT_ISSUER=CatalogApi
JWT_AUDIENCE=CatalogApiUsers
```
ATENÇÃO: No campo "JWT_KEY" você precisa inserir a sua chave secreta de 32 caracteres.

**3️.** Suba os containers
No terminal, dentro da pasta do projeto, execute o comando abaixo para construir e subir os containers do Docker:

```bash
docker compose up -d --build
```

O Docker irá:

* Subir o PostgreSQL
* Subir a API
* Aplicar automaticamente as **migrations**
* Executar o **seed inicial** que gera 50 produtos


**4️.** Acesse a API

Você pode utilizar o swagger para testar as requisições da API 
* Swagger: `http://localhost:8080/swagger`

---
## Autenticação
Esta API utiliza JWT (JSON Web Token) para autenticação e autorização. Para testar endpoints protegidos diretamente pelo Swagger, siga os passos abaixo:

**1.** Acesse o endpoint de login:
```
POST /api/auth/login
```

**2.** Informe um e-mail e senha válidos (previamente registrados). A resposta retornará um objeto contendo o token JWT:
```
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiresAt": "2026-01-28T12:00:00Z"
}
```
**3.** Após copiar o token, no Swagger clique em Authotize (ícone de cadeado) e cole seu token, depois clique em Authorize e depois em Close.

Após isso, todos os endpoints protegidos poderão ser utilizados normalmente pelo Swagger.

---
## GET /api/produtos — Listagem de Produtos
Este endpoint retorna uma lista de produtos com suporte a paginação e ordenação dinâmica, permitindo maior flexibilidade na consulta dos dados.

###  Paginação
- page: define qual página será retornada

- pageSize: define quantos registros existirão por página

### MinPrice
Você pode definir um preço mínimo para sua busca, produtos abaixo do valor que você definiu não aparecerão na sua consulta.
### MaxPrice
Você pode também definir um preço máximo para sua busca, produtos acima do valor que você definiu não aparecerão na sua consulta.

### Ordenação (sortBy)

O parâmetro sortBy define qual campo do produto será usado para ordenar o resultado. O **sortBy** precisa ser utilizado em conjunto com o **sortDirection** para que a ordenação ocorra!

Você pode escolher entre os campos:
```
name 
```
```
price
```

### Direção da Ordenação (sortDirection)

Define o sentido da ordenação:

**1.** crescente
```
asc 
```

**2.** decrescente
```
desc 
```
Lembre-se de combinar o sortDirection com o sortBy para conseguir ordenar.

---
