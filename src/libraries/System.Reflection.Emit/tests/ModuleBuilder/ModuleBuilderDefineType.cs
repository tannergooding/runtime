// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;
using Xunit;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace System.Reflection.Emit.Tests
{
    [RequiresUnreferencedCode("Uses reflection to construct test cases")]
    public class ModuleBuilderDefineType
    {
        public static IEnumerable<object[]> TestData()
        {
            foreach (string name in new string[] { "TestName", "testname", "class", "\uD800\uDC00", "432" })
            {
                foreach (TypeAttributes attributes in new TypeAttributes[] { TypeAttributes.NotPublic, TypeAttributes.Interface | TypeAttributes.Abstract, TypeAttributes.Class })
                {
                    foreach (Type parent in new Type[] { null, typeof(ModuleBuilderDefineType) })
                    {
                        foreach (PackingSize packingSize in new PackingSize[] { PackingSize.Unspecified, PackingSize.Size1 })
                        {
                            foreach (int size in new int[] { 0, -1, 1 })
                            {
                                yield return new object[] { name, attributes, parent, packingSize, size, new Type[0] };
                            }
                        }

                        yield return new object[] { name, attributes, parent, PackingSize.Unspecified, 0, null };
                        yield return new object[] { name, attributes, parent, PackingSize.Unspecified, 0, new Type[] { typeof(IComparable) } };
                    }
                }
            }
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void DefineType(string name, TypeAttributes attributes, Type parent, PackingSize packingSize, int typesize, Type[] implementedInterfaces)
        {
            bool isDefaultImplementedInterfaces = implementedInterfaces?.Length == 0;
            bool isDefaultPackingSize = packingSize == PackingSize.Unspecified;
            bool isDefaultSize = typesize == 0;
            bool isDefaultParent = parent == null;
            bool isDefaultAttributes = attributes == TypeAttributes.NotPublic;

            void Verify(TypeBuilder type, Module module)
            {
                Type baseType = attributes.HasFlag(TypeAttributes.Abstract) && parent == null ? null : (parent ?? typeof(object));
                Helpers.VerifyType(type, module, null, name, attributes, baseType, typesize, packingSize, implementedInterfaces);
            }

            if (isDefaultImplementedInterfaces)
            {
                if (isDefaultSize && isDefaultPackingSize)
                {
                    if (isDefaultParent)
                    {
                        if (isDefaultAttributes)
                        {
                            // Use DefineType(string)
                            ModuleBuilder module1 = Helpers.DynamicModule();
                            Verify(module1.DefineType(name), module1);
                        }
                        // Use DefineType(string, TypeAttributes)
                        ModuleBuilder module2 = Helpers.DynamicModule();
                        Verify(module2.DefineType(name, attributes), module2);
                    }
                    // Use DefineType(string, TypeAttributes, Type)
                    ModuleBuilder module3 = Helpers.DynamicModule();
                    Verify(module3.DefineType(name, attributes, parent), module3);
                }
                else if (isDefaultSize)
                {
                    // Use DefineType(string, TypeAttributes, Type, PackingSize)
                    ModuleBuilder module4 = Helpers.DynamicModule();
                    Verify(module4.DefineType(name, attributes, parent, packingSize), module4);
                }
                else if (isDefaultPackingSize)
                {
                    // Use DefineType(string, TypeAttributes, Type, int)
                    ModuleBuilder module5 = Helpers.DynamicModule();
                    Verify(module5.DefineType(name, attributes, parent, typesize), module5);
                }
                // Use DefineType(string, TypeAttributes, Type, PackingSize, int)
                ModuleBuilder module6 = Helpers.DynamicModule();
                Verify(module6.DefineType(name, attributes, parent, packingSize, typesize), module6);
            }
            else
            {
                // Use DefineType(string, TypeAttributes, Type, Type[])
                Assert.True(isDefaultSize && isDefaultPackingSize); // Sanity check
                ModuleBuilder module7 = Helpers.DynamicModule();
                Verify(module7.DefineType(name, attributes, parent, implementedInterfaces), module7);
            }
        }

        [ConditionalTheory(typeof(PlatformDetection), nameof(PlatformDetection.HasAssemblyFiles))]
        [MemberData(nameof(TestData))]
        public void DefineTypePersistedAssembly(string name, TypeAttributes attributes, Type parent, PackingSize packingSize, int typesize, Type[] implementedInterfaces)
        {
            bool isDefaultImplementedInterfaces = implementedInterfaces?.Length == 0;
            bool isDefaultPackingSize = packingSize == PackingSize.Unspecified;
            bool isDefaultSize = typesize == 0;
            bool isDefaultParent = parent == null;
            bool isDefaultAttributes = attributes == TypeAttributes.NotPublic;
            Type baseType = attributes.HasFlag(TypeAttributes.Abstract) && parent == null ? null : (parent ?? typeof(object));

            void VerifyTypeBuilder(TypeBuilder type, Module module)
            {
                Assert.Equal(module, type.Module);
                Assert.Equal(module.Assembly, type.Assembly);
                Assert.Equal(name, type.Name);
                Assert.Equal(Helpers.GetFullName(name), type.FullName);
                Assert.Equal(attributes, type.Attributes);
                Assert.Equal(baseType, type.BaseType);
                Assert.Equal(typesize, type.Size);
                Assert.Equal(packingSize, type.PackingSize);
                Assert.Equal(implementedInterfaces ?? new Type[0], type.GetInterfaces());
            }

            void VerifyType(Type createdType)
            {
                Assert.Equal(name, createdType.Name);
                Assert.Equal(attributes, createdType.Attributes);
                Assert.Equal(Helpers.GetFullName(name), createdType.FullName);
                Assert.Equal(attributes, createdType.Attributes);
                if (baseType != null)
                {
                    Assert.Equal(baseType.Name, createdType.BaseType.Name);
                }
                Assert.Equal((implementedInterfaces ?? new Type[0]).Length, createdType.GetInterfaces().Length);
            }

            PersistedAssemblyBuilder ab;
            TypeBuilder typeBuilder;
            if (isDefaultImplementedInterfaces)
            {
                if (isDefaultSize && isDefaultPackingSize)
                {
                    if (isDefaultParent)
                    {
                        if (isDefaultAttributes)
                        {
                            // Use DefineType(string)
                            ab = AssemblySaveTools.PopulateAssemblyAndModule(out ModuleBuilder module1);
                            typeBuilder = module1.DefineType(name);
                            typeBuilder.CreateType();
                            VerifyTypeBuilder(typeBuilder, module1);

                            using (var stream = new MemoryStream())
                            using (MetadataLoadContext mlc = new MetadataLoadContext(new CoreMetadataAssemblyResolver()))
                            {
                                ab.Save(stream);
                                VerifyType(mlc.LoadFromStream(stream).GetType(name));
                            }
                        }
                        // Use DefineType(string, TypeAttributes)
                        ab = AssemblySaveTools.PopulateAssemblyAndModule(out ModuleBuilder module2);
                        typeBuilder = module2.DefineType(name, attributes);
                        typeBuilder.CreateType();
                        VerifyTypeBuilder(typeBuilder, module2);

                        using (var stream = new MemoryStream())
                        using (MetadataLoadContext mlc = new MetadataLoadContext(new CoreMetadataAssemblyResolver()))
                        {
                            ab.Save(stream);
                            VerifyType(mlc.LoadFromStream(stream).GetType(name));
                        }
                    }
                    // Use DefineType(string, TypeAttributes, Type)
                    ab = AssemblySaveTools.PopulateAssemblyAndModule(out ModuleBuilder module3);
                    typeBuilder = module3.DefineType(name, attributes, parent);
                    typeBuilder.CreateType();
                    VerifyTypeBuilder(typeBuilder, module3);

                    using (var stream = new MemoryStream())
                    using (MetadataLoadContext mlc = new MetadataLoadContext(new CoreMetadataAssemblyResolver()))
                    {
                        ab.Save(stream);
                        VerifyType(mlc.LoadFromStream(stream).GetType(name));
                    }
                }
                else if (isDefaultSize)
                {
                    // Use DefineType(string, TypeAttributes, Type, PackingSize)
                    ab = AssemblySaveTools.PopulateAssemblyAndModule(out ModuleBuilder module4);
                    typeBuilder = module4.DefineType(name, attributes, parent, packingSize);
                    typeBuilder.CreateType();
                    VerifyTypeBuilder(typeBuilder, module4);

                    using (var stream = new MemoryStream())
                    using (MetadataLoadContext mlc = new MetadataLoadContext(new CoreMetadataAssemblyResolver()))
                    {
                        ab.Save(stream);
                        VerifyType(mlc.LoadFromStream(stream).GetType(name));
                    }
                }
                else if (isDefaultPackingSize)
                {
                    // Use DefineType(string, TypeAttributes, Type, int)
                    ab = AssemblySaveTools.PopulateAssemblyAndModule(out ModuleBuilder module5);
                    typeBuilder = module5.DefineType(name, attributes, parent, typesize);
                    typeBuilder.CreateType();
                    VerifyTypeBuilder(typeBuilder, module5);

                    using (var stream = new MemoryStream())
                    using (MetadataLoadContext mlc = new MetadataLoadContext(new CoreMetadataAssemblyResolver()))
                    {
                        ab.Save(stream);
                        VerifyType(mlc.LoadFromStream(stream).GetType(name));
                    }
                }
                // Use DefineType(string, TypeAttributes, Type, PackingSize, int)
                ab = AssemblySaveTools.PopulateAssemblyAndModule(out ModuleBuilder module6);
                typeBuilder = module6.DefineType(name, attributes, parent, packingSize, typesize);
                typeBuilder.CreateType();
                VerifyTypeBuilder(typeBuilder, module6);

                using (var stream = new MemoryStream())
                using (MetadataLoadContext mlc = new MetadataLoadContext(new CoreMetadataAssemblyResolver()))
                {
                    ab.Save(stream);
                    VerifyType(mlc.LoadFromStream(stream).GetType(name));
                }
            }
            else
            {
                // Use DefineType(string, TypeAttributes, Type, Type[])
                Assert.True(isDefaultSize && isDefaultPackingSize); // Sanity check
                ab = AssemblySaveTools.PopulateAssemblyAndModule(out ModuleBuilder module7);
                typeBuilder = module7.DefineType(name, attributes, parent, implementedInterfaces);
                typeBuilder.CreateType();
                VerifyTypeBuilder(typeBuilder, module7);

                using (var stream = new MemoryStream())
                using (MetadataLoadContext mlc = new MetadataLoadContext(new CoreMetadataAssemblyResolver()))
                {
                    ab.Save(stream);
                    VerifyType(mlc.LoadFromStream(stream).GetType(name));
                }
            }
        }

        [Fact]
        public void DefineType_String_TypeAttributes_Type_TypeCreatedInModule()
        {
            ModuleBuilder module = Helpers.DynamicModule();
            TypeBuilder type1 = module.DefineType("TestType1");
            Type parent = type1.CreateType();

            TypeBuilder type2 = module.DefineType("TestType2", TypeAttributes.NotPublic, parent);
            Type createdType = type2.CreateType();
            Assert.Equal("TestType2", createdType.Name);
            Assert.Equal(TypeAttributes.NotPublic, createdType.GetTypeInfo().Attributes);
            Assert.Equal(parent, createdType.GetTypeInfo().BaseType);
        }

        [Fact]
        public void DefineType_NullName_ThrowsArgumentNullException()
        {
            ModuleBuilder module = Helpers.DynamicModule();
            AssertExtensions.Throws<ArgumentNullException>("name", () => module.DefineType(null));
            AssertExtensions.Throws<ArgumentNullException>("name", () => module.DefineType(null, TypeAttributes.NotPublic));
            AssertExtensions.Throws<ArgumentNullException>("name", () => module.DefineType(null, TypeAttributes.NotPublic, typeof(ModuleBuilderDefineType)));

            AssertExtensions.Throws<ArgumentNullException>("name", () => module.DefineType(null, TypeAttributes.NotPublic, typeof(ModuleBuilderDefineType), PackingSize.Unspecified));
            AssertExtensions.Throws<ArgumentNullException>("name", () => module.DefineType(null, TypeAttributes.NotPublic, typeof(ModuleBuilderDefineType), 0));
            AssertExtensions.Throws<ArgumentNullException>("name", () => module.DefineType(null, TypeAttributes.NotPublic, typeof(ModuleBuilderDefineType), PackingSize.Unspecified, 0));

            AssertExtensions.Throws<ArgumentNullException>("name", () => module.DefineType(null, TypeAttributes.NotPublic, typeof(ModuleBuilderDefineType), new Type[0]));
        }

        [Fact]
        public void DefineType_TypeAlreadyExists_ThrowsArgumentException()
        {
            ModuleBuilder module = Helpers.DynamicModule();
            module.DefineType("TestType");
            AssertExtensions.Throws<ArgumentException>(null, () => module.DefineType("TestType"));
            AssertExtensions.Throws<ArgumentException>(null, () => module.DefineType("TestType", TypeAttributes.NotPublic));
            AssertExtensions.Throws<ArgumentException>(null, () => module.DefineType("TestType", TypeAttributes.NotPublic, typeof(ModuleBuilderDefineType)));

            AssertExtensions.Throws<ArgumentException>(null, () => module.DefineType("TestType", TypeAttributes.NotPublic, typeof(ModuleBuilderDefineType), PackingSize.Unspecified));
            AssertExtensions.Throws<ArgumentException>(null, () => module.DefineType("TestType", TypeAttributes.NotPublic, typeof(ModuleBuilderDefineType), 0));
            AssertExtensions.Throws<ArgumentException>(null, () => module.DefineType("TestType", TypeAttributes.NotPublic, typeof(ModuleBuilderDefineType), PackingSize.Unspecified, 0));

            AssertExtensions.Throws<ArgumentException>(null, () => module.DefineType("TestType", TypeAttributes.NotPublic, typeof(ModuleBuilderDefineType), new Type[0]));
        }

        [Fact]
        public void DefineType_NonAbstractInterface_ThrowsInvalidOperationException()
        {
            ModuleBuilder module = Helpers.DynamicModule();
            Assert.Throws<InvalidOperationException>(() => module.DefineType("A", TypeAttributes.Interface));
        }
    }
}
