// See https://aka.ms/new-console-template for more information

using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Channels;

async Task Serialize()
{
    var product = new Product() { Name = "Zeynel", Age = 21, Surname = "Sahin" };
    product.Category = new Category()
    {
        Id = 5, Name = "Electronic"
    };
    product.Categories = new List<Category>() { new Category() { Id = 1, Name = "Dress" } };
    FileStream createStream = File.Create("ProductAsync.json");
    string jsonString = JsonSerializer.Serialize(product);
    await JsonSerializer.SerializeAsync(createStream, product);

    Console.WriteLine(jsonString);

    string jsonStringGenerics = JsonSerializer.Serialize<Product>(product);
    Console.WriteLine(jsonStringGenerics);

    string fileName = "Product.json";
    File.WriteAllText(fileName, jsonString);
}

async Task Deserialize()
{
    string jsonString = File.ReadAllText("Product.json");

    var product = JsonSerializer.Deserialize<Product>(jsonString);
    Console.WriteLine(product.Name);
    FileStream openFileStream = File.OpenRead("Product.json");
    var product1 = await JsonSerializer.DeserializeAsync<Product>(openFileStream);
    Console.WriteLine(product1.Surname);
}

async Task HttpClientGetPost()
{
    HttpClient client = new()
    {
        BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
    };

    User? user = await client.GetFromJsonAsync<User>("users/2");
    Console.WriteLine(user.Name);

    await client.PostAsJsonAsync("/user", user);
}

void JsonSerializerOptions()
{
    var options = new JsonSerializerOptions { WriteIndented = true }; // json görüntüsü güzelleştirir
    User user1 = new()
    {
        Name = "Zeynel",
        UserName = "Şahin",
        Email = "zczxc@",
        Address = new Address()
        {
            City = "İstanbul"
        },
        Phone = "01010111001", Compay = new Compay()
        {
            Name = "adsa"
        }
    };

    string userJson = JsonSerializer.Serialize(user1, options);
    Console.WriteLine(userJson);

    userJson = JsonSerializer.Serialize(user1, new JsonSerializerOptions(JsonSerializerDefaults.Web));
    Console.WriteLine(userJson);

    userJson = JsonSerializer.Serialize(user1, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }); // büyük küçük duyasız hale getitrir
}

void CamelCase()
{
    var optionPropertyNaming = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase //key value tipinde ("":"",) olmayanları ve PropertyName attributetu bulunmayanları camel case çevirir
    };
}

void CustomNamePolicy()
{
    var optionsCustomPolicy = new JsonSerializerOptions()
    {
        PropertyNamingPolicy = new UpperCaseNamingPolicy()
    };
}

void OptionsDictionaryPolicy()
{
    var optionsDictKeyPolicy = new JsonSerializerOptions()
    {
        DictionaryKeyPolicy = new UpperCaseNamingPolicy(), //dictionary için name config
        WriteIndented = true //güzel gözükmesi
    };
}

void OptionEnums()
{
    var customer1 = new Customer()
    {
        CategoryId = CategoryId.Elektronik
    };

    var optionEnums = new JsonSerializerOptions()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
    };
}

void OptionIgnore()
{
    var deneme1 = new Deneme();

//1. Ignore [JsonIgnore] 
    string jsonString = JsonSerializer.Serialize(deneme1, new JsonSerializerOptions() { WriteIndented = true });
    Console.WriteLine(jsonString);
//2. Ignore read only prop
    jsonString = JsonSerializer.Serialize(deneme1, new JsonSerializerOptions() { IgnoreReadOnlyProperties = true });
//3. Ignore read only field
    jsonString = JsonSerializer.Serialize(deneme1, new JsonSerializerOptions() { IgnoreReadOnlyFields = true });
//4. Ignore null values
    jsonString = JsonSerializer.Serialize(deneme1, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
//5. Ignore default values
    jsonString = JsonSerializer.Serialize(deneme1, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault });
}

void OptionsEncoding()
{
    User user1 = new()
    {
        Name = "Zeynel",
        UserName = "Şahin",
        Email = "zczxc@",
        Address = new Address()
        {
            City = "İstanbul"
        },
        Phone = "01010111001", Compay = new Compay()
        {
            Name = "adsa"
        }
    };
    var jsonString = JsonSerializer.Serialize(user1, new JsonSerializerOptions()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    });
    Console.WriteLine(jsonString);
}

