# 🥗 Sistema de Gerenciamento de Receitas Culinárias

![Badge em Desenvolvimento](http://img.shields.io/static/v1?label=STATUS&message=EM%20DESENVOLVIMENTO&color=GREEN&style=for-the-badge)
![Badge .NET 6](https://img.shields.io/badge/.NET%206-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)

## 📝 Descrição

Desenvolvi um sistema web completo para gerenciamento de receitas culinárias, utilizando **ASP.NET Core Razor Pages** e **.NET 6**. 

O sistema permite que usuários cadastrem, editem e excluam receitas, gerenciem ingredientes e interajam através de comentários. O foco foi criar uma aplicação performática utilizando manipulação direta de banco de dados para máxima otimização.

## 🛠️ Tecnologias Utilizadas

### Backend
* **ASP.NET Core Razor Pages**
* **.NET 6**
* **ADO.NET / System.Data.SqlClient** (Manipulação direta de dados)

### Frontend
* **HTML5 & CSS3**
* **JavaScript**
* **Bootstrap**

### Dados & Segurança
* **SQL Server**
* **Autenticação via Cookies**

---

## 💻 Como executar o projeto

### Pré-requisitos
* [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
* [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads) e SSMS (Management Studio)
* [Visual Studio 2022](https://visualstudio.microsoft.com/)

### Passo a Passo

1. **Clone o repositório**
   ```bash
   git clone [https://github.com/JuanJorgeDEV/ProjReceitas.git](https://github.com/JuanJorgeDEV/ProjReceitas.git)
   ```
   
2. **Configure o Banco de Dados**

-Na pasta raiz do projeto, localize o arquivo script_banco.sql.
-Abra o SQL Server Management Studio (SSMS).
-Execute o script dentro dele para criar o banco (db_Receitas) e as tabelas.
-Importante: Atualize a Connection String no arquivo SistemaReceitas.json na pasta 'Configuration' com os dados do seu banco local.

## 🕹️ Primeiros Passos

Assim que o projeto estiver rodando, você será direcionado para a página inicial.

Para testar todas as funcionalidades (como criar e editar receitas):
1. Clique no botão de **Login/Registro** no menu superior.
2. Crie uma nova conta de usuário (o cadastro é simples e local).
3. Após o login, você terá permissão total para gerenciar receitas, ingredientes e avaliações.

---

## 🤝 Agradecimentos

Obrigado por acessar este repositório!

Este projeto foi meu primeiro projeto desenvolvido, com muita dedicação para aplicar conceitos fundamentais de desenvolvimento web com .NET. Críticas construtivas, sugestões de refatoração e *feedbacks* são muito bem-vindos para o meu crescimento profissional.

---

## 📞 Contato

Se você tiver alguma dúvida sobre a implementação ou quiser trocar ideias sobre desenvolvimento .NET, entre em contato:

**Juan Jorge**

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0077B5?style=for-the-badge&logo=linkedin&logoColor=white)](www.linkedin.com/in/juan-jorge-ba2711222) 
[![Gmail](https://img.shields.io/badge/Gmail-D14836?style=for-the-badge&logo=gmail&logoColor=white)](mailto:juanjorgedm@gmail.com)

---

<p align="center">
  Desenvolvido por Juan Jorge 🚀
</p>
