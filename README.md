Objetivo:<br />

Desenvolver serviços para suportar um fluxo de negócio responsável pela realização de transações bancárias.


Serviços Disponíveis:

✅ API de Usuários<br />
✅ API de Clientes<br />
✅ API de Transações bancárias<br />

Arquitetura do Projeto:<br /><br />
<img width="1823" height="680" alt="image" src="https://github.com/user-attachments/assets/a305ed5f-0b3e-4875-b70e-1e4622076ca3" />



Documentação para execução do Projeto:

🎲 Banco Dados (SQL Lite):

Bases de dados:

✅ Techfin.Users.db<br />
✅ Techfin.Customers.db<br />
✅ Techfin.Transactions.db<br />


Mensageria:

No serviço de transações bancárias, existe a dependência do RabbitMQ que irá gerar um evento, que será consumido pelo serviço do Cliente
RabbitMQ configurado localmente.

<img width="1006" height="257" alt="image" src="https://github.com/user-attachments/assets/b7b0f05c-4c1a-412e-8921-d5f0b9032f92" />

<br />
Scripts para geração de tabelas:<br />


<img width="1229" height="414" alt="image" src="https://github.com/user-attachments/assets/f34135a0-d5b9-4740-83ab-d3f2e706f845" />

Cada base de dados, fica salva em seu respectivo projeto:

Base de Usuarios

<img width="824" height="263" alt="image" src="https://github.com/user-attachments/assets/b70a14a0-24dc-48e9-b5fe-c5a0cbafd150" />

Base de Clientes

<img width="732" height="264" alt="image" src="https://github.com/user-attachments/assets/5b7f48a0-a70a-480b-a16a-e94a27726ca6" />

Base de Transações

<img width="819" height="280" alt="image" src="https://github.com/user-attachments/assets/5211db18-df35-4c47-8591-a7992f86315b" />

Transação de Débito:

A efetivação do débito na conta do cliente, por simplificação foi colocada na API de clientes.

BackgroundService:

<img width="920" height="696" alt="image" src="https://github.com/user-attachments/assets/476fbe68-c1f7-4949-ac3a-04debf1febd9" />

Realização de uma Transação:

A arquitetura deste projeto foi pensada para que cada serviço seja executado de forma apartada.
Desta forma, sugiro que, para a execução local, seja feita a configuração abaixo para que as 3 APIs sejam executadas simultaneamente:

Visual Studio:

<img width="1237" height="862" alt="image" src="https://github.com/user-attachments/assets/3c2c24c9-1641-4ac3-af1f-ee68c5204517" />

APIs em Funcionamento:

<img width="2545" height="746" alt="image" src="https://github.com/user-attachments/assets/5caf6b43-67d5-4404-adbc-c9b7ed711a4e" />
<br />

Executando uma transação Completa:

Cadastro de Usuário:


<img width="1992" height="274" alt="image" src="https://github.com/user-attachments/assets/b05bb091-b68c-45fe-a84d-230792a1b37c" />

Geração de Token:

<img width="2012" height="314" alt="image" src="https://github.com/user-attachments/assets/ba7bb9a8-455e-411a-a233-84249f074cb3" />

Cadastro de Cliente:

<img width="1892" height="374" alt="image" src="https://github.com/user-attachments/assets/a83e818d-d3f0-49ad-8c54-84ae91592e9c" />

Transação:

<img width="1871" height="335" alt="image" src="https://github.com/user-attachments/assets/8cd249f7-bf01-47bf-802a-e4cfcbc293b4" />

<img width="1821" height="437" alt="image" src="https://github.com/user-attachments/assets/72f24e44-f84c-4b02-a9bd-fba8158c751b" />

Banco de dados:

Usuário:

<img width="800" height="300" alt="image" src="https://github.com/user-attachments/assets/dbc39798-00e3-4028-ab50-0c6e145ec80d" />

Cliente: + Evento

<img width="800" height="300" alt="image" src="https://github.com/user-attachments/assets/b681d2de-2222-4b03-8310-1e5ddd803876" />

Transação

<img width="800" height="300" alt="image" src="https://github.com/user-attachments/assets/c51c717f-9b0e-4fc8-9b99-8cbbc1cdd769" />



<br /><br />
Desenvolvedor:<br />


Sidney Cesar dos Santos


Novembro | 2026
