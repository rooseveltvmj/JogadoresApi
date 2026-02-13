using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

// Nome do arquivo onde vamos salvar os dados
string caminhoArquivo = "ranking.json";

// Função para carregar os jogadores do arquivo
List<Jogador> ObterJogadores()
{
    if (!File.Exists(caminhoArquivo)) return new List<Jogador>();
    var json = File.ReadAllText(caminhoArquivo);
    return JsonSerializer.Deserialize<List<Jogador>>(json) ?? new List<Jogador>();
}

// Rota GET: Agora lê do arquivo
app.MapGet("/jogadores", () => 
{
    var lista = ObterJogadores();
    return Results.Ok(lista);
});

// Rota POST: Salva no arquivo após adicionar
app.MapPost("/jogadores", (Jogador novoJogador) => 
{
    var lista = ObterJogadores();
    lista.Add(novoJogador);
    
    var json = JsonSerializer.Serialize(lista);
    File.WriteAllText(caminhoArquivo, json);
    
    return Results.Created($"/jogadores/{novoJogador.Nome}", novoJogador);
});

// Rota para APAGAR todos os jogadores e limpar o arquivo
app.MapDelete("/jogadores/reiniciar", () => 
{
    if (File.Exists(caminhoArquivo))
    {
        File.Delete(caminhoArquivo); // Apaga o arquivo físico
    }
    return Results.NoContent(); // Retorna um aviso de que deu certo, mas não há mais conteúdo
});

app.Run();

record Jogador(string Nome, int Pontuacao, bool Ativo);