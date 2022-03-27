fn main() {
    let three = 0b11;//двоичная
    let thirty = 0o36;//восмеричная
    let three_hundred = 0x12C;//шестнадцатиричная

    println!("base 10: {} {} {}", three, thirty, three_hundred);
    println!("base 2: {:b} {:b} {:b}", three, thirty, three_hundred);
    println!("base 8: {:o} {:o} {:o}", three, thirty, three_hundred);
    println!("base 16: {:x} {:x} {:x}", three, thirty, three_hundred);
}
