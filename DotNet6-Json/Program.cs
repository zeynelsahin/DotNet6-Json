// See https://aka.ms/new-console-template for more information

using System.Text.Json;

var product = new Product() { Name = "Zeynel", Age = 21, Surname = "Sahin" };
FileStream createStream = File.Create("ProductAsync.json");
string jsonString = JsonSerializer.Serialize(product);
await JsonSerializer.SerializeAsync(createStream, product);

Console.WriteLine(jsonString);

string jsonStringGenerics = JsonSerializer.Serialize<Product>(product);
Console.WriteLine(jsonStringGenerics);

string fileName = "Product.json";
File.WriteAllText(fileName, jsonString);

class Product
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public short Age { get; set; }
}