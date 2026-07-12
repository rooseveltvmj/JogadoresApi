using System.Text.Json;
using System.Linq;  // ← ADICIONADO PARA USAR LINQ

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

// Rota GET: Retorna a lista de jogadores ordenada por pontuação
app.MapGet("/jogadores", () =>
{
    var lista = ObterJogadores();
    var ordenada = lista.OrderByDescending(j => j.Pontuacao).ToList();
    return Results.Ok(ordenada);
});

// Rota GET: Busca jogador pelo nome
app.MapGet("/jogadores/{nome}", (string nome) =>
{
    var lista = ObterJogadores();
    var jogador = lista.FirstOrDefault(j => j.Nome == nome);

    if (jogador == null)
        return Results.NotFound("Jogador não encontrado");

    return Results.Ok(jogador);
});

app.MapPut("/jogadores/{nome}", (string nome, Jogador jogadorAtualizado) =>
{
    var lista = ObterJogadores();
    var jogador = lista.FirstOrDefault(j => j.Nome == nome);

    if (jogador == null)
        return Results.NotFound("Jogador não encontrado");

    // Remove o antigo e adiciona o atualizado
    lista.Remove(jogador);
    lista.Add(jogadorAtualizado);

    var json = JsonSerializer.Serialize(lista);
    File.WriteAllText(caminhoArquivo, json);

    return Results.Ok(jogadorAtualizado);
});

// Rota POST: Salva no arquivo após adicionar
// Rota POST: Salva no arquivo após adicionar (com validações abaixo)

app.MapPost("/jogadores", (Jogador novoJogador) =>
{
    // Validações
    if (string.IsNullOrWhiteSpace(novoJogador.Nome))
        return Results.BadRequest("Nome não pode estar vazio");

    if (novoJogador.Pontuacao < 0)
        return Results.BadRequest("Pontuação não pode ser negativa");

    var lista = ObterJogadores();

    if (lista.Any(j => j.Nome == novoJogador.Nome))
        return Results.BadRequest("Jogador com este nome já existe");

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

app.MapDelete("/jogadores/{nome}", (string nome) =>
{
    var lista = ObterJogadores();
    var jogador = lista.FirstOrDefault(j => j.Nome == nome);

    if (jogador == null)
        return Results.NotFound("Jogador não encontrado");

    lista.Remove(jogador);

    var json = JsonSerializer.Serialize(lista);
    File.WriteAllText(caminhoArquivo, json);

    return Results.NoContent();
});

app.Run();

record Jogador(string Nome, int Pontuacao, bool Ativo);