import csv
import os.path
from pymongo import MongoClient
import requests

client = MongoClient()
db = client['udenad']
cards = db['cards']

with open('words.csv', 'w', newline='') as csvfile:
    writer = csv.writer(csvfile, delimiter=' ', quotechar='|', quoting=csv.QUOTE_MINIMAL)
    writer = csv.DictWriter(csvfile, fieldnames=['Front', 'Back'])
    writer.writeheader()
    for card in cards.find():
        word = card['_id']

        word_class = ''
        if card['WordClass'] != None:
            word_class = card['WordClass']

        inflection = ''
        if card['Inflection'] != None:
            inflection = card['Inflection']

        definitions = ''
        if card['Definitions'] != None:
            definitions = card['Definitions']

        writer.writerow({
            'Front': f'<strong>{word}</strong><br>{word_class}<br>{inflection}',
            'Back': f'{definitions}'
        })
        print(word)