$(document).ready(function () {
    let _quoteItemsMsg = $('#quoteItemsMsg');
    let _quotesList = $('#quotesList');
    let _newQuoteMsg = $('#newQuoteMsg');
    let _newTagMsg = $('#newTagMsg');
    let _confirmUpdatedQuoteMsg = $('#confirmUpdatedQuoteMsg');
    let _mostLikedQuotesList = $('#mostLikedQuotesList');
    let _quoteRankMsg = $('#quoteRankMsg');
    let _existingTags = $('#existingTags');
    let _quotesByTag = $('#quotesByTag');

    let _quotesLastModified = new Date(1970, 0, 1);

    let _quotesApiHome = 'https://localhost:7014/quote-api';
    let _quotesUrl = null;
    let _tagsUrl = null;
    let _likeUrl = null;
    let _quoteByIdUrl = null;
    let _quotesByRankUrl = null;
    let _tagQuoteUrl = null;

    // this method will call api home page at startup & set the quotes URL
    let loadBaseApiInfo = async function () {
        const resp = await fetch(_quotesApiHome, {
            mode: 'cors',
            headers: {
                'Accept': 'application/json'
            }
        });

        if (resp.status === 200) {
            const apiHomeResult = await resp.json();
            const links = apiHomeResult.links;

            _quotesUrl = links['quotes'].href;
            _tagsUrl = links['tags'].href;
            _quoteByIdUrl = links['quoteById'].href;
            _likeUrl = links['like'].href;
            _quotesByRankUrl = links['quotesByRank'].href;
            _tagQuoteUrl = links['tagQuote'].href;

        } else {
            _quoteItemsMsg.text('Hmmmm, there was a problem accessing the quotes API.');
            _quoteItemsMsg.attr('class', 'text-danger');
            _quoteItemsMsg.fadeOut(10000);
        }
        loadTags();
    };

    var loadTags = async function (id = '#quoteTag') {
        let _quoteTag = $(id);
        _quoteTag.empty();
        const resp = await fetch(_tagsUrl, {
            mode: 'cors',
            headers: {
                'Accept': 'application/json'
            }
        });

        if (resp.status === 200) {
            const tags = await resp.json();
            console.log(tags)
            for (let i = 0; i < tags.length; i++) {
                _quoteTag.append(`<option value=\"${tags[i].tagId}\">${tags[i].name}</option>`);
            }
        } else {
            _quoteItemsMsg.text('Hmmmm, there was a problem accessing the quotes tags.');
            _quoteItemsMsg.attr('class', 'text-danger');
            _quoteItemsMsg.fadeOut(10000);
        }
    };

    let loadQuotes = async function () {
        // call out to web api using fetch (enabling CORS) to get our quotes:
        let resp = await fetch(_quotesUrl, {
            mode: "cors",
            headers: {
                'Accept': 'application/json'
            }
        });

        if (resp.status === 200) {
            let quotesResult = await resp.json();
            let quotes = quotesResult.quotes;

            if (quotes.length === 0) {
                _quoteItemsMsg.text('No quotes to display - use the form to add some.');
            } else {
                let latestLastModified = new Date(quotesResult.quotesLastModified);

                if (latestLastModified.getTime() > _quotesLastModified.getTime()) {
                    _quotesLastModified = latestLastModified;

                    // loop through the quotes and add them to the UL list:
                    _quotesList.empty();

                    for (let i = 0; i < quotes.length; i++) {
                        _quotesList.append(`
                        <li data-quote-id="${quotes[i].quoteId}">
                        "${quotes[i].content}" (<b>ID:${quotes[i].quoteId}</b>) The author is: ${quotes[i].author} 
                        <button class="thumb-up-btn">👍 Like</button></li>`);
                    }
                }        
            }

            // _quoteItemsMsg.text('# quote is: ' + quotes.length);   
        }
        else {
            _quoteItemsMsg.text('Hmmmm, there was a problem loading the quotes.');
            _quoteItemsMsg.attr('class', 'text-danger');
            _quoteItemsMsg.fadeOut(10000);
        }

    };

    // add a click handler to POST new quotes to our API:
    $('#addQuoteBtn').click(async function () {
        // Create a new quote by reading the form input fields:
        let newQuote = {
            content: $('#quoteContent').val(),
            author: $('#quoteAuthor').val(),
            tag: $('#quoteTag').val()
        };

        let resp = await fetch(_quotesUrl, {
            mode: "cors",
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newQuote)
        });

        if (resp.status === 201) {
            _newQuoteMsg.text('The quote was added successfully.');
            _newQuoteMsg.attr('class', 'text-success');
            $('#quoteContent').val('')
            $('#quoteAuthor').val('')
        } else {
            _newQuoteMsg.text('Hmmm, there was a problem adding the quotes.');
            _newQuoteMsg.attr('class', 'text-danger');
        }
        _newQuoteMsg.fadeOut(10000);
    });

    // add a click handler to POST new tag to our API:
    $('#addTagBtn').click(async function () {
        // Create a new tag by reading the form input fields:
        let newTag = {
            name: $('#tagName').val()
        };

        let resp = await fetch(_tagsUrl, {
            mode: "cors",
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newTag)
        });

        if (resp.status === 201) {
            _newTagMsg.text('The tag was added successfully.');
            _newTagMsg.attr('class', 'text-success');
            $('#tagName').val('')
        } else {
            _newTagMsg.text('Hmmm, there was a problem adding the tags.');
            _newTagMsg.attr('class', 'text-danger');
        }
        _newTagMsg.fadeOut(10000);
    });


    // add a click handler to show edit quote form:
    $('#editQuoteBtn').click(async function () {
        const selectedQuoteId = $('#selectedQuoteId').val();

        if (selectedQuoteId) {
            const urlWithId = _quoteByIdUrl.replace('{id}', selectedQuoteId);

            let resp = await fetch(urlWithId, {
                mode: "cors",
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            _existingTags.empty();
            if (resp.status === 200) {
                const quoteDetails = await resp.json();

                for (const tag of quoteDetails.tags) {
                    _existingTags.append(`<span class="badge text-bg-dark">${tag}</span>  `)
                }
                $('#editQuoteContent').val(quoteDetails.content);
                $('#editQuoteAuthor').val(quoteDetails.author);
                // $('#quoteTag').val(quoteDetails.tag);
                loadTags('#quoteTagForEditQuote');
                $('#editQuoteForm').show();
            }
        }
    });

    $('#confirmUpdateBtn').click(async function () {
        const selectedQuoteId = $('#selectedQuoteId').val();
        const urlWithId = _quoteByIdUrl.replace('{id}', selectedQuoteId);

        const updatedContent = $('#editQuoteContent').val();
        const updatedAuthor = $('#editQuoteAuthor').val();
        const updatedTagId = $('#quoteTagForEditQuote option:selected').val();

        const requestBody = JSON.stringify({
            content: updatedContent,
            author: updatedAuthor,
        });

        const response = await fetch(urlWithId, {
            mode: "cors",
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: requestBody
        });

        if (response.ok) {
            const url = _tagQuoteUrl.replace('{quoteId}', selectedQuoteId).replace('{tagId}', updatedTagId)
            const response = await fetch(url, {
                mode: "cors",
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            console.log(url, response.status)
            _confirmUpdatedQuoteMsg.text('The quote was updated successfully.');
            _confirmUpdatedQuoteMsg.attr('class', 'text-success');
            $('#selectedQuoteId').val('');
            $('#editQuoteForm').hide();
        } else {
            _confirmUpdatedQuoteMsg.text('Hmmmm, there was a problem updating the quote.');
            _confirmUpdatedQuoteMsg.attr('class', 'text-danger');
        }

        _confirmUpdatedQuoteMsg.fadeOut(2000);
        
    });

    $(document).on('click', '.thumb-up-btn', async function () {
        // get the current quote id
        const quoteId = $(this).closest('li').data('quote-id');
        const urlWithId = _likeUrl.replace('{id}', quoteId);
        console.log(quoteId);

        let newLike = {
            // TODO: will change it in assignment 4
            userId: 123,
            quoteId: quoteId
        };

        let resp = await fetch(urlWithId, {
            mode: "cors",
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(newLike)
        });      

        if (resp.status === 204 || resp.status === 200) {
            _quoteItemsMsg.stop().fadeIn(1);
            _quoteItemsMsg.text('Thumb up on Quote ID: ' + quoteId + '!');
            _quoteItemsMsg.attr('class', 'text-success'); 
            _quoteItemsMsg.fadeOut(2000);
        } else {
            _quoteItemsMsg.stop().fadeIn(1);
            _quoteItemsMsg.text('Hmmm, there was a problem to thumb up.');
            _quoteItemsMsg.attr('class', 'text-danger'); 
            _quoteItemsMsg.fadeOut(2000);
        }   

    });

    $('#numsMostLikedQuotesBtn').click(async function () {
        let _quotesLastModified = new Date(1970, 0, 1);
        let nums = $('#numsMostLikedQuotes').val();
        let resp = null;
        console.log("nums", nums);
        if (nums) {
            resp = await fetch(_quotesByRankUrl + '?limit=' + nums, {
                mode: "cors",
                headers: {
                    'Accept': 'application/json'
                }
            });

            if (resp.status === 200) {
                let quotesResult = await resp.json();
                let quotes = quotesResult.quotes;

                let latestLastModified = new Date(quotesResult.quotesLastModified);

                if (latestLastModified.getTime() > _quotesLastModified.getTime()) {
                    _quotesLastModified = latestLastModified;

                    // loop through the quotes and add them to the UL list:
                    _mostLikedQuotesList.empty();

                    for (let i = 0; i < nums; i++) {
                        _mostLikedQuotesList.append(`
                <li data-quote-id="${quotes[i].quoteId}">
                "${quotes[i].content}" (ID:${quotes[i].quoteId}) The author is: ${quotes[i].author} 
                <b>Likes:</b> ${quotes[i].likes}`);
                    }
                }
            }
            else {
                _quoteRankMsg.text('Hmmmm, there was a problem loading the most liked quotes.');
                _quoteRankMsg.attr('class', 'text-danger');
                _quoteRankMsg.fadeOut(10000);
            }  
        } else {
            resp = await fetch(_quotesByRankUrl, {
                mode: "cors",
                headers: {
                    'Accept': 'application/json'
                }
            });

            if (resp.status === 200) {
                let quotesResult = await resp.json();
                let quotes = quotesResult.quotes;

                let latestLastModified = new Date(quotesResult.quotesLastModified);

                if (latestLastModified.getTime() > _quotesLastModified.getTime()) {
                    _quotesLastModified = latestLastModified;

                    // loop through the quotes and add them to the UL list:
                    _mostLikedQuotesList.empty();

                    for (let i = 0; i < quotes.length; i++) {
                        _mostLikedQuotesList.append(`
                        <li data-quote-id="${quotes[i].quoteId}">
                        "${quotes[i].content}" (ID:${quotes[i].quoteId}) The author is: ${quotes[i].author} 
                        <b>Likes:</b> ${quotes[i].likes}`);
                    }
                }
            }
            else {
                _quoteRankMsg.text('Hmmmm, there was a problem loading the most liked quotes.');
                _quoteRankMsg.attr('class', 'text-danger');
                _quoteRankMsg.fadeOut(10000);
            }  
        }
    });

    $('#selectTagBtn').click(async function () {
        let _quotesLastModified = new Date(1970, 0, 1);
        const selectTag = $('#quoteTag option:selected').val();

        const resp = await fetch(_quotesUrl + '?tagId=' + selectTag, {
            mode: "cors",
            headers: {
                'Accept': 'application/json'
            }
        });

        if (resp.status === 200) {
            let quotesResult = await resp.json();
            let quotes = quotesResult.quotes;

            let latestLastModified = new Date(quotesResult.quotesLastModified);

            _quotesByTag.empty();

            if (latestLastModified.getTime() > _quotesLastModified.getTime()) {
                _quotesLastModified = latestLastModified;

                // loop through the quotes and add them to the UL list:

                for (let i = 0; i < quotes.length; i++) {
                    _quotesByTag.append(`
                    <li data-quote-id="${quotes[i].quoteId}">
                    "${quotes[i].content}" (ID:${quotes[i].quoteId}) The author is: ${quotes[i].author} 
                    `);
                }
            }
        }
        else {
            _quoteRankMsg.text('Hmmmm, there was a problem loading the most liked quotes.');
            _quoteRankMsg.attr('class', 'text-danger');
            _quoteRankMsg.fadeOut(10000);     
        }
    });


    
    loadBaseApiInfo();

    // set up a timer to call load quotes
    setInterval(loadQuotes, 1000);

});
