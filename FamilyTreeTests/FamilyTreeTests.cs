namespace FamilyTreeTests
{
    public class FamilyTreeTests
    {

        [Fact]
        public void TestAddParentChildRelation()
        {
            // Call the static method directly
            FamilyTree.ProcessCommand("Pavel P Viktor");

            // Verify the relationship
            var pavel = FamilyTree.GetOrCreatePerson("Pavel");
            var viktor = FamilyTree.GetOrCreatePerson("Viktor");

            Assert.Contains(pavel.Parents, c => c.Name == "Viktor");
            Assert.Contains(viktor.Children, p => p.Name == "Pavel");
        }

        [Fact]
        public void TestAddSiblingRelation()
        {
            // Add Parent-Child relationships
            FamilyTree.ProcessCommand("Pavel P Viktor");
            FamilyTree.ProcessCommand("Tanya P Viktor");

            // Add Sibling relationship
            FamilyTree.ProcessCommand("Pavel S Tanya");

            var pavel = FamilyTree.GetOrCreatePerson("Pavel");
            var tanya = FamilyTree.GetOrCreatePerson("Tanya");

            // Assert that they share the same parents (i.e., they are siblings)
            var pavelParents = pavel.Parents.Select(p => p.Name).OrderBy(name => name).ToList();
            var tanyaParents = tanya.Parents.Select(p => p.Name).OrderBy(name => name).ToList();

            // Assert that Pavel and Tanya have the same parents (they are siblings)
            Assert.Equal(pavelParents, tanyaParents);
        }

    }
    }