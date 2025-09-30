# UserManagementAPI

API para gerenciamento de usuários com autenticação JWT e refresh token.

---

## Tecnologias utilizadas

* .NET 9
* C#
* ASP.NET Core Web API
* Entity Framework Core (SQLite)
* JWT (JSON Web Token) para autenticação
* Swagger para documentação

---

## Pré-requisitos

* [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
* Git
* Editor de código (VSCode, Visual Studio, Rider, etc.)

---

## Como rodar

1. Clonar o repositório:

```bash
git clone https://github.com/MatheusJuski/UserManagementAPI.git
cd UserManagementAPI
```

2. Configurar a conexão com SQLite em `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=UserManagementAPI.db"
},
"Jwt": {
  "Key": "sua_chave_super_secreta_256bits",
  "Issuer": "UserManagementAPI",
  "Audience": "UserManagementAPI",
  "DurationInMinutes": "60"
}
```

3. Rodar migrações (se houver) e iniciar a API:

```bash
dotnet build
dotnet run
```

4. A API estará disponível em: `https://localhost:5001` ou `http://localhost:5000`.

---

## Endpoints

### AuthController

* **POST** `/api/auth/login`

  * Body:

    ```json
    {
      "username": "usuario",
      "password": "senha123"
    }
    ```
  * Retorna:

    ```json
    {
      "token": "JWT_TOKEN",
      "refreshToken": "REFRESH_TOKEN",
      "expires": "2025-09-29T22:00:00Z"
    }
    ```

* **POST** `/api/auth/refresh-token`

  * Body:

    ```json
    {
      "refreshToken": "REFRESH_TOKEN"
    }
    ```
  * Retorna novo JWT e refresh token.

---

### UserController

> Todos os endpoints abaixo requerem **JWT no Header** (`Authorization: Bearer JWT_TOKEN`), exceto o endpoint público.

* **GET** `/api/user` - Retorna todos os usuários.
* **GET** `/api/user/{id}` - Retorna usuário por ID.
* **POST** `/api/user` - Cria um novo usuário.

  * Body:

    ```json
    {
      "username": "novoUsuario",
      "email": "email@teste.com",
      "passwordHash": "senha123",
      "role": "User"
    }
    ```
* **PUT** `/api/user/{id}` - Atualiza usuário.
* **DELETE** `/api/user/{id}` - Remove usuário (**Admin apenas**).
* **GET** `/api/user/publico` - Endpoint público, qualquer pessoa pode acessar.

---

## Testando com Swagger

Após rodar a API, acesse:

```
https://localhost:5001/swagger
```

* Clique em "Authorize" e insira o JWT:

```
Bearer SEU_TOKEN_AQUI
```

---

## Segurança

* Senhas armazenadas usando `PasswordHasher<User>` (hash + salt seguro).
* JWT para autenticação de endpoints protegidos.
* Refresh token para renovar o JWT sem pedir senha novamente.

---

## Contribuição

Pull requests são bem-vindos. Para mudanças grandes, abra uma issue primeiro para discutir o que você quer mudar.

---

## Licença

Este projeto está licenciado sob a MIT License.
