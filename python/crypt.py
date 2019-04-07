from simplecrypt import decrypt

passwords = []
encrypted = ''

with open("passwords.txt", "r") as pwd:
    passwords = pwd.readlines()

with open("encrypted.bin", "r") as inp:
    encrypted = inp.read()
    print( encrypted)

for p in passwords:
    print (p)
    try:
        d = decrypt(p.strip(), encrypted)
        print (d)
    except:
        print ('error')


