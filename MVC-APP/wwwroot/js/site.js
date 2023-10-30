// Notes and templates

var loadTemplateBtn = document.getElementById('loadTemplateBtn');

loadTemplateBtn.addEventListener('click', function() {
  const notesTextarea = document.getElementById('notesTextArea');  
  const notesValue = notesTextarea.value;

  // Set the notes value as the value of the hidden input field
  const notesInput = document.getElementById('notesInput');
  notesInput.value = notesValue;
})

var addNotesBtn = document.getElementById('addNotesBtn');
addNotesBtn.addEventListener('click', function() {
  const responseTextArea = document.getElementById('responseTextArea');
  const responseValue = responseTextArea.value;

  // Set the response value as the value of the hidden input field
  const responseInput = document.getElementById('responseInput');
  responseInput.value = responseValue;
})

// Pagination
// Show 5 cards per page
var cardsPerPage = 5;

// Get the card elements
var cards = document.querySelectorAll('.card.ticket-card');

// Calculate the total number of pages
var totalPages = Math.ceil(cards.length / cardsPerPage);

// Create a function to display the cards for a given page
function showCards(page) {
  // Calculate the start and end indices for the cards to display
  var startIndex = (page - 1) * cardsPerPage;
  var endIndex = startIndex + cardsPerPage;

  // Hide all cards
  cards.forEach(function(card) {
    card.style.display = 'none';
  });

  // Display the cards for the current page
  for (var i = startIndex; i < endIndex; i++) {
    if (cards[i]) {
      cards[i].style.display = 'block';
    }
  }
}

// Create the pagination buttons
var paginationContainer = document.getElementById('pagination-container');
var currentPage = 1;

function updatePagination() {
  paginationContainer.innerHTML = '';

  // Calculate the total number of pages
  var totalPages = Math.ceil(cards.length / cardsPerPage);

  // Only display pagination if there are cards
  if (totalPages > 0) {
    // Create the previous button
    var previousButton = document.createElement('button');
    previousButton.textContent = 'Previous';
    previousButton.classList.add('page-link');
    previousButton.disabled = (currentPage === 1);
    previousButton.addEventListener('click', function() {
      currentPage--;
      showCards(currentPage);
      updatePagination();
    });
    paginationContainer.appendChild(previousButton);

    // Create the label for the current page
    var pageLabel = document.createElement('span');
    pageLabel.textContent = 'Page ' + currentPage + ' of ' + totalPages;
    pageLabel.style.margin = '10px';
    paginationContainer.appendChild(pageLabel);

    // Create the next button
    var nextButton = document.createElement('button');
    nextButton.classList.add('page-link');
    nextButton.textContent = 'Next';
    nextButton.disabled = (currentPage === totalPages);
    nextButton.addEventListener('click', function() {
      currentPage++;
      showCards(currentPage);
      updatePagination();
    });
    paginationContainer.appendChild(nextButton);
  }
}

// Show the cards for the first page initially
showCards(currentPage);
updatePagination();




