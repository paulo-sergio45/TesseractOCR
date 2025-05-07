using System;
using System.IO;
using Tesseract;

class Program
{
    static void Main(string[] args)
    {
        string basePath = Environment.CurrentDirectory;

        string tessdataPath = Path.Combine(basePath, "tessdata");
        string imagensPath = Path.Combine(basePath, "imagens");
        string fileTrainedData = Path.Combine(tessdataPath, "por.traineddata");
        string outputPath = Path.Combine(basePath, "resultado.txt");
        string idioma = "por";


        if (!File.Exists(fileTrainedData) || !Directory.Exists(imagensPath))
        {
            Console.WriteLine("Arquivos traineddata ou Imagem  não encontrados.");
            Environment.Exit(0);
        }

        var arquivosImagem = Directory.GetFiles(imagensPath, "*.*")
               .Where(f => f.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                        || f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                        || f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
               .ToList();

        if (arquivosImagem.Count == 0)
        {
            Console.WriteLine($"Nenhum arquivo de imagem encontrado na pasta: {imagensPath}");
            Environment.Exit(0);
        }

        try
        {

            using (var engine = new TesseractEngine(tessdataPath, idioma, EngineMode.Default))
            using (var writer = new StreamWriter(outputPath, false))
            {

                foreach (var caminhoImagem in arquivosImagem)
                {
                    string nomeArquivo = Path.GetFileNameWithoutExtension(caminhoImagem);
                    string saidaTexto = Path.Combine(outputPath, nomeArquivo + ".txt");

                    byte[] imagemBytes = File.ReadAllBytes(caminhoImagem);
                    using (var img = Pix.LoadFromMemory(imagemBytes))
                    using (var page = engine.Process(img))
                    {
                        string texto = page.GetText();
                        writer.WriteLine(texto);
                        writer.WriteLine();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Erro ao processar imagem: " + ex.Message);
        }


    }
}
