using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Reflection.Emit;

namespace FamilyTreeTests
{
    public class FamilyTreeTests : IDisposable
    {

        [Fact]
        public void TestAddParentChildRelation()
        {
            FamilyTree.ProcessCommand("Pavel P Viktor");

            var pavel = FamilyTree.GetOrCreatePerson("Pavel");
            var viktor = FamilyTree.GetOrCreatePerson("Viktor");

            Assert.Contains(pavel.Parents, c => c.Name == "Viktor");
            Assert.Contains(viktor.Children, p => p.Name == "Pavel");


        }

        [Fact]
        public void TestAddSiblingRelation()
        {
            FamilyTree.ProcessCommand("Pavel P Viktor");
            FamilyTree.ProcessCommand("Tanya P Viktor");

            FamilyTree.ProcessCommand("Pavel S Tanya");

            var pavel = FamilyTree.GetOrCreatePerson("Pavel");
            var tanya = FamilyTree.GetOrCreatePerson("Tanya");

            // Assert that they share the same parents 
            var pavelParents = pavel.Parents.Select(p => p.Name).OrderBy(name => name).ToList();
            var tanyaParents = tanya.Parents.Select(p => p.Name).OrderBy(name => name).ToList();

            Assert.Equal(pavelParents, tanyaParents);
        }

        [Fact]

        public void TestParentSiblingRelation()
        {
            FamilyTree.ProcessCommand("Tanya P Viktor");
            FamilyTree.ProcessCommand("Viktor P Vera");
            FamilyTree.ProcessCommand("Tanya PS Valera");

            var viktor = FamilyTree.GetOrCreatePerson("Viktor");
            var valera = FamilyTree.GetOrCreatePerson("Valera");

            // Assert that they share the same parents 
            var viktorParents = viktor.Parents.Select(p => p.Name).OrderBy(name => name).ToList();
            var valeraParents = valera.Parents.Select(p => p.Name).OrderBy(name => name).ToList();

            Assert.Equal(viktorParents, valeraParents);
        }

        [Fact]
        public void TestFindOldestWithMostDescendants()
        {
            // Build the family tree
            FamilyTree.ProcessCommand("Pieter P Tinus");
            FamilyTree.ProcessCommand("Pieter P Petra");
            FamilyTree.ProcessCommand("Pieter C Lila");
            FamilyTree.ProcessCommand("Lila P Emily");

            // Find the oldest with the most descendants
            var result = FamilyTree.FindOldestWithMostDescendants();

            // Assert that Tinus is the oldest with the most descendants
            Assert.NotNull(result);
            Assert.Equal("Tinus", result.Name);

        }

        [Fact]
        public void TestPrintTree()
        {
            // Build the family tree
            FamilyTree.ProcessCommand("Tanya P Elena");
            FamilyTree.ProcessCommand("Tanya P Viktor");
            FamilyTree.ProcessCommand("Tanya C Gabi");

            // Capture console output
            var output = CaptureConsoleOutput(() =>
            {
                var oldestWithMostOffspring = FamilyTree.FindOldestWithMostDescendants();
                if (oldestWithMostOffspring != null)
                {
                    var visited = new List<string>();
                    FamilyTree.PrintTree(oldestWithMostOffspring, 0, visited);
                }
            });

            // Assert the tree structure
            Assert.Contains("Elena", output);
            Assert.Contains("Viktor", output);
            Assert.Contains("    Tanya", output);
            Assert.Contains("        Gabi", output);
        }

        private string CaptureConsoleOutput(Action action)
        {
            var output = new System.IO.StringWriter();
            Console.SetOut(output);

            action();

            return output.ToString();
        }

        public void Dispose()
        {
            FamilyTree.ClearTree(); // Clear the tree after each test
        }
    }

}