void JsonDocument()
{
    string jsonString = File.ReadAllText(@"Students.json");
    double sum = 0;
    int count = 0;

    StudentModel? studentModel;

    using (var document = System.Text.Json.JsonDocument.Parse(jsonString))
    {
        var root = document.RootElement;
        Console.WriteLine(root.ToString());
        Console.Clear();
        var ders = root.GetProperty("notlar");
        Console.WriteLine(ders.ToString());
        Console.Clear();
        foreach (var item in ders.EnumerateArray())
        {
            if (item.TryGetProperty("not", out var element))
            {
                sum += element.GetInt32();
                count++;
                studentModel = item.Deserialize<StudentModel>();
            }
        }
    }
}

// await Serialize();

// await Deserialize();

// await HttpClientGetPost();

// JsonSerializerOptions();

// CamelCase();

// CustomNamePolicy();

// OptionsDictionaryPolicy();

// OptionEnums();

// OptionIgnore();

// OptionsEncoding();

// JsonDocument();

var jsonString = File.ReadAllText("Students.json", Encoding.Default);
var options = new JsonSerializerOptions() { WriteIndented = true };

var jsonNode = JsonNode.Parse(jsonString);
Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine(jsonNode.ToJsonString(options));

Console.WriteLine(jsonNode.AsObject().Count());
var keys = jsonNode.AsObject().Select(p => p.Key).ToList();
Console.WriteLine(string.Join(", ", keys));

var deneme = jsonNode["deneme"];
Console.WriteLine($"{deneme.GetType()}");
Console.WriteLine($"{deneme.ToJsonString()}");

var notlar = jsonNode["notlar"];
Console.WriteLine($"{notlar.GetType()}");
Console.WriteLine($"{notlar.ToJsonString()}");

Console.WriteLine($"{notlar[0].GetType()}");
Console.WriteLine($"{notlar[0].ToJsonString()}");

var not = jsonNode["notlar"][0];
var asdas = jsonNode["asdas"].GetValue<int>();
jsonNode.AsObject().Remove("asdas");
jsonNode.AsObject().Add("wind",asdas );


var jsonObject = new JsonObject()
{
    ["deneme"] = DateTime.Now,
    ["list"] =new JsonObject()
    {
        ["ders"] = "asd",
        ["not"] = 50
    }
    ,
    ["array"]= new JsonArray(){"das","dasd","asd"}
};

Console.WriteLine(jsonObject.ToJsonString(new JsonSerializerOptions() { WriteIndented = true }));

Console.WriteLine(jsonObject["array"][0]);



class StudentModel
{
    public string Ders { get; set; }
    public int Not { get; set; }
}

//JsonPropertyName change
class Deneme
{
    [JsonPropertyName("Numara")]
    [JsonPropertyOrder(0)] // Serialize sırası
    public int Id { get; set; }

    [JsonPropertyOrder(-1)] public string Name { get; set; }
    [JsonPropertyOrder(-2)] public Category Category { get; set; }

    [JsonIgnore] // serialize edilmez
    public int Amount { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public int StockAmount { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public int? Count { get; set; }
}

//

class Customer
{
    public CategoryId CategoryId { get; set; }
}

internal enum CategoryId
{
    Elektronik,
    BeyazEsya
}

class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public Address Address { get; set; }
    public string Phone { get; set; }
    public string Website { get; set; }
    public Compay Compay { get; set; }
}

internal class Compay
{
    public string Name { get; set; }
    public string CatchPhrase { get; set; }
    public string Bs { get; set; }
}

internal class Address
{
    public string Street { get; set; }
    public string Suite { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public Geo Geo { get; set; }
}

internal class Geo
{
    public string Lat { get; set; }
    public string Lng { get; set; }
}

public class Product
{
    public string Name { get; set; }
    public string Surname { get; set; }
    public short Age { get; set; }

    public Category Category { get; set; }
    public List<Category> Categories { get; set; }
}

public class Category
{
    public string Name { get; set; }
    public int Id { get; set; }
}

public class UpperCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name) => name.ToUpper();
}