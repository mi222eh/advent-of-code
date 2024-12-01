use std::{env::current_dir, fs};

fn main() {
    let mut result = 0;

    let path = current_dir().unwrap().join("input.txt");
    let content = fs::read_to_string(path.as_path()).unwrap();

    let lines: Vec<&str> = content.split_whitespace().collect();
    let mut current_y = 0;
    lines.iter().for_each(|line| {
        let mut start_index = 0;

        loop {
            if start_index >= line.chars().count() - 1 {
                break;
            }
            let mut end_index = start_index;
            let mut current_nr = String::new();
            loop {
                if end_index > line.chars().count() - 1 {
                    break;
                }
                let current_char = line.chars().nth(end_index).expect(&format!(
                    "Could not get character at {end_index} from line {current_y}"
                ));
                if current_char.is_numeric() {
                    current_nr = format!("{current_nr}{current_char}");
                    end_index += 1;
                } else {
                    break;
                }
            }

            let there_is_nr = current_nr.chars().count() > 0;
            if there_is_nr {
                let has_symbol = has_symbol(
                    &lines,
                    start_index.try_into().unwrap(),
                    current_y,
                    current_nr.chars().count().try_into().unwrap(),
                );
                if has_symbol {
                    result += current_nr.parse::<i32>().unwrap()
                }
            }

            if there_is_nr {
                start_index = end_index;
            } else {
                start_index += 1;
            }
        }
        current_y += 1;
    });

    println!("Result is {result}")
}

fn is_symbol(c: char) -> bool {
    if c == '.' {
        return false;
    } else if c.is_numeric() {
        return false;
    } else {
        return true;
    }
}

fn has_symbol(lines: &Vec<&str>, from_x: i32, y: i32, nr_size: i32) -> bool {
    let line_length = lines
        .iter()
        .nth(y.try_into().unwrap())
        .unwrap()
        .chars()
        .count()
        - 1;

    let check_to_x = max_nr(from_x + nr_size + 1, line_length.try_into().unwrap());
    let check_to_y = max_nr(y + 1, line_length.try_into().unwrap());

    for x_add in 0..=nr_size + 1 {
        for y_add in 0..=2 {
            let current_x = ensure_positive(from_x - 1 + x_add);
            let current_y = ensure_positive(y - 1 + y_add);
            if current_x > check_to_x.try_into().unwrap() {
                break;
            }
            if current_y > check_to_y.try_into().unwrap() {
                break;
            }
            if (current_x > line_length.try_into().unwrap()) {
                break;
            }
            if current_y > lines.iter().count().try_into().unwrap() {
                break;
            }

            let current_char = lines
                .iter()
                .nth(current_y.try_into().unwrap())
                .unwrap()
                .chars()
                .nth(current_x.try_into().unwrap())
                .expect(&format!(
                    "Failed to get {current_x}th character from line {current_y}"
                ));

            let is_symbol = is_symbol(current_char);
            if is_symbol {
                println!("The number at {from_x},{y} is included");
                return true;
            }
        }
    }

    println!("The number at {from_x},{y} is NOT included");
    return false;
}

fn ensure_positive(nr: i32) -> usize {
    if nr < 0 {
        0
    } else {
        nr.try_into().unwrap()
    }
}

fn max_nr(nr: i32, max_nr: i32) -> i32 {
    if nr < max_nr {
        nr
    } else {
        max_nr
    }
}
