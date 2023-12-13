use std::{
    collections::{btree_map::Range, HashMap},
    env::current_dir,
    fs::{self, File},
    io::BufReader,
    vec,
};

fn main() {
    let binding = current_dir().unwrap().join("input.txt");

    let path_to_file = binding.to_str().unwrap();

    let file_content = fs::read_to_string(path_to_file).expect(&format!("Cannot read file"));

    let numberList: Vec<_> = file_content
        .split("\n")
        .map(|row| {
            let number_list = find_nrs_in_string(row);

            let first_nr = number_list
                .first()
                .expect(format!("Error getting the first number of {row}").as_str());

            let second_nr = number_list
                .last()
                .expect(format!("Error getting the last number of {row}").as_str());

            let result: String = vec![first_nr.to_string(), second_nr.to_string()]
                .into_iter()
                .collect();
            result
        })
        .map(|nr_string| {
            nr_string
                .parse::<i32>()
                .expect(format!("Error parsing {nr_string} into a numberr").as_str())
        })
        .collect();

    let result: i32 = numberList.iter().sum();
    print!("The sum is {result}")
}

struct NrEntry {
    nr: i32,
    index: usize,
}

fn find_nrs_in_string(x: &str) -> Vec<i32> {
    let nr_map = HashMap::from([
        ("one", 1),
        ("two", 2),
        ("three", 3),
        ("four", 4),
        ("five", 5),
        ("six", 6),
        ("seven", 7),
        ("eight", 8),
        ("nine", 9),
    ]);

    let mut nr_list: Vec<NrEntry> = vec![];

    for n in 1..=9 {
        let current_nr = n.to_string();
        x.match_indices(&current_nr).for_each(|nr| {
            let (index, nr_str) = nr;

            nr_list.push(NrEntry {
                index: index,
                nr: nr_str.parse::<i32>().unwrap(),
            })
        })
    }

    for n in nr_map.iter() {
        let (nr_string, nr_nr) = n;
        x.match_indices(nr_string).for_each(|nr| {
            let (index, nr_str) = nr;

            nr_list.push(NrEntry {
                index: index,
                nr: nr_nr.to_owned(),
            })
        })
    }

    nr_list.sort_by(|a, b| a.index.cmp(&b.index));
    nr_list.iter().map(|a| a.nr).collect()
}
