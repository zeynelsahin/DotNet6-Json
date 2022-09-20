
using System.ComponentModel;
using System.Globalization;
using System.IO.Enumeration;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Text.Unicode;
using System.Threading.Channels;

async Task Serialize()
{
    var product = new Product() { Name = "Zeynel", Age = 21, Surname = "Sahin" };
    product.Category = new Category()
    {
        Id = 5, Name = "Electronic"
    };
    product.Categories = new List<Category>() { new() { Id = 1, Name = "Dress" } };
    var createStream = File.Create("ProductAsync.json");
    var jsonString = JsonSerializer.Serialize(product);
    await JsonSerializer.SerializeAsync(createStream, product);

    Console.WriteLine(jsonString);

    var jsonStringGenerics = JsonSerializer.Serialize<Product>(product);
    Console.WriteLine(jsonStringGenerics);

    var fileName = "Product.json";
    File.WriteAllText(fileName, jsonString);
}

async Task Deserialize()
{
    var jsonString = File.ReadAllText("Product.json");

    var product = JsonSerializer.Deserialize<Product>(jsonString);
    Console.WriteLine(product.Name);
    var openFileStream = File.OpenRead("Product.json");
    var product1 = await JsonSerializer.DeserializeAsync<Product>(openFileStream);
    Console.WriteLine(product1.Surname);
}

async Task HttpClientGetPost()
{
    HttpClient client = new()
    {
        BaseAddress = new Uri("https://jsonplaceholder.typicode.com")
    };

    var user = await client.GetFromJsonAsync<User>("users/2");
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

    var userJson = JsonSerializer.Serialize(user1, options);
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
    var jsonString = JsonSerializer.Serialize(deneme1, new JsonSerializerOptions() { WriteIndented = true });
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
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    });
    Console.WriteLine(jsonString);
}

void JsonDocument()
{
    var jsonString = File.ReadAllText(@"Students.json");
    double sum = 0;
    var count = 0;

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
            if (item.TryGetProperty("not", out var element))
            {
                sum += element.GetInt32();
                count++;
                studentModel = item.Deserialize<StudentModel>();
            }
    }
}

void JsonNode()
{
    var jsonString = File.ReadAllText("Students.json", Encoding.Default);
    var options = new JsonSerializerOptions() { WriteIndented = true };

    var jsonNode = System.Text.Json.Nodes.JsonNode.Parse(jsonString);
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
    jsonNode.AsObject().Add("wind", asdas);


    var jsonObject = new JsonObject()
    {
        ["deneme"] = DateTime.Now,
        ["list"] = new JsonObject()
        {
            ["ders"] = "asd",
            ["not"] = 50
        },
        ["array"] = new JsonArray() { "das", "dasd", "asd" }
    };

    Console.WriteLine(jsonObject.ToJsonString(new JsonSerializerOptions() { WriteIndented = true }));

    Console.WriteLine(jsonObject["array"][0]);
}

void EncodingWithWriter()
{
    var optionBuilder = new JsonWriterOptions()
    {
        Indented = true
    };

    using var stream = new MemoryStream();
    using var writer = new Utf8JsonWriter(stream, optionBuilder);
    writer.WriteStartObject();
    writer.WritePropertyName("id");
    writer.WriteStringValue("123");

    writer.WriteString("category", "teknoloji");
    writer.WriteNumber("asd", 112);

    writer.WriteStartObject("category");
    writer.WriteString("name", "deneme");
    writer.WriteNumber("id", 2);
    writer.WriteEndObject();

    writer.WriteStartArray("arrays");
    writer.WriteStringValue("deneme");
    writer.WriteStringValue("deneme");
    writer.WriteStringValue("deneme");
    writer.WriteEndArray();
    writer.WriteNull("adsad");

    writer.WriteStartArray("denemeArray");
    writer.WriteNullValue();
    writer.WriteEndArray();

    var array = @"[
{""id"":5,
""not"":10},
{""id"":5,
""not"":10},
{""id"":5,
""not"":10}
]";
    writer.WritePropertyName("türkçe");
    writer.WriteRawValue(array);
    writer.WriteEndObject();
    writer.Flush();

    var json = Encoding.UTF8.GetString(stream.ToArray());
    Console.OutputEncoding = Encoding.UTF8;
    Console.WriteLine(json);
}

