// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

private string[] TestTypes = new[]
{
    "Vector",
    "Matrix3x2",
    "Matrix4x4",
    "Quaternion",
    "Plane"
};

private string[][] SupportedTypes = new[]
{
    new[] { "Single", "f" },
    new[] { "Double", "d" }
};

void TestSingleType(string type)
{
    if (type == "Vector")
    {
        TestVector();
        return;
    }

    var file = type + ".template";

    var templatedText = ReadFile(file);

    GenerateFilesForSupportedType(type, templatedText);
}

void TestVector()
{
    string shared = ReadFile("Vector_All.template");

    for (var i = 2; i <= 4; i++)
    {
        var arifiedVector = "Vector" + i.ToString();

        var specific = ReadFile(arifiedVector + ".template").Split('\n');
        var correct = new StringBuilder(specific.Length);

        // these add indenting to lines which need it
        Console.WriteLine(specific.Length);
        foreach (var line in specific)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                correct.AppendLine("        " + line);
            }
            else
            {
                correct.AppendLine(line);
            }
        }


        var withShared = shared.Replace("{Specifics}", correct.ToString());
        GenerateFilesForSupportedType(arifiedVector, withShared);
    }
}

void GenerateFilesForSupportedType(string typename, string template)
{
    // Handle non-generic type first
    var nonGenericText = template.Replace("{TestType}", typename);
    nonGenericText = nonGenericText.Replace("{ScalarType}", "Single");
    nonGenericText = nonGenericText.Replace("{ScalarSuffix}", "f");
    nonGenericText = nonGenericText.Replace("{AssignX}", ".X = "); // non generic doesn't have With methods

    nonGenericText = nonGenericText.Replace("{GenericSpecific}", "");

    WriteToTestFile(typename, nonGenericText);

    var genericFile = typename + "_Generic.template";

    var genericTemplate = template.Replace("{GenericSpecific}", File.Exists(genericFile) ? ReadFile(genericFile) : "");

    foreach (var supportedType in SupportedTypes)
    {
        // this means we can blindly change everything, including method names (as < and > are not allowed in them)
        // namespace as we put this riiight at the start of the file, before the using
        var alias = typename + supportedType[0];
        var usingAlias = $"using {alias} = System.Numerics.{typename}<System.{supportedType[0]}>;{Environment.NewLine}";
        var genericType = typename + '<' + supportedType[0] + '>';

        var genericText = usingAlias + genericTemplate.Replace("{TestType}", alias);

        genericText = genericText.Replace("{ScalarType}", supportedType[0]);
        genericText = genericText.Replace("{ScalarSuffix}", supportedType[1]);
        genericText = genericText.Replace("{AssignX}", " = b.WithX"); // generic uses with methods

        WriteToTestFile(typename + "_" + supportedType[0], genericText);
    }
}

string ReadFile(string name)
{
    string license = "{EndLicense}";
    string text = File.ReadAllText(name);
    var ind = text.IndexOf(license);
    if (ind != -1)
    {
        text = text.Substring(ind + license.Length);
    }
    return text;
}

void WriteToTestFile(string file, string text)
{
    File.WriteAllText(Path.Combine("Test", file + "Tests.cs"), text);
}

foreach (var type in TestTypes)
{
    TestSingleType(type);
}