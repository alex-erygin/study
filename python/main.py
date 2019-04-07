import requests


with open('C:\\Users\\Aleksandr\\Downloads\\dataset_3378_3.txt', 'r') as f:
    url = f.readline().strip()
    print(url)
    r = requests.get(url)
    url = 'https://stepic.org/media/attachments/course67/3.6.3/' + r.text
    while 1 == 1:
        try:
            r = requests.get(url)
            if r.status_code != 200:
                break
            url = 'https://stepic.org/media/attachments/course67/3.6.3/' + r.text
            print(url)
        except:
            break