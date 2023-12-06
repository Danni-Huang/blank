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
	login_url = result['links']['login']['href']
	register_url = result['links']['register']['href']
	
	return {
		'quotes_url': quotes_url, 
		'tags_url': tags_url, 
		'tag_assignment_url': tag_assignment_url,
		'login_url': login_url,
		'register_url': register_url
	}

def get_tags(tags_url, access_token):
	headers = {
		'Authorization': f'Bearer {access_token}'
	}
	resp = requests.get(tags_url, headers=headers, verify=False)
	
	result = {}
	if resp.status_code == 200:
		result['success'] = True
		
		tag_result = resp.json()
		tag_names = [tag['name'] for tag in tag_result]

		num_tags = len(tag_names)
		
		result['tags'] = tag_names
		
		if num_tags == 0:
			result['message'] = 'Currently no tags'
		else:
			result['message'] = f'Successfully retrieved {num_tags} tags'           
	elif resp.status_code == 401:
		result['success'] = False
		result['tags'] = []
		result['message'] = 'You are not authorized to do this - please log in first.'
	else:
		result['success'] = False
		result['tags'] = []
		result['message'] = 'There was a problem adding the new task'

	return result 


def option_post_new_quote(quotes_url, quote_tags, access_token):
	if access_token == '':
		print('You are not authorized to do this - please log in first.')
	else:
		quote_content = input('What is the content of the quote? ')
		quote_author = input('What is the author of the quote? ')

		quote_tags = get_tags(tags_url, access_token)
		tags = quote_tags['tags']
		quote_tag_str = '\n'.join([f'{i+1}). {tags[i]}' for i in range(len(quote_tags['tags']))])

		print(f'Choose your tag. The option are...\n{quote_tag_str}\n')
		quote_tag_index = int(input('What tag? '))

		selected_tag = tags[quote_tag_index - 1]

		result = add_new_quote(quotes_url, access_token, quote_content, quote_author, selected_tag)

		if result['success']:
			print('The new quote was added successfully!\n')
		else:
			print(result['message'])


def add_new_quote(quotes_url, access_token, quote_content, quote_author, quote_tag):
	headers = {
		'Content-Type': 'application/json',
		'Authorization': f'Bearer {access_token}'
	}

	new_quote = {
		'content': quote_content,
		'author': quote_author
	}

	resp = requests.post(quotes_url, headers=headers, json=new_quote, verify=False)

	result = {}
	if quote_tag != '':
		quote_id = int(resp.headers['Location'].split('/')[-1])
		add_tag_to_quote(quotes_url, access_token, quote_id, quote_tag)
		result['success'] = True
		result['message'] = 'New Quote with tag added successfully.'
	elif resp.status_code == 201 and 'Location' in resp.headers:
		quote_id = int(resp.headers['Location'].split('/')[-1])
		result['success'] = True
		result['message'] = 'New Quote added successfully.'	
	elif resp.status_code == 401:
		result['success'] = False
		result['message'] = 'You are not authorized to do this - please log in first.'
	else:
		result['success'] = False
		result['message'] = 'There was a problem adding the new quote.'
				
	return result

def add_tag_to_quote(quotes_url, access_token, quote_id, quote_tag):
	headers = {
		'Authorization': f'Bearer {access_token}'        
	}
	tag_id = get_tag_id(quotes_url, access_token, quote_tag)

	if tag_id is not None:
		tag_assignment_url = tag_assignment_url_temp.format(quoteId=quote_id, tagId=tag_id)
		resp = requests.post(tag_assignment_url, headers=headers, verify=False)

		if resp.status_code == 200:
			return True
		else:
			print(f'Failed to add tag to quote. {str(resp.content)}')
	else:
		print(f'Tag not found: {quote_tag}')
	return False

def get_tag_id(quotes_url, access_token, quote_tag):
	headers = {
		'Authorization': f'Bearer {access_token}'
	}

	resp = requests.get(tags_url, headers=headers, verify=False)

	if resp.status_code == 200:
		tags = resp.json()
		for tag in tags:
			if tag['name'] == quote_tag:
				return tag['tagId']
	else:
		print(f'Failed to get tags. {str(resp.content)}')
	return None


def display_ramdomly_selected_quote(quotes_url, access_token):
	result = get_all_quotes(quotes_url, access_token)
	if result['success']:
		all_quotes = result['quotes']
		
		if len(all_quotes) > 0:
			random_quote = random.choice(all_quotes)
			print(f'The randomly selected quote is...\nContent: {random_quote["content"]}\nAuthor: {random_quote["author"]}\nTag: {random_quote["tags"]}')
		else:
			print('Currently no quotes to randomly select - add some and try again!')
	else:
		print(result['message'])

	#index = random.randint(0, len(all_quotes) - 1)
	#print('Random Quote:')
	#print('Content: ' + all_quotes[index]['content'])
	#print('Author: ' + all_quotes[index]['author'])
	#print('Tags: ' + ', '.join(str(tag) for tag in all_quotes[index]['tags']))