void EncodingFile()
{
    var stringJson = File.ReadAllText("Product.json");
    using var fileStream = File.Create("OutProducts.json");
    using var writer = new Utf8JsonWriter(fileStream);
    using var document = System.Text.Json.JsonDocument.Parse(stringJson, new JsonDocumentOptions()
    {
        CommentHandling = JsonCommentHandling.Skip
    });

    var root = document.RootElement;

    if (root.ValueKind == JsonValueKind.Object)
        writer.WriteStartObject();
    else return;

    foreach (var jsonProperty in root.EnumerateObject()) jsonProperty.WriteTo(writer);

    writer.WriteEndObject();
    writer.Flush();
}

void EncodingUtfJsonReader()
{
    var utf8Bom = new byte[]
    {
        0xEF, 0xBB, 0xBF
    };

    ReadOnlySpan<byte> jsonByte = File.ReadAllBytes("Product.json");
    if (jsonByte.StartsWith(utf8Bom)) jsonByte = jsonByte.Slice(utf8Bom.Length);
    // var json = Encoding.UTF8.GetString(jsonByte);

    var reader = new Utf8JsonReader(jsonByte);
    Console.OutputEncoding = Encoding.UTF8;

    string text;
    while (reader.Read())
    {
        var tokenType = reader.TokenType;
        Console.WriteLine(tokenType.ToString());

        if (tokenType == JsonTokenType.String)
        {
            text = reader.GetString();

            Console.WriteLine($" {text}");
        }
    }
}

void JsonException()
{
    var jsonString = File.ReadAllText("Product.json");
    try
    {
        var product = JsonSerializer.Deserialize<Product>(jsonString);
    }
    catch (JsonException exception)
    {
        Console.WriteLine(exception.Message);
        var zeynelJsonException = new ZeynelJsonException("Failed", exception);
        throw zeynelJsonException;
    }
}

void CommandLine()
{
    var options = new JsonSerializerOptions()
    {
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString
    };

    var jsonString = File.ReadAllText("Product.json");
    var product = JsonSerializer.Deserialize<Product>(jsonString, options);
    if (product != null) Console.WriteLine($"Date: {product.Date}");
}

void ExpressionData()
{
    var jsonString = File.ReadAllText("CustomerTest.json");

    var customerTest = JsonSerializer.Deserialize<CustomerTest>(jsonString, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });

    if (customerTest != null)
        foreach (var item in customerTest.ExtensionData)
            Console.WriteLine($"{item.Key} : {item.Value}");
}

void Collection()
{
    var test = new Test()
    {
        CategoryId = "Test"
    };

    var test1 = new Test()
    {
        CategoryId = "Test1"
    };

    test.Tests = new List<Test>();
    test.Tests.Add(test1);
    test.Tests.Add(test);


    var options = new JsonSerializerOptions()
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.Preserve
    };

    var jsonString = JsonSerializer.Serialize(test, options);
    Console.WriteLine(jsonString);

    var options1 = new JsonSerializerOptions()
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };
    jsonString = JsonSerializer.Serialize(test, options1);
    Console.WriteLine(jsonString);
}

void FileCreateWithJsonWriter()
{
    using var fileStream = File.Create("NonPublicSet.json");
    var writer = new Utf8JsonWriter(fileStream, new JsonWriterOptions() { Indented = true });
    writer.WriteStartObject();
    writer.WriteString("date", "2020-09-06T11:31:01.923395-07:00");
    writer.WriteNumber("id", -1);
    writer.WriteString("total", "cold");
    writer.WriteEndObject();
    writer.Flush();
}

void NonPublicSetCtor()
{
    var jsonString = File.ReadAllText("NonPublicSet.json");
    Console.WriteLine($"Input JSON:\r\n{jsonString}");
    var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    var nonPublicSet = JsonSerializer.Deserialize<NonPublicSetJsonInclude>(jsonString, options);
    Console.WriteLine(nonPublicSet.Date);
    Console.WriteLine(nonPublicSet.Id);
    Console.WriteLine(nonPublicSet.Total);
}

void NonPublicSetJsonInclude()
{
    var jsonString = File.ReadAllText("NonPublicSet.json");
    Console.WriteLine($"Input JSON: \r\n{jsonString}");
    var nonPublicSet = JsonSerializer.Deserialize<NonPublicSetJsonInclude>(jsonString);
    Console.WriteLine($"Date: {nonPublicSet.Date}");
    Console.WriteLine($"Date: {nonPublicSet.Id}");

    jsonString = JsonSerializer.Serialize<NonPublicSetJsonInclude>(nonPublicSet);
    Console.WriteLine($"Output JSON: {jsonString}");
}

