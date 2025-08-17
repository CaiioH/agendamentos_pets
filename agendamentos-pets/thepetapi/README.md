# 📌 Projeto: **Backend - Gerenciamento de Pets**

## 📖 Sobre o Projeto  
Este projeto consiste em uma aplicação baseada em **microserviços** para gerenciamento de pets e agendamentos. Ele é composto por diferentes serviços independentes que se comunicam via **RabbitMQ**, garantindo escalabilidade e eficiência na troca de informações.  

## 🏗 Arquitetura  

A aplicação é estruturada da seguinte forma:

- **PetApi**: Responsável pelo gerenciamento dos pets e consumo das APIs **TheCatApi** e **TheDogApi**.  
- **AgendaApi**: Realiza o CRUD de agendamentos de pets.  
- **Notifications**: Serviço para envio de e-mails quando um agendamento é criado.  
- **API Gateway**: Implementado com **YARP**, organiza e roteia as requisições para os serviços **PetApi** e **AgendaApi**.  

## 🚀 Funcionalidades  

### 🔹 **PetApi**  
- Criar um pet  
- Atualizar um pet  
- Deletar um pet  
- Listar todos os pets  
- Listar pets por espécie  
- Listar pets por raça  
- Buscar pet por ID  
- Listar todas as raças disponíveis por espécie  
- Buscar imagem de um pet por raça  

### 🔹 **AgendaApi**  
- Criar agendamento manual  
- Atualizar um agendamento  
- Deletar um agendamento  
- Listar todos os agendamentos  
- Listar agendamentos por e-mail  
- Listar agendamentos por serviço  
- Listar agendamentos por data  

### 🔹 **Notifications**  
- Envio de e-mail ao tutor do pet sempre que um agendamento é criado  

## 🔄 Fluxo da Aplicação  

### 📌 **Criação de Pet e Agendamento Automático**  
1. O usuário cadastra um pet na **PetApi**.  
2. A **PetApi** publica o evento `pet_created` no **RabbitMQ**.  
3. A **AgendaApi** consome essa mensagem e cria um **agendamento automático** de "Banho Grátis" para 7 dias depois (ajustando para o próximo dia útil se necessário).  
4. Após criar o agendamento com sucesso, a **AgendaApi** publica o evento `appointment_created`.  
5. O serviço **Notifications** consome essa mensagem e envia um **e-mail ao tutor** do pet.  

### 📌 **Fluxo de Agendamento Manual**  
1. O usuário agenda um serviço manualmente na **AgendaApi**, informando o nome do pet e e-mail do tutor.  
2. A **AgendaApi** publica a mensagem `pet_info_request` no **RabbitMQ**.  
3. A **PetApi** consome essa mensagem e verifica se o pet existe no banco de dados.  
4. Se o pet for encontrado, a **PetApi** publica a mensagem `pet_info_response`.  
5. A **AgendaApi** consome essa resposta e conclui o agendamento.  
6. Após a criação do agendamento, a **AgendaApi** publica `appointment_created`.  
7. O serviço **Notifications** consome essa mensagem e envia um **e-mail ao tutor** confirmando o agendamento.  

## 🛠 Tecnologias Utilizadas  

- **.NET 9.0** (C#)  
- **RabbitMQ** (Mensageria)  
- **MySQL** (Banco de Dados NoSQL)  
- **YARP** (API Gateway)  
- **Docker** (Containerização)  

## 🚀 Como Executar o Projeto  

1. Clone o repositório:  
   ```bash
   git clone https://git.gft.com/ctca/thepetapi.git
   ```  
2. Configure o **RabbitMQ** no Docker.
3. Utilize ```dotnet ef database update em cada Serviço```.
4. Execute os serviços **ApiGateway**, **PetApi**, **AgendaApi** e **Notifications** com ```dotnet watch run```.  
5. Configure o **API Gateway** se necessário e inicie a aplicação.  

## 📬 Contato  

Caso tenha dúvidas ou sugestões, entre em contato:  
📧 **E-mail:** caio.rhenrique@hotmail.com  
🔗 **LinkedIn:** [Caio Henrique](https://www.linkedin.com/in/caiorhenrique/)  
