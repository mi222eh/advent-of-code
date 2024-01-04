use std::{env::current_dir, fs};

const MAX_NR_OF_RED:i32 = 12;
const MAX_NR_OF_GREEN:i32 = 13;
const MAX_NR_OF_BLUE:i32 = 14;

fn main() {
    let mut result_1 = 0;
    let mut result_2 = 0;
    let file = current_dir().unwrap().join("input.txt");
    let content = fs::read_to_string(file.as_path()).unwrap();

    content.split("\n").for_each(|row|{
        let mut greens = 0;
        let mut blues = 0;
        let mut reds = 0;
        
        let game_parts:Vec<&str> = row.split(":").collect();

        let game_id_str = game_parts.first().unwrap();
        let game_data_str = game_parts.last().unwrap();

        let game_id:i32 = game_id_str.replace("Game ", "").parse().unwrap();

        let mut game_data_sets = game_data_str.split(";");
        let is_game_possible = game_data_sets.all(|set_str| {
            let mut color_parts_str = set_str.split(",");
            color_parts_str.all(|color_str| {
                let data:Vec<&str> = color_str.trim().split(" ").collect();
                let count:i32 = data.first().unwrap().parse().unwrap();
                let color = data.last().unwrap().to_owned();

                match color {
                    "blue" => count <= MAX_NR_OF_BLUE,
                    "green" => count <= MAX_NR_OF_GREEN,
                    "red" => count <= MAX_NR_OF_RED,
                    _ => false
                }
            })


        });

        game_data_str.split(";").for_each(|set_str| {
            let color_parts_str = set_str.split(",");
            color_parts_str.for_each(|color_str| {
                let data:Vec<&str> = color_str.trim().split(" ").collect();
                let count:i32 = data.first().unwrap().parse().unwrap();
                let color = data.last().unwrap().to_owned();

                println!("Found {count} of color {color}");

                match color {
                    "blue" => blues = get_bigger(blues, count),
                    "green" => greens = get_bigger(greens, count),
                    "red" => reds = get_bigger(reds, count),
                    &_ => panic!()
                };
            })
        });

        let result_power = blues * greens * reds;
        result_2 += result_power;


        if is_game_possible {
            result_1 += game_id;
        }
    });

    println!("Result part 1: {result_1}");
    println!("Result part 2: {result_2}");
}


fn get_bigger(nr1: i32, nr2: i32) -> i32 {
    println!("current is {nr1}");
    if nr1 > nr2 {
        println!("returning nr1");
        nr1
    } else {
        println!("returning nr2");
        nr2
    }
}