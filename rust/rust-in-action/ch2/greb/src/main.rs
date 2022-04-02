use std::fs::{File};
use std::io;
use std::io::{BufRead, BufReader};
use regex::Regex;
use clap::{App, Arg};

fn process_lines<T: BufRead + Sized>(reader: T, re: Regex) {
    let mut line_counter = 0;
    for line_ in reader.lines() {
        let line = line_.unwrap();
        match re.find(&line) {
            Some(_) => println!("{}", line),
            None => (),
        }
        line_counter += 1;
    }
    println!("Обошли {} линий", line_counter);
}

fn main() {
    let args = App::new("grep-lite")
        .version("0.1")
        .about("Ищет по паттернам")
        .arg(Arg::with_name("pattern")
            .help("Паттерн для поиска")
            .takes_value(true)
            .required(true))
        .arg(Arg::with_name("input")
            .help("Файл для поиска")
            .takes_value(true)
            .required(false))
        .get_matches();

    let pattern = args.value_of("pattern").unwrap();
    let re = Regex::new(pattern).unwrap();

    let input = args.value_of("input").unwrap_or("-");

    println!("{}", pattern);
    println!("{}", input);

    if input == "-" {
        let stdin = io::stdin();
        let reader = stdin.lock();
        process_lines(reader, re);
    } else {
        let f = File::open(input).unwrap();
        let reader = BufReader::new(f);
        process_lines(reader, re);
    }


}
