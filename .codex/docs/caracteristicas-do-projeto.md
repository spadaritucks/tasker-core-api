# Tasker API — Características do projeto

## 1. Visão geral

Tasker é uma API em estágio inicial de desenvolvimento para gerenciamento de agendas, com integração planejada ao Google Calendar. A proposta central é permitir que usuários autentiquem-se com uma conta Google, mantenham um ou mais calendários de planejamento e gerenciem eventos sincronizados com o Google Calendar.

Este documento consolida o contexto atual observado no código-fonte e no diagrama entidade-relacionamento disponível em [`database/mermaid-database-diagram-01.mermaid`](database/mermaid-database-diagram-01.mermaid). Ele diferencia funcionalidades já iniciadas de elementos que ainda estão somente no desenho de domínio.

## 2. Estado atual

O projeto possui uma solução .NET organizada em quatro projetos:

- `Tasker.API`: aplicação ASP.NET Core, controllers, configuração e inicialização.
- `Tasker.Application`: casos de uso, DTOs, exceções e filtro de tratamento de erros.
- `Tasker.Domain`: reservado para entidades e regras de negócio; ainda contém apenas um placeholder.
- `Tasker.Infrastructure`: reservado para persistência e integrações externas; ainda contém apenas um placeholder.

### Implementado

- Inicialização de uma API ASP.NET Core.
- Documentação/teste de endpoints via OpenAPI/Swagger em ambiente de desenvolvimento.
- Endpoint `POST /google` para validação de um `idToken` emitido pelo Google.
- Validação do token usando `GoogleJsonWebSignature`, restringindo o `audience` ao `ClientId` configurado.
- Retorno dos dados básicos validados do usuário: e-mail e nome.
- Tratamento padronizado para erros de autenticação e validação, incluindo respostas HTTP 400, 401, 404, 409 e 500.
- Configuração de injeção de dependência para `IGoogleClientUsecase`.

### Ainda não implementado

- Criação e armazenamento de usuários.
- Emissão ou persistência de sessão da aplicação.
- Autorização de requisições autenticadas.
- Entidades e repositórios do domínio.
- Banco de dados, migrations e mecanismo de persistência.
- CRUD de planners/calendários.
- CRUD de eventos.
- Sincronização de eventos com o Google Calendar.
- Notificações e controle de leitura.
- Testes automatizados e pipeline de validação observável no repositório.

## 3. Objetivo funcional

O produto deverá oferecer uma camada própria de gerenciamento de agenda sobre calendários Google:

1. O usuário autentica-se com o Google.
2. A aplicação identifica ou cria o usuário local.
3. O usuário associa calendários Google a planners da aplicação.
4. Eventos são criados, consultados, atualizados e removidos por meio da API.
5. Alterações são sincronizadas com o Google Calendar.
6. O sistema acompanha o estado da sincronização e comunica notificações relevantes ao usuário.

O diagrama atual modela o relacionamento entre usuário, sessões, notificações, planners e eventos. A cardinalidade proposta é:

```text
USER 1 ── N SESSION
USER 1 ── N NOTIFICATION
USER 1 ── N PLANNER
USER 1 ── N EVENT
PLANNER 1 ── N EVENT
```

## 4. Modelo de dados proposto

O modelo abaixo é derivado do diagrama e deve ser tratado como contrato de domínio em evolução, não como esquema já persistido.

| Entidade | Campos previstos | Responsabilidade |
| --- | --- | --- |
| `USER` | `id`, `name`, `email`, `created_at` | Identidade e dados básicos do usuário local. |
| `SESSION` | `id`, `user_id`, `access_token`, `created_at`, `expires_in`, `device_id`, `device_info`, `revoke` | Sessões e credenciais de acesso associadas a um usuário/dispositivo. |
| `PLANNER` | `id`, `google_calendar_id`, `user_id` | Representação local de um calendário Google pertencente ao usuário. |
| `EVENT` | `id`, `planner_id`, `title`, `description`, `date_start`, `date_end`, `google_html_link`, `sync_status`, `created_at` | Evento de agenda e seu vínculo com o calendário externo. |
| `NOTIFICATION` | `id`, `user_id`, `user_email`, `title`, `content`, `is_read`, `read_at` | Mensagens e avisos destinados ao usuário. |

### Pontos a esclarecer no modelo

- O diagrama relaciona `USER` diretamente a `EVENT`, mas a tabela `EVENT` contém `planner_id` e não `user_id`. É necessário decidir se o vínculo direto será mantido, removido ou materializado por consulta através de `PLANNER`.
- `SESSION.access_token` precisa de uma política explícita de proteção, rotação, expiração e revogação. Se for um token Google, também será necessário avaliar armazenamento de refresh token e escopos autorizados.
- `SESSION.expires_in` está desenhado como `datetime`, embora o nome costume representar uma duração em segundos. Recomenda-se padronizar o significado e o tipo.
- `EVENT.created_at` está desenhado como `string`; deve ser convertido para um tipo temporal consistente.
- `NOTIFICATION.user_email` é potencialmente redundante quando já existe `user_id`; sua necessidade deve ser justificada ou substituída por uma projeção.
- O modelo ainda não informa chaves primárias, índices, unicidade de e-mail, chaves estrangeiras, timezone ou estratégia de concorrência.

## 5. Arquitetura e dependências

A solução segue uma separação inicial por camadas, com dependências atualmente configuradas assim:

