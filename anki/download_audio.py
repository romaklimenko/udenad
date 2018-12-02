import os.path
from pymongo import MongoClient
import requests

client = MongoClient()
db = client['udenad']
cards = db['cards']

for card in cards.find({ 'Audio': { '$ne': '' } }):
    word = card["_id"]
    file_path = f'mp3/{word}.mp3'
    if os.path.exists(file_path):
        continue

    response = requests.get(card['Audio'], allow_redirects=True)
    open(f'mp3/{word}.mp3', 'wb').write(response.content)
    print(word)