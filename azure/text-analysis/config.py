import string


class Config:
    def __init__(self, key: string, location: string):
        self.key = key
        self.endpoint = location
