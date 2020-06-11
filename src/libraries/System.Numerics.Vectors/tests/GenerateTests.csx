private string[] TestTypes = new[]
{
    "Vector",
    "Matrix3x2",
    "Matrix4x4",
    "Quaternion",
    "Plane"
};

private string[] SupportedTypes = new[]
{
    "float",
    "double"
};

void TestSingleType(string type)
{
    if (type == "Vector")
    {
        TestVector();
        return;
    }

    var file = type + ".template";

    var templatedText = File.ReadAllText(file);

    GenerateFilesForSupportedType(type);
}

void TestVector()
{
    string shared = File.ReadAllText("Vector_All.template");

    for (var i = 1; i <= 3; i++)
    {
        var arifiedVector = "Vector" + i.ToString();

        var specific = File.ReadAllText(arifiedVector + ".template");

        var withShared = shared.Replace("{Specific}", specific);

        for (var supportedType in SupportedTypes)
        {
            GenerateFilesForSupportedType(arifiedVector, withShared);
        }
    }
}

void GenerateFilesForSupportedType(string typename, string template)
{
    // Handle non-generic type first
    var nonGenericText = templatedText.Replace("{TestType}", typename);
    File.WriteAllText(Path.Join("..", typename + ".cs"), nonGenericText);

    for (var supportedType in SupportedTypes)
    {
        var genericType = typename + '<' + supportedType '>';
        var genericText = templatedText.Replace("{TestType}", genericType);

        File.WriteAllText(Path.Join("..", typename + "_" + supportedType  + ".cs"), genericText);
    }
}

foreach (var type in TestTypes)
{
    TestSingleType(type);
}