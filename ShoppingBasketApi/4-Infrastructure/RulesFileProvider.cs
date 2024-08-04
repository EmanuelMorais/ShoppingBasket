namespace ShoppingBasketApi.Infrastructure;

using System.IO;
using System.Threading.Tasks;
using ShoppingBasketApi.Domain.Abstractions;

public class RulesFileProvider : IRulesFileProvider
{
    private readonly string _filePath;

    public RulesFileProvider(string filePath)
    {
        _filePath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, filePath));
    }

    public async Task<string> GetRulesJsonAsync()
    {
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException($"Rules file not found at path: {_filePath}");
        }

        return await File.ReadAllTextAsync(_filePath);
    }
}

