using Microsoft.VisualStudio.TestTools.UnitTesting;
using Promact.Caching;
using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Promact.Caching.Test
{
    [TestClass]
    public class FunctionalLogicBasedUniqueKeyGenerationTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GenerateUniqueKey_NullKey_ThrowsArgumentNullException()
        {
            // Arrange
            var generator = new FunctionalLogicBasedUniqueKeyGeneration();
            string key = null;
            string[] args = { "SomeType", "SomeMethod" };

            // Act
            generator.GenerateUniqueKey(key, args);

            // Assert is handled by ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GenerateUniqueKey_NullArgs_ThrowsArgumentNullException()
        {
            // Arrange
            var generator = new FunctionalLogicBasedUniqueKeyGeneration();
            string key = "SomeKey";
            string[] args = null;

            // Act
            generator.GenerateUniqueKey(key, args);

            // Assert is handled by ExpectedException
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Type not found")]
        public void GenerateUniqueKey_InvalidTypeName_ThrowsArgumentException()
        {
            // Arrange
            var generator = new FunctionalLogicBasedUniqueKeyGeneration();
            string key = "SomeKey";
            string[] args = { "InvalidTypeName", "SomeMethod" };

            // Act
            generator.GenerateUniqueKey(key, args);

            // Assert is handled by ExpectedException
        }

        [TestMethod]
        public void GenerateUniqueKey_ValidInput_ReturnsExpectedKeyFormat()
        {
            // Arrange
            var generator = new FunctionalLogicBasedUniqueKeyGeneration();
            string key = "SomeKey";
            // Use a real type and method name from your project or .NET framework
            string[] args = { typeof(string).FullName, "Insert" };

            // Act
            var result = generator.GenerateUniqueKey(key, args);

            // Assert
            Assert.IsTrue(result.StartsWith($"{key}-"));
        }

        //[TestMethod]
        //public void ComputeMethodHash_DifferentMethodBodies_ProducesDifferentHashes()
        //{
        //    var generator = new FunctionalLogicBasedUniqueKeyGeneration();

        //    // Create a dynamic type with a method
        //    var type1 = DynamicMethodExample.CreateDynamicTypeWithMethod("TestMethod", "Body1");
        //    var hash1 = generator.GenerateUniqueKey("SomeKey", new string[] { type1.FullName, "TestMethod" });

        //    // Create another dynamic type with a different method body
        //    var type2 = DynamicMethodExample.CreateDynamicTypeWithMethod("TestMethod", "Body2");
        //    var hash2 = generator.GenerateUniqueKey("SomeKey", new string[] { type2.FullName, "TestMethod" });

        //    Assert.AreNotEqual(hash1, hash2, "Hashes should be different for different method bodies.");
        //}


        // Additional tests can be added here to cover more scenarios
    }


    internal class FakeType
    {
        public void FakeMethod()
        {
            int a = 10, b = 20;
            var c = a + b;
        }
    }

    public class DynamicMethodExample
    {
        public static Type CreateDynamicTypeWithMethod(string methodName, string methodBody)
        {
            var assemblyName = new AssemblyName("DynamicAssembly");
            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            var typeBuilder = moduleBuilder.DefineType("DynamicType", TypeAttributes.Public);

            var methodBuilder = typeBuilder.DefineMethod(methodName,
                MethodAttributes.Public | MethodAttributes.Static,
                typeof(void),
                Type.EmptyTypes);

            var ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.EmitWriteLine(methodBody);
            ilGenerator.Emit(OpCodes.Ret);

            return typeBuilder.CreateType();
        }        
    }
}