void PolymorphicSerilization()
{
    var seismicForecast = new WeatherForecastSeismic()
    {
        Date = DateTime.Parse("1999-10-01"),
        TemperatureCelsius = 30,
        Summary = "Hot",
        TimeToNextEarthquake = DateTime.Parse("14:40:15"),
        MagnitudeOfNextEarthquake = 4.5
    };

    var options = new JsonSerializerOptions()
    {
        WriteIndented = true
    };
    //Type belirtmek zorunlu
    var jsonString = JsonSerializer.Serialize(seismicForecast, seismicForecast.GetType(), options);

    Console.WriteLine(jsonString);
}

void BasicConverter()
{
    var weatherForecast = new WeatherForecastWithBasicConverter()
    {
        Date = DateTime.Now,
        TemperatureCelsius = 2,
        Summary = "winter"
    };
    var jsonString = JsonSerializer.Serialize(weatherForecast, new JsonSerializerOptions() { WriteIndented = true });
    Console.WriteLine(jsonString);

    var serializeOptions = new JsonSerializerOptions()
    {
        WriteIndented = true,
        Converters = { new DateTimeOffsetJsonConverter() }
    };
    jsonString = JsonSerializer.Serialize(weatherForecast, serializeOptions);
    Console.WriteLine(jsonString);
}

void FactoryConverter()
{
    var currentWeather = new Dictionary<Feels, string>();
    currentWeather.Add(Feels.Cold, "I am from Costa Rice, this guy is freezing!");
    currentWeather.Add(Feels.Cool, "I am from England, this is ok!");
    currentWeather.Add(Feels.Warm, "I am from Canada,this is what we call a Tuesday");
    currentWeather.Add(Feels.Hot, "I am from Turkey, this guy is summer!");
    var jsonFeels = JsonSerializer.Serialize(currentWeather, new JsonSerializerOptions() { WriteIndented = true });
    Console.WriteLine(jsonFeels);
    var options = new JsonSerializerOptions()
    {
        WriteIndented = true,
        Converters = { new DictionaryTKeyEnumValueConverter() }
    };
    jsonFeels = JsonSerializer.Serialize(currentWeather, options);
    Console.WriteLine(jsonFeels);
}
//
// void JsonSourceGeneration()
// {
//     string jsonString = @"{""Date"": ""2022-01-01T00:00:00""
// ""TemperatureCelsius"": 20,
// ""Summary"":""Hot""";
//
//     WeatherForecast? weatherForecast;
//     weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(jsonString, SourceGenerationContext.Default.WeatherForecast);
//     Console.WriteLine($"Date= {weatherForecast?.Date}");
//
//     weatherForecast = JsonSerializer.Deserialize<WeatherForecast>(jsonString, SourceGenerationContext.Default) as WeatherForecast;
//     Console.WriteLine($"Date= {weatherForecast?.Date}");
//
//     weatherForecast = JsonSerializer.Serialize(jsonString, typeof(WeatherForecast), SourceGenerationContext.Default.WeatherForecast);
//     Console.WriteLine(jsonString);
//
//     weatherForecast = JsonSerializer.Serialize(jsonString, typeof(WeatherForecast), SourceGenerationContext.Default);
//     Console.WriteLine(jsonString);
// }


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

// JsonNode();

// EncodingWithWriter();

// EncodingFile();

// EncodingUtfJsonReader();

// JsonException();

// CommandLine();

// ExpressionData();

// Collection();

// FileCreateWithJsonWriter();

// NonPublicSetCtor();

// NonPublicSetJsonInclude();

// PolymorphicSerialize();

// BasicConverter();

// FactoryConverter();

// JsonSourceGeneration();

[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(WeatherForecast))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
    public SourceGenerationContext(JsonSerializerOptions? options) : base(options)
    {
    }

    public override JsonTypeInfo? GetTypeInfo(Type type)
    {
        return null;
    }

    protected override JsonSerializerOptions? GeneratedSerializerOptions { get; }
}

internal class DictionaryTKeyEnumValueConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        if (!typeToConvert.IsGenericType) return false;
        if (typeToConvert.GetGenericTypeDefinition() != typeof(Dictionary<,>)) return true;
        return typeToConvert.GetGenericArguments()[0].IsEnum;
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var keyType = typeToConvert.GetGenericArguments()[0];
        var valueType = typeToConvert.GetGenericArguments()[1];
        var converter = (JsonConverter)Activator.CreateInstance(typeof(DictionaryEnumConverterInner<,>).MakeGenericType(
            new Type[] { keyType, valueType }), BindingFlags.Instance | BindingFlags.Public, null, new object[] { options }, null);
        return converter;
    }
}

