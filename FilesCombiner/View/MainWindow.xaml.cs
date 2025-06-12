using System.IO;
using System.Windows;
using FilesCombiner.Services;
using Microsoft.Extensions.Logging;

namespace FilesCombiner
{
    public partial class MainWindow : Window
    {
        private readonly FileCombinerService _combiner;
        private readonly ILogger<MainWindow> _logger;
        private string _selectedFolderPath;
        public MainWindow(FileCombinerService combiner, ILogger<MainWindow> logger)
        {
            InitializeComponent();
            _combiner = combiner;
            _logger = logger;
            _selectedFolderPath = string.Empty;
        }

        private void BtnSelectFolder_Click(object sender, RoutedEventArgs e)
        {
            const string method = $"{nameof(MainWindow)}.{nameof(BtnSelectFolder_Click)}";

            _logger.LogInformation("{Method} - Clique no botão 'Selecionar Pasta'", method);

            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    _selectedFolderPath = dialog.SelectedPath;
                    txtFolderPath.Text = _selectedFolderPath;
                    _logger.LogInformation("{Method} - Pasta selecionada: {SelectedFolderPath}", method, _selectedFolderPath);
                }
            }
        }

        private void BtnCombineFiles_Click(object sender, RoutedEventArgs e)
        {
            const string method = $"{nameof(MainWindow)}.{nameof(BtnCombineFiles_Click)}";

            _logger.LogInformation("{Method} - Clique no botão 'Combinar Arquivos'", method);

            if (string.IsNullOrEmpty(_selectedFolderPath))
            {
                System.Windows.MessageBox.Show("Por favor, selecione a pasta do projeto primeiro.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                _logger.LogWarning("{Method} - Nenhuma pasta selecionada para combinar os arquivos.", method);
                return;
            }

            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Arquivos de Texto (*.txt)|*.txt|Todos os Arquivos (*.*)|*.*";
            saveFileDialog.Title = "Salvar Arquivo Combinado do Projeto";
            saveFileDialog.FileName = $"files-combined_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt";

            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var outputFilePath = saveFileDialog.FileName;
                var outputDirectory = Path.GetDirectoryName(outputFilePath)!;

                if (!Directory.Exists(outputDirectory))
                {
                    Directory.CreateDirectory(outputDirectory);
                    _logger.LogInformation("{Method} - Diretório de saída criado: {OutputDirectory}", method, outputDirectory);
                }

                var result = _combiner.CombineProjectFiles(_selectedFolderPath, outputFilePath);

                System.Windows.MessageBox.Show(result, "Resultado da Combinação", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}