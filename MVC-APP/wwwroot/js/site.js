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
  for (var i = 1; i <= totalPages; i++) {
    var button = document.createElement('button');
    button.classList.add('page-item','page-link');
    button.textContent = i;
    button.addEventListener('click', function() {
      // Get the page number from the button text
      var page = parseInt(this.textContent);
      showCards(page);
    });
    paginationContainer.appendChild(button);
  }

  // Show the cards for the first page initially
  showCards(1);
