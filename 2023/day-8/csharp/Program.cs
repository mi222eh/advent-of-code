using System.Linq;

var lines = File.ReadAllLines("../../../input.txt");

var dirs = lines[0];
var nodes = lines.Skip(2).Select(Node.FromInput).ToList();
var node_dic = nodes.ToDictionary(x => x.Value);

var current_nodes = nodes.GetManyNodesByEndsWithValue("A");
long nr_of_loops = 0;
int current_step = 0;

while (!current_nodes.TrueForAll(x => x.Value.EndsWith("Z")))
{
    if (current_step >= dirs.Length){
        current_step = 0;
        nr_of_loops++;
    }
    current_nodes = dirs[current_step] switch {
        'R' => current_nodes.Select(x => node_dic[x.R]).ToList(),
        'L' => current_nodes.Select(x => node_dic[x.L]).ToList()
    };
    Console.WriteLine($"Steps: {GetTotalSteps()}");
    current_step++;
}


Console.WriteLine(GetTotalSteps());

long GetTotalSteps(){
    long total = nr_of_loops * dirs.Length + current_step;
    return total;
}

public static class Helper{
    public static Node GetNodeByValue(this List<Node> nodes, string value) => nodes.First(x => x.Value == value);
    public static List<Node> GetManyNodesByEndsWithValue(this List<Node> nodes, string value) => nodes.Where(x => x.Value.EndsWith(value)).ToList();
} 
public record Node(string Value, string L, string R){
    public static Node FromInput(string input){
        var values = input.Split('=');
        var value = values[0].Trim();
        var lr = values[1].Replace("(", "").Replace(")", "").Split(',');
        return new Node(value, lr[0].Trim(), lr[1].Trim());
    }
};