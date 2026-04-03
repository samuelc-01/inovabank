# InovaBank - Plataforma Bancária

Uma plataforma robusta de contas bancárias desenvolvida com foco em escalabilidade, resiliência e integridade de dados.  
O projeto utiliza DDD, CQRS e uma arquitetura orientada a eventos para garantir alta performance e consistência.

---

## Como Executar

O projeto foi totalmente containerizado. Para subir o ecossistema completo (API, Worker, Bancos e Broker), basta um único comando:

```bash
docker compose up -d --build
```

### Endpoints Principais

- **Swagger UI:** http://localhost:8080/swagger  
- **RabbitMQ Management:** http://localhost:15672 (guest/guest)  
- **API Base:** http://localhost:8080/api/v1  

---

## Arquitetura e Decisões

### 1. CQRS (Command Query Responsibility Segregation)

Separei as operações de escrita (Commands) das operações de leitura (Queries).

- **Write Side:** PostgreSQL para garantir transações ACID e integridade dos saldos.  
- **Read Side:** MongoDB para consultas rápidas de extrato e saldo, utilizando modelos de leitura desnormalizados (Read Models).

---

### 2. Consistência Eventual com Transactional Outbox

Para evitar a perda de eventos caso o Message Broker falhe, implementei o padrão Transactional Outbox via MassTransit.

- A mensagem do evento de transação é gravada na mesma transação do banco de dados relacional.  
- Um serviço em background (Worker) garante a entrega dessas mensagens ao RabbitMQ.

---

### 3. Idempotência

Operações financeiras utilizam uma `idempotencyKey` armazenada em Redis.

- Impede duplicidade de depósitos ou saques  
- Permite retransmissão segura de requisições

---

### 4. Validação e Domínio

- **CNPJ:** Validado via dígito verificador e enriquecido com dados da ReceitaWS  
- **Regras de Negócio:** Centralizadas no Agregado de `Account`  
  - Conta nunca fica com saldo negativo  
  - Transferências apenas entre contas ativas  

---

### 5. Padronização de Respostas

Utilizei de um `ApiControllerBase` com um método abstrato `HandleResult<T>`.
- **Uniformidade:** Garante que todos os endpoints retornem um formato de resposta consistente ao usuário.
- **Desacoplamento:** Centraliza a lógica de conversão entre os objetos Result da camada de Application e os ActionResults do ASP.NET Core (Ok, BadRequest, NotFound).

---

## Stack Tecnológica

| Tecnologia            | Função                                      |
|----------------------|---------------------------------------------|
| .NET 10              | Framework principal                         |
| Entity Framework Core | ORM para PostgreSQL                        |
| MongoDB Driver       | Persistência do Read Model                  |
| MassTransit          | Abstração e resiliência para RabbitMQ       |
| MediatR              | Implementação de CQRS e In-process messaging|
| FluentValidation     | Validação rigorosa de inputs                |
| Redis                | Cache distribuído e Idempotency Store       |
| Polly                | Resiliência nas chamadas na API ReceitaWS   |

---

## Melhorias Futuras

Caso houvesse mais tempo para o desafio, os próximos passos seriam:

- **Observabilidade:** OpenTelemetry + Jaeger (Distributed Tracing)  
- **Dead Letter Queues (DLQ):** Interface para reprocessamento manual  
- **Segurança:** OAuth2/JWT para proteção dos endpoints  
- **Testes de Integração:** TestContainers para validar fluxo completo  
- **Gestão de Incidentes:** Integração com Trello API para abertura automática de cards em exceções críticas via IExceptionHandler.

---

## Autor

[Guilherme Pacheco](https://github.com/DFaltGP)