```text
Tasker.API → Tasker.Application → Tasker.Domain
                         └──────→ Tasker.Infrastructure → Tasker.Domain
```

Na prática, `Tasker.Application` referencia `Tasker.Infrastructure`, o que cria uma dependência invertida em relação ao padrão de arquitetura limpa mais comum. Quando a persistência começar, recomenda-se manter contratos/abstrações no domínio ou na aplicação e fazer a infraestrutura implementá-los, evitando que os casos de uso dependam diretamente de detalhes de persistência.

Tecnologias identificadas:

- .NET 10 (`net10.0`).
- ASP.NET Core Web API.
- OpenAPI/Swagger com Swashbuckle.
- `Google.Apis.Auth` para validação de tokens Google.
- Injeção de dependência nativa do ASP.NET Core.

## 6. API disponível hoje

### `POST /google`

Recebe um corpo semelhante a:

```json
{
  "idToken": "<token emitido pelo Google>"
}
```

O caso de uso valida o token contra o `ClientId` configurado e retorna:

```json
{
  "email": "usuario@exemplo.com",
  "name": "Nome do usuário"
}
```

Respostas previstas no código:

- `200 OK`: token válido.
- `400 Bad Request`: `idToken` ausente ou vazio.
- `401 Unauthorized`: token Google inválido.
- `500 Internal Server Error`: erro não tratado.

Observação: embora o controller declare `[Route("/api/[controller]")]`, o atributo `[HttpPost("/google")]` começa com `/` e define uma rota absoluta. Portanto, o endpoint efetivamente exposto pelo código é `/google`, e não `/api/Auth/google`.

## 7. Padrão de erros

As exceções específicas herdam de `TaskerException` e são convertidas pelo `ExceptionFilter` para um envelope global com `Success`, `Data`, `Error` e `Metadata`.

O padrão pretendido é adequado como ponto de partida, mas ainda precisa ser refinado:

- preencher `Metadata.Path` com o caminho real da requisição;
- gerar `Metadata.Timestamp` no instante da resposta;
- evitar expor detalhes internos em erros 500;
- padronizar namespaces, que atualmente usam referências como `Tasker.API.Middleware` dentro do projeto `Tasker.Application`;
- adicionar validação automática dos DTOs e códigos de erro estáveis para clientes.

## 8. Requisitos funcionais futuros

- Autenticar e identificar usuários por Google OAuth/OpenID Connect.
- Criar, consultar, atualizar e revogar sessões.
- Associar planners aos calendários Google autorizados.
- Criar, editar, consultar e excluir eventos.
- Sincronizar alterações locais e externas, registrando `sync_status`.
- Tratar conflitos, falhas temporárias, expiração de credenciais e reprocessamento seguro.
- Listar notificações, marcar como lidas e registrar `read_at`.
- Isolar completamente os dados de cada usuário.

## 9. Requisitos não funcionais e riscos

- **Segurança:** não manter `ClientSecret` nem tokens em arquivos versionados. Os arquivos de configuração atuais contêm credenciais Google aparentes; elas devem ser revogadas/rotacionadas e movidas para Secret Manager, variáveis de ambiente ou User Secrets.
- **Autorização:** validar identidade não é suficiente para proteger os recursos. Será necessário middleware de autenticação, claims e políticas de autorização.
- **Privacidade:** tokens e dados de agenda são sensíveis; aplicar criptografia em repouso, HTTPS, logs sem segredos e retenção mínima.
- **Idempotência:** operações de sincronização devem suportar retries sem duplicar eventos.
- **Tempo:** armazenar instantes em UTC e preservar timezone/IANA do evento quando necessário.
- **Observabilidade:** adicionar logs estruturados, correlação de requisições, métricas de sincronização e rastreamento de falhas.
- **Resiliência externa:** chamadas ao Google Calendar devem ter timeout, retry com backoff e tratamento de rate limit.
- **Compatibilidade:** fixar e atualizar dependências com revisão, pois o projeto já está direcionado a uma versão de framework muito recente.

## 10. Próximos incrementos recomendados

1. Remover credenciais expostas, rotacionar o segredo Google e definir a estratégia de configuração segura.
2. Corrigir o contrato de rota e definir a convenção oficial de versionamento (`/api/v1`, por exemplo).
3. Definir entidades, chaves, constraints, timezone e banco de dados; depois criar migrations.
4. Criar interfaces de repositório e serviços de domínio antes das implementações de infraestrutura.
5. Implementar autenticação completa: usuário local, sessão, claims e autorização.
6. Integrar a Google Calendar API com escopos mínimos e armazenamento seguro das credenciais necessárias.
7. Implementar planners e eventos com controle de ownership e estado de sincronização.
8. Adicionar testes unitários, testes de integração e exemplos HTTP atualizados.
9. Criar pipeline de build, análise estática, testes e verificação de segredos.

## 11. Referências internas

- Diagrama de dados: [`database/mermaid-database-diagram-01.mermaid`](database/mermaid-database-diagram-01.mermaid)
- Inicialização da API: `Tasker.API/Program.cs`
- Autenticação: `Tasker.API/Controllers/AuthController.cs` e `Tasker.Application/Usecases/GoogleClientUsecase/`
- Tratamento de erros: `Tasker.Application/Filters/ExceptionFilter.cs` e `Tasker.Application/Exceptions/`
