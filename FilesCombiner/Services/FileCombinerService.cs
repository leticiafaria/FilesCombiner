using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace FilesCombiner.Services
{
    public class FileCombinerService
    {
        private readonly ILogger<FileCombinerService> _logger;
        public FileCombinerService(ILogger<FileCombinerService> logger)
        {
            _logger = logger;
        }
        public string CombineProjectFiles(string projectRootFolderPath, string outputFilePath)
        {
            const string method = $"{nameof(FileCombinerService)}.{nameof(CombineProjectFiles)}";

            if (Directory.Exists(projectRootFolderPath) is false)
            {
                var mensagem = $"Erro: A pasta de projeto especificada não existe: {projectRootFolderPath}";
                _logger.LogError("{Method} - A pasta de projeto especificada não existe: {projectRootFolderPath}", method, projectRootFolderPath);
                return mensagem;
            }

            var combinedContent = new StringBuilder();

            try
            {
                ProcessDirectory(projectRootFolderPath, combinedContent);

                string outputDirectory = Path.GetDirectoryName(outputFilePath) ?? string.Empty;
                if (!string.IsNullOrEmpty(outputDirectory) && !Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                    _logger.LogInformation("{Method} - Diretório de saída criado: {OutputDirectory}", method, outputDirectory);
                }

                File.WriteAllText(outputFilePath, combinedContent.ToString(), Encoding.UTF8);

                _logger.LogInformation("{Method} - Arquivos combinados com sucesso em: {OutputFilePath}", method, outputFilePath);

                return $"Sucesso! Todos os arquivos do projeto foram combinados em: {outputFilePath}";
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.LogError(exception, "{Method} - Por favor, verifique se você tem permissão para acessar todos os arquivos e pastas no caminho: {projectRootFolderPath}",
                    method, projectRootFolderPath);

                return $"Erro de permissão: {exception.Message} Por favor, verifique se você tem permissão para acessar todos os arquivos e pastas no caminho: {projectRootFolderPath}";
            }
            catch (PathTooLongException exception)
            {
                var mensagem = $"Erro: O caminho do arquivo ou pasta é muito longo. {exception.Message}\n" +
                    $"Por favor, tente usar um caminho mais curto ou mova o projeto para um local com um caminho mais curto.";
                _logger.LogError(exception, mensagem + "{projectRootFolderPath}", method, projectRootFolderPath);

                return mensagem;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "{Method} - Ocorreu um erro inesperado.", method);

                return $"Erro inesperado: {exception.Message}\n. Por favor, verifique o log para mais detalhes.";
            }
        }

        private void ProcessDirectory(string currentDirectory, StringBuilder combinedContent)
        {
            // Define as pastas a serem ignoradas.
            // Convertemos para HashSet para buscas mais eficientes, especialmente com muitos itens.
            HashSet<string> ignoredFolderNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
#region Exceções Backend
                ".vs",
                "bin",
                "obj",
                ".git",
                "Deploy.hw.k8s",
                "Deploy.k8s",
#endregion
#region Exceções Frontend
                ".vscode",
                "Deploy.k8s",
                "e2e",
                "assets",
                "environments",
                "configs",
                "core",
                ".git"
#endregion               
        };

            // Define as extensões de arquivo a serem ignoradas
            HashSet<string> ignoredFileExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
#region Exceções Backend
                ".dll",
                ".exe",
                ".pdb",
                ".zip",
                ".rar",
                ".tmp",
                ".log",
                ".bak",
                ".json",
                ".config",
                ".sln",
                ".csproj",
                ".gitignore",
                ".md",
#endregion
#region Exceções Frontend
                ".conf",
                ".prettierignore",
                ".prettierrc",
                ".browserslistrc",
                ".editorconfig",
                ".ico"
#endregion

        };

            // Define nomes de arquivos específicos a serem ignorados (sem considerar extensão, ou com extensão completa)
            HashSet<string> ignoredSpecificFileNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Dockerfile",
            "karma.conf.js",
            "karma.conf.js",
            "polyfills.ts"
        };

            _logger.LogInformation("Processando a pasta: {CurrentDirectory}", currentDirectory);

            foreach (string file in Directory.GetFiles(currentDirectory))
            {
                try
                {
                    string fileName = Path.GetFileName(file);
                    string fileExtension = Path.GetExtension(file);

                    // Ignora arquivos com extensões específicas
                    if (ignoredFileExtensions.Contains(fileExtension))
                    {
                        _logger.LogInformation("Ignorando arquivo por extensão: {file}", file);
                        continue;
                    }

                    // Ignora arquivos por nome específico
                    if (ignoredSpecificFileNames.Contains(fileName))
                    {
                        _logger.LogInformation("Ignorando arquivo por nome: {file}", file);
                        continue;
                    }

                    combinedContent.AppendLine(File.ReadAllText(file, Encoding.UTF8));
                }
                catch (IOException exception)
                {
                    _logger.LogError(exception, "Erro ao ler o arquivo {FilePath}", file);
                }
                catch (UnauthorizedAccessException exception)
                {
                    _logger.LogError(exception, "Permissão negada para o arquivo {FilePath}", file);
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception, "Erro inesperado ao processar o arquivo {FilePath}", file);
                }
            }

            foreach (string subdirectory in Directory.GetDirectories(currentDirectory))
            {
                string folderName = new DirectoryInfo(subdirectory).Name;

                // Verifica se o nome da subpasta está na lista de pastas a serem ignoradas
                if (ignoredFolderNames.Contains(folderName))
                {
                    _logger.LogInformation("Ignorando pasta: {Subdirectory}", subdirectory);
                    continue;
                }

                try
                {
                    ProcessDirectory(subdirectory, combinedContent);
                }
                catch (UnauthorizedAccessException exception)
                {
                    _logger.LogError(exception, "Permissão negada para acessar a subpasta {Subdirectory}", subdirectory);
                }
                catch (PathTooLongException exception)
                {
                    _logger.LogError(exception, "Caminho muito longo para a subpasta {Subdirectory}", subdirectory);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado ao processar a subpasta {Subdirectory}", subdirectory);
                }
            }

            _logger.LogInformation("Processamento concluído para a pasta: {CurrentDirectory}", currentDirectory);
        }
    }
}
