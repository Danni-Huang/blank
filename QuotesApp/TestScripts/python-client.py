import requests
import json
import random
import pick


quote_api_url = 'https://localhost:7014/quote-api'

def get_api_info():
	resp = requests.get(quote_api_url, verify=False)
	result = resp.json()

	quotes_url = result['links']['quotes']['href']
	tags_url = result['links']['tags']['href']
	tag_assignment_url = result['links']['tagQuote']['href']
	
	return {'quotes_url': quotes_url, 'tags_url': tags_url, 'tag_assignment_url': tag_assignment_url}

def get_tags(tags_url):
	resp = requests.get(tags_url, verify=False)
	return resp.json()

def option_post_new_quote(quotes_url, quote_tags):
	quote_content = input('What is the content of the quote? ')
	quote_author = input('What is the author of the quote? ')

	quote_tag_str = '\n'.join([f'{i+1}). {quote_tags[i]}' for i in range(len(quote_tags))])
	print(f'Choose your tag. The option are...\n{quote_tag_str}\n')
	quote_tag_index = int(input('What tag? '))

	selected_tag = quote_tags[quote_tag_index - 1]

	quote_added_successfully = add_new_quote(quotes_url, quote_content, quote_author, selected_tag)

	if quote_added_successfully:
		print('The new quote was added successfully!\n')
	else:
		print('Hmmm, sorry, there was a problem adding your new quote.\n')


def add_new_quote(quotes_url, quote_content, quote_author, quote_tag):
	headers = {
		'Content-Type': 'application/json'
	}

	new_quote = {
		'content': quote_content,
		'author': quote_author
	}

	resp = requests.post(quotes_url, headers=headers, json=new_quote, verify=False)

	if resp.status_code == 201:
		quote_id = int(resp.headers['Location'].split('/')[-1])

		if quote_tag != '':
			tag_added_successfully = add_tag_to_quote(quotes_url, quote_id, quote_tag)
			return tag_added_successfully
		
		return True
	else:
		print(str(resp.content))
		return False

def add_tag_to_quote(quotes_url, quote_id, quote_tag):
	tag_id = get_tag_id(quotes_url, quote_tag['name'])
	if tag_id is not None:
		tag_assignment_url = tag_assignment_url_temp.format(quoteId=quote_id, tagId=tag_id)
		resp = requests.post(tag_assignment_url, verify=False)
		if resp.status_code == 200:
			return True
		else:
			print(f'Failed to add tag to quote. {str(resp.content)}')
	else:
		print(f'Tag not found: {quote_tag}')
	return False

def get_tag_id(quotes_url, quote_tag):
	resp = requests.get(tags_url, verify=False)

	if resp.status_code == 200:
		tags = resp.json()
		for tag in tags:
			if tag['name'] == quote_tag:
				return tag['tagId']
	else:
		print(f'Failed to get tags. {str(resp.content)}')
	return None


def display_ramdomly_selected_quote(quotes_url):
	all_quotes = get_all_quotes(quotes_url)
	if len(all_quotes) == 0:
		print("No quotes available.")
		return

	index = random.randint(0, len(all_quotes) - 1)
	print('Random Quote:')
	print('Content: ' + all_quotes[index]['content'])
	print('Author: ' + all_quotes[index]['author'])
	print('Tags: ' + ', '.join(str(tag) for tag in all_quotes[index]['tags']))

def get_all_quotes(quotes_url):
	headers = {
		'Content-Type': 'application/json'
	}

	resp = requests.get(quotes_url, headers=headers, verify=False)
	result = resp.json()
	return result['quotes']

def load_quotes(quotes_url):
	print('Loading quotes...')
	headers = {
		'Content-Type': 'application/json'
	}
	quote_added_successfully = False
	try:
		task_file = open('data/quotes.txt', 'r')

		for line in task_file.readlines():
			parts = [c.strip() for c in line.split('|')]
			quote_added_successfully = add_new_quote(quotes_url, parts[0], parts[1], '')	

		if (quote_added_successfully):
			print('Quotes loaded successfully!\n')			
	except:
		print('Sorry, there was a problem loading quotes :(\n')


# set up the app:
api_info = get_api_info()
quotes_url = api_info['quotes_url']
tags_url = api_info['tags_url']
tag_assignment_url_temp = api_info['tag_assignment_url']

quote_tags = get_tags(tags_url)

title = 'Please choose your option: '
options = ['Load quotes to the Web API', 'Add a new quote', 'Display a randomly selected quote']
option, index = pick.pick(options, title)
print(option)

if index == 0:
	load_quotes(quotes_url)
elif index == 1:
	option_post_new_quote(quotes_url, quote_tags)
elif index == 2:
	display_ramdomly_selected_quote(quotes_url)

