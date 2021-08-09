# DCE

[![npm](https://img.shields.io/npm/l/react)](https://github.com/Branoliv/DCE/blob/master/LICENSE)


## SOBRE O PROJETO

<p align="justify">Aplicativo para documentação fotográfica da inspeção de containers para exportação, com armazenamento no sharepoint no qual o número do cotainer e número da ordem de carregamento são os identificadores do documento.  São criadas pastas diretamente dentro da estrutura de documentos do sharedpoint onde serão salvas as fotos 
 coletadas pelo aplicativo.</p>

## Layout App

<a href="https://github.com/Branoliv/DCE/blob/master/Assets/App/">Todas imagens</a>

## Layout Docs Sharedpoint

<a href="https://github.com/Branoliv/DCE/blob/master/Assets/Sharedpoint/">Todas imagens</a>

# Tecnologias utilizadas
## App :books:
     
   - [Microsoft .Graph](https://github.com/microsoftgraph/msgraph-sdk-dotnet): versão 3.21.0
   - [Microsoft.Identity.Client](https://www.nuget.org/packages/Microsoft.Identity.Client/): versão 4.24.0
   - [TimeZoneConverter](https://github.com/mattjohnsonpint/TimeZoneConverter): versão 3.3.0
   - [Xam.Plugin.Media](https://github.com/jamesmontemagno/MediaPlugin): versão 5.0.1
   - [Xamarin.Forms](https://github.com/xamarin/Xamarin.Forms): versão 5.0.0.2012
   - [Xamarin.Essentials](https://github.com/xamarin/Essentials): versão 1.6.1
   - [Microsoft.EntityFrameworkCore.Sqlite](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Sqlite/): versão 3.1.13
   - [Microsoft.EntityFrameworkCore.Tools](https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Tools): versão 3.1.13

## Sharedpoint :books:

   - [Sharedpoint](https://www.microsoft.com/pt-br/microsoft-365/sharepoint/collaboration)


# Como executar o projeto

## App

```bash
# clonar repositório
git clone https://github.com/Branoliv/DCE/DCE
```

### Criar pasta com nome "Security" em:
~ DCE/
### Criar classe statica com nome "OAuthSettings" em :
~ DCE/Security/

### Incluir o código:

```C#

 public static class OAuthSettings
    {
        public static string Scopes = "{SCOPES}"; // Os escopos devem ser serparados por espaço
        public static string ApplicationId = "{APPLICATION_ID}";
        public static string RedirectUri = "{REDIRECT_URI}";
        public static string TenantId = "{TENANT_ID}"; //Caso o acesso seja realizado apenas por usuários do locatário.
    }
    
```

### Criar arquivo XML com nome "secrets" em:
~DCE.Android/Resources/values

### Incluir o código:

```xml
<resources>
  <string name="application_id">{APPLICATION_ID}</string>
  <string name="redirect_uri">{REDIRECT_URI}</string>
  <string name="scheme_uri">{SCHEME_URI}</string>
</resources>

```

### Registar o aplicativo no Azure AD conforme documentação:

- [Documentação Active Directory](https://docs.microsoft.com/pt-br/azure/active-directory/develop/quickstart-register-app)
- [Acessar Microsfot Graph](https://docs.microsoft.com/pt-br/graph/tutorials/xamarin)


## Sharedpoint

### Criar site com nome DCE
<p align="justify">Dentro da estrutura de documentos do site deverão ser criadas as pastas quais serão configuradas no app para receber as fotos</p>

# Autor

Cleber Brandão de Oliveira

- [Linkedin](https://www.linkedin.com/in/cleber-brand%C3%A3o-3a631a133)
