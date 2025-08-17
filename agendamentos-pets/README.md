# 📌 Guia: Executando Frontend e Backend com Docker Compose

## 📖 Sobre Este Guia

Este documento explica como rodar toda a aplicação (frontend e backend) utilizando **Docker Compose**. O objetivo é facilitar a inicialização do projeto sem precisar configurar os serviços individualmente.

## 🏗 Estrutura do Projeto
A estrutura de pastas do projeto segue este padrão:

```
project-root/
│── docker-compose.yml
│── frontend/   # Aplicação Angular
│── thepetapi/  # Microserviços do backend
│   ├── ApiGateway/
│   ├── PetApi/
│   ├── AgendaApi/
│   ├── Notifications/
```

## 🚀 Executando com Docker Compose

### 📌 Pré-requisitos
Certifique-se de que você tem instalado:
- [Docker](https://docs.docker.com/get-docker/)
- [Docker Compose](https://docs.docker.com/compose/install/)

### ▶️ Passos para execução

1. **Clone o repositório:**
   ```bash
   git clone https://git.gft.com/ctca/thepetapi.git
   cd thepetapi
   ```

2. **Inicie os containers:**
   ```bash
   docker-compose up -d
   ```

3. **Acesse os serviços:**
   - **Frontend:** [`http://localhost:4200`](http://localhost:4200)
   - **API Gateway:** [`http://localhost:5159`](http://localhost:5159)
   - **RabbitMQ Management:** [`http://localhost:15672`](http://localhost:15672) (Usuário: `guest`, Senha: `guest`)

### 🛑 Parar os containers
Para interromper os containers, execute:
```bash
docker-compose down
```

## 🐞 Resolvendo Problemas

### ⚠️ **Porta em Uso**
Se alguma porta já estiver em uso, verifique os containers em execução:
```bash
docker ps
```
Pare um container específico com:
```bash
docker stop <container_id>
```

Se precisar remover todos os containers:
```bash
docker-compose down -v
```

### 🔄 **Recriando Containers**
Se fizer alterações no código e precisar reconstruir os containers:
```bash
docker-compose up --build -d
```

## 📬 Contato
Caso tenha dúvidas ou sugestões, entre em contato:  
📧 **E-mail:** caio.rhenrique@hotmail.com  
🔗 **LinkedIn:** [Caio Henrique](https://www.linkedin.com/in/caiorhenrique/)