internal class DictionaryEnumConverterInner<TKey, TValue> : JsonConverter<Dictionary<TKey, TValue>> where TKey : struct, Enum
{
    private readonly JsonConverter<TValue> _valueConverter;
    private readonly Type _keyType;
    private readonly Type _valueType;

    public DictionaryEnumConverterInner(JsonSerializerOptions options)
    {
        //For performance
        _valueConverter = (JsonConverter<TValue>)options.GetConverter(typeof(TValue));
        _keyType = typeof(TKey);
        _valueType = typeof(TValue);
    }

    public override Dictionary<TKey, TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

        var dictionary = new Dictionary<TKey, TValue>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject) return dictionary;

            if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException();

            var propertyName = reader.GetString();
            if (!Enum.TryParse(propertyName, false, out TKey key) && !Enum.TryParse(propertyName, true, out key))
                throw new JsonException($"Unable to convert \"{propertyName}\" to Enum \"{_keyType}\".");

            TValue value;
            if (_valueConverter != null)
            {
                reader.Read();
                value = _valueConverter.Read(ref reader, _valueType, options);
            }
            else
            {
                value = JsonSerializer.Deserialize<TValue>(ref reader, options);
            }

            dictionary.Add(key, value);
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Dictionary<TKey, TValue> dictionary, JsonSerializerOptions options)
    {
        string ModifySentence(string original)
        {
            try
            {
                var words = original.Split(",");
                var place = words[0].Replace("I am from ", "");
                var howFeels = words[1].Replace(",", " ");
                return place + ":" + howFeels;
            }
            catch
            {
                throw new JsonException("Sentence not in the expected format");
            }
        }

        writer.WriteStartObject();
        foreach ((var key, var value) in dictionary)
        {
            var propertyName = key.ToString();
            writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(propertyName) ?? propertyName);
            if (_valueConverter != null)
            {
                if (value.GetType() == typeof(string))
                {
                    var newT = (TValue)(object)ModifySentence(value.ToString());
                    _valueConverter.Write(writer, newT, options);
                }
            }
            else
            {
                JsonSerializer.Serialize(writer, value, options);
            }
        }

        writer.WriteEndObject();
    }
}

internal class DateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTimeOffset.ParseExact(reader.GetString(), "MM/dd/yyyy", CultureInfo.InvariantCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture));
    }
}

internal enum Feels
{
    Cold,
    Warm,
    Hot,
    Cool
}

internal class WeatherForecastWithBasicConverter
{
    // [JsonConverter((typeof(DateTimeOffsetConverter)))]
    public DateTimeOffset Date { get; set; }

    public int TemperatureCelsius { get; set; }
    public string Summary { get; set; }
}

internal class WeatherForecast
{
    public DateTime Date { get; set; }
    public int TemperatureCelsius { get; set; }
    public string Summary { get; set; }
}

internal class WeatherForecastSeismic : WeatherForecast
{
    public DateTime TimeToNextEarthquake { get; set; }
    public double MagnitudeOfNextEarthquake { get; set; }
}

internal class NonPublicSetJsonInclude
{
    public string Date { get; }
    [JsonInclude] public int Id { get; }
    [JsonInclude] public string Total { get; }
}

internal class NonPublicSetCtor
{
    [JsonConstructor]
    public NonPublicSetCtor(string date, int id, string total)
    {
        Date = date;
        Id = id;
        Total = total;
    }

    public string Date { get; }
    public int Id { get; }
    public string Total { get; }
}

internal class StudentModel
{
    public string Ders { get; set; }
    public int Not { get; set; }
}

//JsonPropertyName change
internal class Deneme
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
internal class CustomerTest
{
    public string CategoryId { get; set; }
    [JsonExtensionData] public Dictionary<string, JsonElement> ExtensionData { get; set; }
}

internal class Test
{
    public string CategoryId { get; set; }
    public ICollection<Test>? Tests { get; set; }
}

internal class Customer
{
    public CategoryId CategoryId { get; set; }
}

internal enum CategoryId
{
    Elektronik,
    BeyazEsya
}

internal class User
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
    public DateTime Date { get; set; }
}

public class Category
{
    public string Name { get; set; }
    public int Id { get; set; }
}

public class UpperCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return name.ToUpper();
    }
}

public class ZeynelJsonException : JsonException
{
    public ZeynelJsonException()
    {
    }

    public ZeynelJsonException(string message) : base(message)
    {
    }

    public ZeynelJsonException(string message, Exception exception) : base(message, exception)
    {
    }
}