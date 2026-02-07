var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

var jogadores = new List<Jogador>
{
    new Jogador("Roosevelt", 1500, true),
    new Jogador("Gemini AI", 2000, true)
};

// Rota para BUSCAR os jogadores
app.MapGet("/jogadores", () => Results.Ok(jogadores));

// Rota para ADICIONAR um novo jogador
app.MapPost("/jogadores", (Jogador novoJogador) => 
{
    jogadores.Add(novoJogador);
    return Results.Created($"/jogadores/{novoJogador.Nome}", novoJogador);
});

app.Run();

record Jogador(string Nome, int Pontuacao, bool Ativo);