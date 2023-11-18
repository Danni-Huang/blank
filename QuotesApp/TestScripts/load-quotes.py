import requests
import json

def get_all_quotes():
	url = 'https://localhost:7014/quotes'

	headers = {
		'Content-Type': 'application/json'
	}

	resp = requests.get(url, headers=headers, verify=False)

	return resp.json()

def load_quotes(quotes):
	for quote in quotes:
		success = post_quote(quote)
		if success:
			print("quote post successful")
		else:
			print("quote post failed")
	return

def write_quotes_to_file(quote_data):

	for q in quote_data:
		# opening a file for writing named by the quote's ID:
		curr_quote_id = q.get('quoteId')
		quote_file = open(f'data/{curr_quote_id}.txt', 'w')

		# write/dump to the file the txt for that quote:
		quote_file.write(json.dumps(q, indent=2))

		# close the file:
		quote_file.close()


def read_quotes_from_file(file_path):
    quotes = []

    with open(file_path, 'r') as file:
        lines = file.readlines()

        for line in lines:
            # Split the line into content and author using '--'
            parts = line.strip().split('--')

            if len(parts) == 2:
                content = parts[0].strip().replace("\"", "")
                author = parts[1].strip()
                quote = {
					"content": content,
					"author": author
				}
                quotes.append(quote)

    return quotes

def post_new_quote(quote):
	url = 'https://localhost:7014/quotes'

	headers = {
		'Content-Type': 'application/json'
	}

	quote_content = input('What is the content of the quote? ')
	quote_author = input('What is the author of the quote? ')

	new_quote = {
		'content': quote_content,
		'author': quote_author
	}

	resp = requests.post(url, json=new_quote, verify=False)

	if 'Location' in resp.headers:
		print(f'New quote is at: {resp.headers["Location"]}')
	else:
		print('Hmmm, there was a problem adding a new quote.')

	if resp.ok:
		return True
	else:
		return False

quotes = read_quotes_from_file('QuotesApp/TestScripts/data/quotes.txt')
load_quotes(quotes)
