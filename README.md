# FIAP — API de Solicitações de Documentos Acadêmicos

API RESTful desenvolvida em .NET 8 (C#) para o gerenciamento de solicitações de documentos acadêmicos por alunos da FIAP, eliminando a necessidade de atendimento presencial ou via secretaria.

---

## 🛠️ Tecnologias Utilizadas

- Plataforma: .NET 8.0 SDK
- Linguagem: C# 12
- Documentação & Testes Interativos: Swagger UI / OpenAPI
- Persistência em Memória: ConcurrentDictionary (Thread-Safe)

---

## 🏗️ Arquitetura e Estrutura da Solução

O projeto segue uma estrutura baseada em camadas (DDD simplificado) para garantir separação de responsabilidades e código limpo:

Fiap.SolicitacoesDocs.Api/
├── Application/              # Casos de uso, DTOs e regras de negócio
│   ├── Dtos/                 # Contratos de entrada e saída
│   └── Services/             # Serviços da aplicação
├── Domain/                   # Entidades, Enums e Interfaces
│   ├── Entities/             # Entidade Solicitacao
│   ├── Enums/                # TipoDocumento e SituacaoSolicitacao
│   └── Repositories/         # Interface do Repositório
├── Infrastructure/           # Persistência e repositórios concretos
│   └── Repositories/         # Implementação InMemory
├── Controllers/              # Endpoints REST da API
└── Program.cs                # Configuração de DI, Middlewares e Swagger

---

## ⚙️ Como Executar Localmente

### Pré-requisitos
- .NET 8.0 SDK instalado.

### Passos de Execução
1. Extraia o arquivo .zip em uma pasta de sua preferência.
2. Abra o terminal / prompt de comando na pasta do projeto (Fiap.SolicitacoesDocs.Api).
3. Restaure as dependências e compile o projeto:
   dotnet build
4. Execute a aplicação:
   dotnet run
5. Acesse a documentação do Swagger no seu navegador:
   - URL Swagger: http://localhost:5000/swagger ou https://localhost:5001/swagger

---

## 📌 Endpoints da API

A API disponibiliza as operações exigidas através de 3 endpoints principais:

| Método | Endpoint | Descrição |
|---|---|---|
| POST | /api/solicitacoes | Abre uma nova solicitação de documento. |
| GET | /api/solicitacoes/aluno/{rm} | Consulta todas as solicitações (Abertas e Canceladas) de um RM. |
| PATCH | /api/solicitacoes/{id}/cancelar | Cancela uma solicitação Aberta pelo seu ID (Guid). |

---

## 📋 Mapeamento dos Critérios de Aceite (CA)

| Código | Critério de Aceite | Implementação e Resposta HTTP |
|---|---|---|
| CA01 | Abertura: Dados válidos registram a solicitação e retornam os dados. | Retorna 201 Created com objeto contendo Id (Guid), DataAbertura e status Aberta. |
| CA02 | Validação: Recusa RM <= 0, descrição vazia/espaços ou tipo inválido. | Retorna 400 Bad Request com o motivo legível no corpo da resposta. |
| CA03 | Duplicidade: Impede 2 solicitações Abertas do mesmo tipo para o mesmo RM. | Retorna 409 Conflict informando a duplicidade. Permite tipos diferentes no mesmo RM. |
| CA04 | Consulta por RM: Retorna todas as solicitações (Abertas e Canceladas). | Retorna 200 OK com a lista. Retorna [] (lista vazia) caso o RM não possua registros. |
| CA05 | Cancelamento: Altera o status para Cancelada sem excluir o registro. | Retorna 204 No Content mantendo os dados e a data de abertura originais intactos. |
| CA06 | Cancelamento Inválido: Recusa solicitação inexistente ou já cancelada. | Retorna 404 Not Found se ID não existir e 400 Bad Request se já estiver cancelada. |
| CA07 | Nova Abertura: Permite nova abertura para mesmo RM e Tipo se a anterior estiver cancelada. | POST /api/solicitacoes gera um novo Guid e preserva o histórico da solicitação cancelada. |

---

## 💡 Decisões de Projeto

- Armazenamento em Memória: Utilização de ConcurrentDictionary injetado como Singleton no contêiner de DI. Isso garante que os dados persistam entre as requisições da mesma execução com segurança em acessos concorrentes (thread-safe).
- Tratamento Semântico de Erros: Mapeamento explícito de exceções no Controller (ArgumentException, InvalidOperationException, KeyNotFoundException) para retornar status códigos HTTP padronizados (400, 404, 409).
- Enums no Swagger: Configuração do JsonStringEnumConverter para exibir e receber os valores de TipoDocumento (DeclaracaoMatricula, HistoricoEscolar) como texto direto na interface do Swagger UI.
