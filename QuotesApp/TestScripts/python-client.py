import requests
import json
import random
import pick


def load_quotes(quotes):
	for quote in quotes:
		success = post_new_quote(quote)
		if success:
			print("quote post successful")
		else:
			print("quote post failed")
	return

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

def option_post_new_quote():
	quote_content = input('What is the content of the quote? ')
	quote_author = input('What is the author of the quote? ')

	new_quote = {
		'content': quote_content,
		'author': quote_author
	}

	post_new_quote(new_quote)

	return


def post_new_quote(quote):
	url = 'https://localhost:7014/quotes'

	headers = {
		'Content-Type': 'application/json'
	}

	resp = requests.post(url, json=quote, verify=False)

	if 'Location' in resp.headers:
		print(f'New quote is at: {resp.headers["Location"]}')
	else:
		print('Hmmm, there was a problem adding a new quote.')

	if resp.ok:
		return True
	else:
		print(str(resp.content))
		return False

def display_ramdomly_selected_quote(quotes):
	if len(quotes) == 0:
		print("No quotes available.")
		return

	index = random.randint(0, len(quotes) - 1)
	print('Random Quote:')
	print('Content: ' + quotes[index]['content'])
	print('Author: ' + quotes[index]['author'])

def get_all_quotes():
	url = 'https://localhost:7014/quotes'

	headers = {
		'Content-Type': 'application/json'
	}

	resp = requests.get(url, headers=headers, verify=False)

	return resp.json()


def write_quotes_to_file(quote_data):

	for q in quote_data:
		# opening a file for writing named by the quote's ID:
		curr_quote_id = q.get('quoteId')
		quote_file = open(f'data/{curr_quote_id}.txt', 'w')

		# write/dump to the file the txt for that quote:
		quote_file.write(json.dumps(q, indent=2))

		# close the file:
		quote_file.close()

title = 'Please choose your option: '
options = ['Load quotes to the Web API', 'Add a new quote', 'Display a randomly selected quote']
option, index = pick.pick(options, title)
print(option)

if index == 0:
	quotes = read_quotes_from_file('data/quotes.txt')
	load_quotes(quotes)
elif index == 1:
	option_post_new_quote()
elif index == 2:
	data = get_all_quotes()
	display_ramdomly_selected_quote(data['quotes'])