def get_all_quotes(quotes_url, access_token):
	headers = {
		'Authorization': f'Bearer {access_token}'
	}

	resp = requests.get(quotes_url, headers=headers, verify=False)

	result = {}
	if resp.status_code == 200:
		result['success'] = True
		
		quote_result = resp.json()
		quotes = quote_result['quotes']
		num_quotes = len(quotes)
		
		result['quotes'] = quotes
		
		if num_quotes == 0:
			result['message'] = 'Currently no quotes.'
		else:
			result['message'] = f'Successfully retrieved {num_quotes} tasks'           
	elif resp.status_code == 401:
		result['success'] = False
		result['quotes'] = []
		result['message'] = 'You are not authorized to do this - please log in first'
	else:
		result['success'] = False
		result['quotes'] = []
		result['message'] = 'There was a problem loading quotes.'
	
	return result


def load_quotes(quotes_url, access_token):
	print('Loading quotes...')

	try:
		task_file = open('data/quotes.txt', 'r')
		success = True
		for line in task_file.readlines():
			parts = [c.strip() for c in line.split('|')]
			result = add_new_quote(quotes_url, access_token, parts[0], parts[1], '')

			if not result['success']:
				print(result['message'])
				success = False
				break

		if success:
			print('Quotes loaded successfully!\n')			
	except:
		print('Sorry, there was a problem loading quotes :(\n')

# assignment 4
def handle_register_user(register_url):
	first_name = input('First name? ')
	last_name = input('Last name? ')
	username = input('Username? ')
	password = input('Password? ')
	email = input('Email? ')
	phone_number = input('Phone number? ')
	
	result = register_user(register_url, first_name, last_name, username, password, email, phone_number)
	print(result['message'] + '\n')


def register_user(register_url, first_name, last_name, username, password, email, phone_number):
	headers = {
		'Content-Type': 'application/json'
	}

	# hard-code the presence of the QuoteManager role
	register_request = {
		'firstName': first_name,
		'lastName': last_name,
		'userName': username,
		'password': password,
		'email': email,
		'phoneNumber': phone_number,
		'roles': ['QUOTE_MANAGER']
	}

	resp = requests.post(register_url, headers=headers, json=register_request, verify=False)

	result = {}
	if resp.status_code == 201:
		result['success'] = True        
		result['message'] = 'User registered successfully'
	else:
		result['success'] = False
		if resp.status_code == 400:
			result['message'] = 'The email or user name is already taken.'
		else:
			result['message'] = 'There was a problem registering this new user'

	return result

def handle_login(login_url):
	username = input('Username? ')
	password = input('Password? ')
	
	result = login_user(login_url, username, password)
	print(result['message'] + '\n')

	return result['token']

def login_user(login_url, username, password):
	headers = {
		'Content-Type': 'application/json'
	}

	login_request = {
	  'userName': username,
	  'password': password
	}

	resp = requests.post(login_url, headers=headers, json=login_request, verify=False)

	result = {}
	if resp.status_code == 200:
		result['success'] = True        
		login_result = resp.json()
		result['token'] = login_result['token']
		result['message'] = 'Logged in successfully'
	else:
		result['success'] = False
		result['token'] = ''
		result['message'] = 'There was a problem logging in'

	return result

# set up the app:
api_info = get_api_info()
quotes_url = api_info['quotes_url']
tags_url = api_info['tags_url']
tag_assignment_url_temp = api_info['tag_assignment_url']

register_url = api_info['register_url']
login_url = api_info['login_url']

done = False
title = '\n\nWhat do you want to do? '
options = ['Register a user', 'Log in', 'Load quotes to the Web API', 'Add a new quote', 'Display a randomly selected quote', 'Quit']

access_token = ''
quote_tags = get_tags(tags_url, access_token)

while not done:
	print('\n' + '\n'.join([f'{i+1}). {options[i]}' for i in range(len(options))]))
	main_index = int(input(title)) - 1

	if main_index == 0:
		handle_register_user(register_url)
	elif main_index == 1:
		access_token = handle_login(login_url)
	elif main_index == 2:
		load_quotes(quotes_url, access_token)
	elif main_index == 3:
		option_post_new_quote(quotes_url, quote_tags, access_token)
	elif main_index == 4:
		display_ramdomly_selected_quote(quotes_url, access_token)
	else:
		print('\n\nGoodbye!\n')
		done = True

