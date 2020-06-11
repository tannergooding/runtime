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
    "Single",
    "Double"
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

    GenerateFilesForSupportedType(type, templatedText);
}

void TestVector()
{
    string shared = File.ReadAllText("Vector_All.template");

    for (var i = 2; i <= 4; i++)
    {
        var arifiedVector = "Vector" + i.ToString();

        var specific = File.ReadAllText(arifiedVector + ".template");

        var withShared = shared.Replace("{Specifics}", specific);

        foreach (var supportedType in SupportedTypes)
        {
            GenerateFilesForSupportedType(arifiedVector, withShared);
        }
    }
}

void GenerateFilesForSupportedType(string typename, string template)
{
    // Handle non-generic type first
    var nonGenericText = template.Replace("{TestType}", typename);
    WriteToTestFile(typename + ".cs", nonGenericText);

    foreach (var supportedType in SupportedTypes)
    {
        // this means we can blindly change everything, including method names (as < and > are not allowed in them)
        var alias = typename + supportedType;
        var usingAlias = $"using {alias} = {typename}<{supportedType}>;\n";
        var genericType = typename + '<' + supportedType + '>';
        var genericText = usingAlias + template.Replace("{TestType}", alias);

        WriteToTestFile(typename + "_" + supportedType  + ".cs", genericText);
    }
}

void WriteToTestFile(string file, string text)
{
    File.WriteAllText(file, text);
}

foreach (var type in TestTypes)
{
    TestSingleType(type);
}