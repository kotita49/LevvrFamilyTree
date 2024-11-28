public class Person
{
    public string Name { get; set; }
    public List<Person> Parents { get; set; } = new List<Person>();
    public List<Person> Children { get; set; } = new List<Person>();

    public Person(string name)
    {
        Name = name;
    }
}

public class FamilyTree
{
    static Dictionary<string, Person> people = new Dictionary<string, Person>();

    public static Person GetOrCreatePerson(string name)
    {
        if (!people.ContainsKey(name))
        {
            people[name] = new Person(name);
        }
        return people[name];
    }

    static void AddParentChildRelation(Person parent, Person child)
    {
        // Ensure no duplicates in the parent's or child's relationship lists
        if (!child.Parents.Contains(parent))
        {
            child.Parents.Add(parent);
        }
        if (!parent.Children.Contains(child))
        {
            parent.Children.Add(child);
        }
    }

    static void AddSiblingRelation(Person sibling1, Person sibling2)
    {
        // Add shared parents between siblings
        foreach (var parent in sibling1.Parents)
        {
            AddParentChildRelation(parent, sibling2);
        }
        foreach (var parent in sibling2.Parents)
        {
            AddParentChildRelation(parent, sibling1);
        }

        sibling1.Parents.AddRange(sibling2.Parents.Except(sibling1.Parents));
        sibling2.Parents.AddRange(sibling1.Parents.Except(sibling2.Parents));

    }

    public static void ProcessCommand(string command)
    {
        var parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3)
        {
            Console.WriteLine("Invalid command. Please try again.");
            return;
        }

        string person1Name = parts[0];
        string relation = parts[1];
        string person2Name = parts[2];

        Person person1 = GetOrCreatePerson(person1Name);
        Person person2 = GetOrCreatePerson(person2Name);

        switch (relation)
        {
            case "P": // Parent
                AddParentChildRelation(person2, person1);
                Console.WriteLine($"{person2.Name} is {person1.Name}'s parent.");
                break;

            case "C": // Child
                AddParentChildRelation(person1, person2);
                Console.WriteLine($"{person2.Name} is {person1.Name}'s child.");
                break;
                           

            case "S": // Sibling
                AddSiblingRelation(person1, person2);
                Console.WriteLine($"{person2.Name} is {person1.Name}'s sibling.");
                break;

            case "PS": // Parent's sibling (aunt/uncle)
                foreach (var parent in person1.Parents)
                {
                    // Add a sibling relationship between the parent of person1 and person2
                    AddSiblingRelation(parent, person2);
                }
                Console.WriteLine($"{person2.Name} is {person1.Name}'s aunt/uncle.");
                break;

            default:
                Console.WriteLine("Invalid relation type. Please use P, C, S, or PS.");
                break;
        }
    }

    public static void PrintTree(Person person, int depth, List<string> visited)
    {
        if (visited.Contains(person.Name)) return;

        // Mark the person as visited to avoid reprinting
        visited.Add(person.Name);

        // Print all parents at the same depth
        var parents = person.Parents.OrderBy(p => p.Name).ToList();
        foreach (var parent in parents)
        {
            if (!visited.Contains(parent.Name))
            {
                PrintTree(parent, depth - 1, visited);
            }
        }

        // Print the person's name at the appropriate depth
        Console.WriteLine(new string(' ', depth * 4) + person.Name);

        // iterate over all children and print them
        var children = person.Children.OrderBy(c => c.Name).ToList();
        foreach (var child in children)
        {
            if (!visited.Contains(child.Name))
            {
                PrintTree(child, depth + 1, visited);
            }
        }
    }

    public static void ClearTree()
    {
        people.Clear();
    }

    static void Main()
    {
        Console.WriteLine("Family Tree Application");
        Console.WriteLine("Enter commands to build the family tree, or type 'PRINT' to display the tree.");
        Console.WriteLine("Type 'EXIT' to quit the program.");
        Console.WriteLine("Command format: [Person1] [Relation] [Person2] (for relation use P (parent), C (child), S (sibling), PS (parent sibling)");

        while (true)
        {
            Console.Write(" ");
            string input = Console.ReadLine();
            if (input.Equals("EXIT", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }
            else if (input.Equals("PRINT", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Family Tree:");
                var oldestWithMostDescendants = FindOldestWithMostDescendants();
                if (oldestWithMostDescendants != null)
                {
                    var visited = new List<string>();
                    PrintTree(oldestWithMostDescendants, 0, visited);
                }
            }
            else
            {
                ProcessCommand(input);
            }
        }
    }

    public static Person? FindOldestWithMostDescendants()
    {
        // Helper method to calculate total descendants of a person 
        int CountDescendants(Person person)
        {
            if (person.Children.Count == 0) return 0; 
            int count = person.Children.Count; // Count direct children
            foreach (var child in person.Children)
            {
                count += CountDescendants(child); // Add descendants of each child
            }
            return count;
        }

        // Find all people who have no parents (most likely oldest members)
        var oldestMembers = people.Values.Where(p => p.Parents.Count == 0);

        // Find the oldest member with the most descendants
        return oldestMembers
       .OrderByDescending(person => CountDescendants(person))
       .FirstOrDefault();
    }

}
