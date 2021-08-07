# DCE

[![npm](https://img.shields.io/npm/l/react)](https://github.com/Branoliv/DCE/blob/master/LICENSE)


# SOBRE O PROJETO

<p align="justify">Aplicativo para documentação fotográfica da inspeção de containers para exportação, com armazenamento no sharepoint no qual o número do cotainer e número da ordem de carregamento são os identificadores do documento.  São criadas pastas diretamente dentro da estrutura de documentos do sharedpoint onde serão salvas as fotos 
 coletadas pelo aplicativo.</p>

## Layout App

<a href="https://github.com/Branoliv/DCE/blob/master/Assets/App">Todas imagens</a>

## Layout Docs Sharedpoint

<a href="https://github.com/Branoliv/DCE/blob/master/Assets/Sharedpoint">Todas imagens</a>

# Tecnologias utilizadas
## App :books:
     
   - [Microsoft .Graph](https://www.nuget.org/packages/Microsoft.Graph/3.21.0): versão 3.21.0
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

### Criar pasta com nome Security em:
~ DCE/
### Criar classe statica com nome OAuthSettings em :
~ DCE/Security/

### Incluir o código:

```C#

 public static class OAuthSettings
    {
        public static string Scopes = "{socopes}";
        public static string ApplicationId = "{}";
        public static string RedirectUri = "{}";
        public static string TenantId = "{}"; //Caso o acesso seja realizado apenas por usuários do locatário.
    }
    
```

### Criar arquivo XML com nome "secrets" em:
~DCE.Android/Resources/values

### Incluir o código:

```xml
<resources>
  <string name="application_id">{}</string>
  <string name="redirect_uri">{}</string>
  <string name="scheme_uri">{}</string>
</resources>

```

