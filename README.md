# Identity-Service

Projeto em desenvolvimento utilizando **.NET 10**.

```bash
Packages:
> DotNetEnv                                  3.1.1        3.1.1    
> Microsoft.AspNetCore.OpenApi               10.0.3       10.0.3   
> Microsoft.EntityFrameworkCore.Design       10.0.0       10.0.0   
> Npgsql.EntityFrameworkCore.PostgreSQL      10.0.0       10.0.0
```

# 🚀 Como executar o projeto

### 1️⃣ Criar o arquivo de ambiente

Crie um arquivo `.env` na raiz do projeto com base no `.env-example`:

```bash
cp .env-example .env
```

Edite as variáveis conforme necessário.

---

### 2️⃣ Subir o banco de dados (PostgreSQL via Docker)

Entre na pasta `/docker` e execute:

```bash
docker compose --env-file ../.env up -d
```

Isso irá subir o container do PostgreSQL utilizando as variáveis definidas no `.env`.

---

### 3️⃣ Trabalhando com Migrations (Entity Framework)

#### ➕ Criar uma nova migration

```bash
dotnet ef migrations add <NomeDaMigration>
```

> ⚠️ Sempre revise o arquivo gerado antes de aplicar quaisquer modificações no banco de dados.

#### ➖ Remover a última migration

```bash
dotnet ef migrations remove
```

#### ▶️ Aplicar as migrations no banco

```bash
dotnet ef database update
```

Para rodar o projeto você vai precisar rodar as migrations, assim o banco de dados vai ficar compátivel com o códgio atual do projeto.