from pathlib import Path


class Node():
    def __init__(self, value:str, left:str, right:str):
        self.value = value
        self.left = left
        self.right = right

    @staticmethod
    def FromInput(input:str):
        value, lr = str(input).replace("(", "").replace(")", "").split("=")
        l, r = lr.split(",")
        return Node(value.strip(), l.strip(), r.strip())    

instructions, space, *nodes_str = Path(__file__).parent.joinpath("input.txt").read_text().splitlines()

nodes:list[Node] = [Node.FromInput(node_str) for node_str in nodes_str]

def get_node_by_value(value:str) -> Node:
    return list(filter(lambda n: n.value == value, nodes))[0]

current_step = 0
current_node = get_node_by_value("AAA")

while current_node.value != "ZZZ":
    current_dir = instructions[current_step % len(instructions)]
    if current_dir is 'R':
        print("Stepping Right")
        current_node = get_node_by_value(current_node.right)
    else:
        print("Stepping Left")
        current_node = get_node_by_value(current_node.left)
    current_step += 1

print("Done")
print(f"{current_step}")