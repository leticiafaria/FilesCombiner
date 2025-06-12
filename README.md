# 💻 Sobre o Files Combiner 

## 🚀 Visão Geral do Projeto

O `FilesCombiner` é uma aplicação desktop desenvolvida em **WPF (.NET 9)** que permite **combinar o conteúdo de múltiplos arquivos** de um diretório de projeto em um único arquivo de texto. A ferramenta é útil para gerar um "snapshot" do código-fonte, facilitando a análise, compartilhamento ou documentação do projeto.

A aplicação é inteligente e **ignora automaticamente pastas e arquivos desnecessários** (como diretórios de build, controle de versão, arquivos de log, etc.), garantindo que o arquivo de saída contenha apenas o código-fonte relevante e limpo.

---

## 📚 Funcionalidades Principais

* **Seleção de Pasta do Projeto**: Permite ao usuário selecionar visualmente a pasta raiz do projeto que deseja combinar os arquivos.
* **Combinação Inteligente de Arquivos**: Percorre recursivamente todas as subpastas e arquivos dentro do diretório do projeto selecionado, garantindo uma cobertura completa.
* **Exclusão Automática de Arquivos e Pastas Irrelevantes**: Ignora pastas como `.vs`, `bin`, `obj`, `.git`, `Deploy.hw.k8s`, `Deploy.k8s`, `.vscode`, `e2e`, `assets`, `environments`, `configs`, `core`. Também exclui arquivos com extensões como `.dll`, `.exe`, `.pdb`, `.zip`, `.rar`, `.tmp`, `.log`, `.bak`, `.json`, `.config`, `.sln`, `.csproj`, `.gitignore`, `.md`, `.conf`, `.prettierignore`, `.prettierrc`, `.browserslistrc`, `.editorconfig`, `.ico` e nomes específicos como `Dockerfile`, `karma.conf.js`, `polyfills.ts`. Isso assegura um output limpo e focado no código-fonte.

Para cenários de projeto distintos que não se enquadram nas configurações atuais de backend e backoffice, será necessário realizar ajustes manuais na classe `FileCombinerService`. Especificamente, as seguintes propriedades (`HashSet<string>`) devem ser comentadas ou modificadas para adequação:
* `ignoredFolderNames`: Linhas: `73 até 90`, abrangendo as `#region Exceções Backend` e `#region Exceções Frontend`.
* `ignoredFileExtensions`: Linhas: `96 até 112`, abrangendo as `#region Exceções Backend` e `#region Exceções Frontend`.
* `ignoredSpecificFileNames`: Linhas: `126 até 129`, definem nomes de arquivos específicos a serem ignorados.
* **Geração de Arquivo Único**: Salva todo o conteúdo combinado em um arquivo de texto especificado pelo usuário, com total controle sobre o nome e o local.

 ---

## 🚨 Alerta de Segurança

É fundamental **remover todas as informações sensíveis** da pasta do projeto que você deseja combinar os arquivos. Segue alguns exemplos:

* **Nomes de usuário**
* **Senhas**
* **Credenciais de cliente (Client Credentials)**
* **Strings de conexão (Connection Strings)**
* Quaisquer outras informações que possam comprometer a segurança de sistemas ou dados.
* 
---

## 🧪 Como Utilizar

### Requisitos Essenciais

* **Sistema Operacional Windows**
* **`.NET Desktop Runtime`** (versão compatível com a compilação do projeto)
* **Para Desenvolvimento**:
    * **Visual Studio** (com workload de "Desenvolvimento para desktop com .NET")
    * **OU** **VS Code** (com extensões C# e .NET SDK instaladas)

### Passos para Uso

1.  **Faça o download do projeto**:
    Clone o projeto do GitHub. Acesse a documentação de como clonar um repositório [**aqui**](https://docs.github.com/pt/repositories/creating-and-managing-repositories/cloning-a-repository).
#### Com Visual Studio

1.  **Abrir o Projeto**:
    * Abra o **Visual Studio**.
    * Vá em `Arquivo` > `Abrir` > `Projeto/Solução` e selecione o arquivo da solução (`.sln`) do projeto `FilesCombiner`.
2.  **Restaurar Pacotes NuGet**:
    * O Visual Studio deve **restaurar automaticamente** os pacotes NuGet necessários. Caso contrário, clique com o botão direito na solução no "Gerenciador de Soluções" e selecione "Restaurar Pacotes NuGet".
3.  **Compilar e Executar**:
    * Pressione `F5` ou clique no botão `Iniciar` (com o ícone de seta verde) na barra de ferramentas. Isso **compilará e executará a aplicação** para você testar.

#### Com VS Code

1.  **Abrir a Pasta do Projeto**:
    * Abra o **VS Code**.
    * Vá em `Arquivo` > `Abrir Pasta...` e selecione a pasta raiz do projeto `FilesCombiner` (a pasta que contém o arquivo `.csproj`).
2.  **Restaurar Pacotes NuGet**:
    * Abra o Terminal Integrado (`Ctrl+\` ou `Visualizar` > `Terminal`).
    * Execute o comando: `dotnet restore`
3.  **Executar a Aplicação**:
    * No Terminal Integrado, execute o comando: `dotnet run`
    * Isso **compilará e executará a aplicação**, deixando-a pronta para uso.

### Passos para Uso

1.  **Execute o projeto**:
    * Execute o projeto por meio da IDE ou versão compilada (arquivo .exe).
2.  **Selecione a Pasta do Projeto**:
    * Clique no botão **"Selecionar Pasta"**.
    * Navegue até a **pasta raiz do seu projeto** (aquela que contém o código-fonte que você deseja combinar) e clique em "OK". O caminho da pasta selecionada será exibido claramente na caixa de texto.
3.  **Combine os Arquivos**:
    * Após selecionar a pasta, clique no botão **"Combinar Arquivos"**.
    * Será aberta uma janela para você **escolher onde salvar o arquivo combinado** e qual nome dar a ele. Por padrão, ele sugerirá um nome útil como `files-combined_YYYY-MM-DD-HH-MM-SS.txt`.
    * Selecione o local e o nome do arquivo de saída e clique em **"Salvar"**.
4.  **Verifique o Resultado**:
    * Ao finalizar a consolidação dos arquivos será exibida a caixa de diálogo acima.

### Logs 📄

Os logs da aplicação são gravados em um arquivo de texto (`.txt`) na seguinte localização padrão do seu sistema:

`C:\Users\<SeuUsuario>\Downloads\FilesCombiner_logs\`

Cada execução da aplicação gerará um **novo arquivo de log com um timestamp** no nome (ex: `FilesCombiner_log_YYYY-MM-DD-HH-MM-SS.txt`), facilitando o rastreamento e a análise de eventos.
